using BLL.DTOs;
using BLL.Interfaces;
using DAL.Data;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives; // for IChangeToken
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Task.BLL.Interfaces;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _db;
        private readonly IEmailSender _emailSender;
        private readonly JwtService _jwt;
        private readonly IConfiguration _config;

        public UserService(AppDbContext db, IEmailSender emailSender, JwtService jwt, IConfiguration config)
        {
            _db = db;
            _emailSender = emailSender;
            _jwt = jwt;
            _config = config;
        }

        // --- existing Hash/VerifyPassword methods kept as before ---
        private string HashPassword(string password)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            rng.GetBytes(salt);
            using var derive = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var hash = derive.GetBytes(32);
            var combined = new byte[48];
            Buffer.BlockCopy(salt, 0, combined, 0, 16);
            Buffer.BlockCopy(hash, 0, combined, 16, 32);
            return Convert.ToBase64String(combined);
        }

        private bool VerifyPassword(string hashed, string provided)
        {
            try
            {
                var combined = Convert.FromBase64String(hashed);
                var salt = new byte[16];
                Buffer.BlockCopy(combined, 0, salt, 0, 16);
                using var derive = new Rfc2898DeriveBytes(provided, salt, 10000, HashAlgorithmName.SHA256);
                var hash = derive.GetBytes(32);
                for (int i = 0; i < 32; i++)
                {
                    if (combined[16 + i] != hash[i]) return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        // Helper: generate secure random token string (base64url safe)
        private string GenerateRandomTokenString(int size = 48)
        {
            var bytes = new byte[size];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            // use Base64Url (replace +/ with -_)
            var token = Convert.ToBase64String(bytes);
            token = token.Replace("+", "-").Replace("/", "_").Replace("=", "");
            return token;
        }

        // Create refresh token entity and save
        private RefreshToken CreateRefreshToken(string ipAddress)
        {
            var ttlMinutes = _config.GetValue<int?>("Jwt:RefreshTokenExpiresMinutes") ?? 60 * 24 * 7; // default 7 days
            return new RefreshToken
            {
                Token = GenerateRandomTokenString(64),
                Expires = DateTime.UtcNow.AddMinutes(ttlMinutes),
                CreatedAt = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                Revoked = false
            };
        }

        // Login -> returns access + refresh
        public async Task<(bool Success, AuthResponseDto Response, string Error)> LoginAsync(string email, string password, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return (false, null, "Invalid credentials");

            var user = await _db.Users.Include(u => u.RefreshTokens).SingleOrDefaultAsync(u => u.Email == email.Trim().ToLowerInvariant());
            if (user == null) return (false, null, "Invalid credentials");
            if (!VerifyPassword(user.PasswordHash, password)) return (false, null, "Invalid credentials");
            if (!user.IsEmailConfirmed) return (false, null, "Email not confirmed");

            // access token
            var accessToken = _jwt.GenerateToken(user);

            // create refresh token and save
            var refreshToken = CreateRefreshToken(ipAddress);
            user.RefreshTokens ??= new System.Collections.Generic.List<RefreshToken>();
            user.RefreshTokens.Add(refreshToken);

            // cleanup old expired tokens (optional)
            user.RefreshTokens = user.RefreshTokens.Where(rt => !rt.Revoked && rt.Expires > DateTime.UtcNow.AddDays(-30)).ToList();

            await _db.SaveChangesAsync();

            var jwtExpires = _config.GetSection("Jwt")?.GetSection("ExpiresMinutes")?.Value is string val && int.TryParse(val, out var minutes) ? minutes : 60;

            var resp = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresInMinutes = jwtExpires
            };

            return (true, resp, null);
        }

        // Refresh flow: rotate token (create new refresh token, revoke old)
        public async Task<(bool Success, AuthResponseDto Response, string Error)> RefreshTokenAsync(string token, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token)) return (false, null, "Token is required");

            var dbToken = await _db.RefreshTokens
                .Include(rt => rt.User)
                .Where(rt => rt.Token == token)
                .FirstOrDefaultAsync();

            if (dbToken == null) return (false, null, "Invalid refresh token");
            if (dbToken.Revoked) return (false, null, "Refresh token revoked");
            if (dbToken.Expires < DateTime.UtcNow) return (false, null, "Refresh token expired");

            // rotate: revoke current, issue new one
            dbToken.Revoked = true;
            dbToken.RevokedByIp = ipAddress;

            var newRefreshToken = CreateRefreshToken(ipAddress);
            newRefreshToken.CreatedAt = DateTime.UtcNow;
            newRefreshToken.UserId = dbToken.UserId;

            // save
            _db.Set<RefreshToken>().Add(newRefreshToken);
            await _db.SaveChangesAsync();

            // generate new access token
            var newAccessToken = _jwt.GenerateToken(dbToken.User);

            var resp = new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresInMinutes = _config.GetSection("Jwt")?.GetSection("ExpiresMinutes")?.Value is string val2 && int.TryParse(val2, out var minutes2) ? minutes2 : 60
            };

            return (true, resp, null);
        }

        // Revoke a refresh token (logout)
        public async Task<(bool Success, string Error)> RevokeRefreshTokenAsync(string token, string ipAddress)
        {
            if (string.IsNullOrWhiteSpace(token)) return (false, "Token is required");

            var dbToken = await _db.RefreshTokens.Where(rt => rt.Token == token).FirstOrDefaultAsync();
            if (dbToken == null) return (false, "Token not found");

            if (dbToken.Revoked) return (false, "Token already revoked");

            dbToken.Revoked = true;
            dbToken.RevokedByIp = ipAddress;
            await _db.SaveChangesAsync();
            return (true, null);
        }

        public Task<(bool Success, string Error)> RegisterAsync(CreateUserDto dto, string origin)
        {
            throw new NotImplementedException();
        }

        public Task<(bool Success, string Error)> ConfirmEmailAsync(int userId, string token)
        {
            throw new NotImplementedException();
        }

        // باقي دوال Register/ConfirmEmail إلخ موجودة في نفس الملف كما قبل...
    }
}