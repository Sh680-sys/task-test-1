using DAL.Enums;

namespace DAL.Entities;

public class Notification : BaseEntity
{
    public int UserId { get; set; }
    public User? User { get; set; }

    public NotificationType Type { get; set; }
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; }

    public int? PostId { get; set; }
    public int? TodoId { get; set; }
}