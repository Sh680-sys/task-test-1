using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Todo : BaseEntity
{
    [Required, MaxLength(200)]
    public string Title { get; set; } = null!;
    [MaxLength(2000)]
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? DueDate { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}