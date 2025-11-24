using System.ComponentModel.DataAnnotations;

namespace BLL.DTOs;

public class CreateTodoDto
{
    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    [Required]
    public int UserId { get; set; }
}