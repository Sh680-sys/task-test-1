using AutoMapper;
using BLL.DTOs;
using BLL.Interfaces;
using DAL.Entities;
using DAL.Interfaces;

namespace BLL.Services;

public class NotificationService(
    IRepository<Notification> notifRepo,
    IUnitOfWork uow,
    IMapper mapper) : INotificationService
{
    private readonly IRepository<Notification> _notifRepo = notifRepo;
    private readonly IUnitOfWork _uow = uow;
    private readonly IMapper _mapper = mapper;

    public async Task<List<NotificationDto>> GetNotificationsAsync(int userId, CancellationToken ct = default)
    {
        var items = await _notifRepo.FindAsync(n => n.UserId == userId, ct);
        // هون ممكن ترتّب حسب التاريخ عالسريع
        var ordered = items.OrderByDescending(n => n.CreatedAt).ToList();
        return ordered.Select(_mapper.Map<NotificationDto>).ToList();
    }

    public async Task<bool> MarkAsReadAsync(int notificationId, CancellationToken ct = default)
    {
        var list = await _notifRepo.FindAsync(n => n.Id == notificationId, ct);
        var notif = list.FirstOrDefault();
        if (notif is null) return false;
        if (!notif.IsRead)
        {
            notif.IsRead = true;
            _notifRepo.Update(notif);
            await _uow.SaveChangesAsync(ct);
        }
        return true;
    }
}