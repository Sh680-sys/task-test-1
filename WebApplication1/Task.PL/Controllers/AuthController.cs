using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Task.BLL.DTOs;

namespace PL.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _users;
        public AuthController(IUserService users)
        {
            _users = users;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var (success, response, error) = await _users.LoginAsync(dto.Email, dto.Password, ip);
            if (!success) return BadRequest(new { message = error });

            // ممكن تخزن الريفرش توكن في httpOnly cookie بدل body لو تحب
            return Ok(response);
        }

        // POST: api/auth/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var (success, response, error) = await _users.RefreshTokenAsync(dto.RefreshToken, ip);
            if (!success) return BadRequest(new { message = error });
            return Ok(response);
        }

        // POST: api/auth/revoke
        [Authorize]
        [HttpPost("revoke")]
        public async Task<IActionResult> Revoke([FromBody] RevokeRequestDto dto)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var (success, error) = await _users.RevokeRefreshTokenAsync(dto.RefreshToken, ip);
            if (!success) return BadRequest(new { message = error });
            return Ok(new { message = "Refresh token revoked" });
        }
    }
}