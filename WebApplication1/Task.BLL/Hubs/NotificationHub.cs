using Microsoft.AspNetCore.SignalR;

namespace BLL.Hubs
{
    public class NotificationHub : Hub
    {
        public async System.Threading.Tasks.Task SendToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}