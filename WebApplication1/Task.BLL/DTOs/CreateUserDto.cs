using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class CreateUserDto
{
    [Required]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;
}