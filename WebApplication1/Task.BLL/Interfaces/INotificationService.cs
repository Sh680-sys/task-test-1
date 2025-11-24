using BLL.DTOs;

namespace BLL.Interfaces;

public interface INotificationService
{
    Task<List<NotificationDto>> GetNotificationsAsync(int userId, CancellationToken ct = default);
    Task<bool> MarkAsReadAsync(int notificationId, CancellationToken ct = default);
}