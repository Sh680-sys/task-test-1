using System.ComponentModel.DataAnnotations;

namespace DAL.Entities;

public class User : BaseEntity
{
    [Required, MaxLength(50)]
    public string UserName { get; set; } = null!;

    [Required, MaxLength(100)]
    public string DisplayName { get; set; } = null!;

    public ICollection<Todo> Todos { get; set; } = new List<Todo>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public string PasswordHash { get; set; } // <-- Add this property to fix CS1061

    public object? Email { get; internal set; }
    public bool IsEmailConfirmed { get; set; }
}