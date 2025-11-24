namespace BLL.DTOs
{
    public class AuthResponseDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresInMinutes { get; set; }
    }

    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; }
    }

    public class RevokeRequestDto
    {
        public string RefreshToken { get; set; }
    }
}