using DAL.Enums;

namespace BLL.DTOs;

public record NotificationDto(
    int Id,
    int UserId,
    NotificationType Type,
    string Message,
    bool IsRead,
    int? PostId,
    int? TodoId,
    DateTime CreatedAt
);