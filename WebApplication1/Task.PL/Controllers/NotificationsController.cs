using BLL.DTOs;
using BLL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace PL.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController(INotificationService notifService) : ControllerBase
{
    private readonly INotificationService _notifService = notifService;

    [HttpGet]
    public async Task<ActionResult<List<NotificationDto>>> Get([FromQuery] int userId, CancellationToken ct)
    {
        // هون منرجع إشعارات اليوزر مرتبة
        var list = await _notifService.GetNotificationsAsync(userId, ct);
        return Ok(list);
    }

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id, CancellationToken ct)
    {
        var ok = await _notifService.MarkAsReadAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}