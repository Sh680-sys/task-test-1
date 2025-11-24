using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DAL.Entities;

namespace BLL.Services
{
    // خدمة بسيطة لتوليد JWT
    public class JwtService
    {
        private readonly IConfiguration _config;
        public JwtService(IConfiguration config)
        {
            _config = config;
        }

        // يولد توكن JWT بناءً على بيانات اليوزر
        public string GenerateToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            var jwtSection = _config.GetSection("Jwt");
            var keyString = jwtSection.GetSection("Key").Value;
            if (string.IsNullOrWhiteSpace(keyString))
                throw new InvalidOperationException("JWT key is not configured. Set Jwt:Key in configuration.");

            var issuer = jwtSection.GetSection("Issuer").Value;
            var audience = jwtSection.GetSection("Audience").Value;
            var expiresMinutesStr = jwtSection.GetSection("ExpiresMinutes").Value;
            int expiresMinutes = 60;
            if (!string.IsNullOrWhiteSpace(expiresMinutesStr) && int.TryParse(expiresMinutesStr, out var parsedMinutes))
                expiresMinutes = parsedMinutes;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, (string)(user.Email ?? string.Empty)),
                new Claim("name", user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}