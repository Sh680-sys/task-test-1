using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Comment : BaseEntity
{
    public int PostId { get; set; }
    public Post? Post { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }

    [Required, MaxLength(1000)]
    public string Content { get; set; } = null!;
}