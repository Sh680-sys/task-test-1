using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class Post : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    [Required, MaxLength(2000)]
    public string Content { get; set; } = null!;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
}