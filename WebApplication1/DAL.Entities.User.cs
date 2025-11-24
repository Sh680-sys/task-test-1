public class User : BaseEntity
{
    public string UserName { get; set; }
    public string DisplayName { get; set; }
    public ICollection<Todo> Todos { get; set; }
    public ICollection<Post> Posts { get; set; }
    public ICollection<Comment> Comments { get; set; }
    public ICollection<Like> Likes { get; set; }
    public ICollection<Notification> Notifications { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
    public object? Email { get; internal set; }
    public string PasswordHash { get; set; } // <-- Add this property to fix CS1061
}