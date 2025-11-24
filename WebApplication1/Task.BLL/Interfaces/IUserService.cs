using System.Threading.Tasks;
using BLL.DTOs;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        Task<(bool Success, string Error)> RegisterAsync(CreateUserDto dto, string origin);
        Task<(bool Success, string Error)> ConfirmEmailAsync(int userId, string token);

        // Login يرجع Access + Refresh
        Task<(bool Success, AuthResponseDto Response, string Error)> LoginAsync(string email, string password, string ipAddress);

        // Refresh access using refresh token string
        Task<(bool Success, AuthResponseDto Response, string Error)> RefreshTokenAsync(string token, string ipAddress);

        // Revoke a refresh token (logout)
        Task<(bool Success, string Error)> RevokeRefreshTokenAsync(string token, string ipAddress);
    }
}