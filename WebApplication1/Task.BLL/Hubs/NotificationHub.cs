using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace BLL.Hubs
{
    public class NotificationHub : Hub
    {
        public override ValueTask OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async ValueTask SendToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }
    }
}