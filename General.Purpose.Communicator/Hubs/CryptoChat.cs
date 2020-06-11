using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace General.Purpose.Communicator.Hubs
{
    [Authorize]
    public class CryptoChat : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            var localDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
            await Clients.All.SendAsync("ReceiveMessage", user, localDate + ": " + message);
            await UpdateUsersStatus();
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("SetOnlineUsers", $"{Context.User.Identity.Name} вошел в чат");
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.All.SendAsync("SetOnlineUsers", $"{Context.User.Identity.Name} покинул в чат");
            await base.OnDisconnectedAsync(exception);
        }

        public async Task UpdateUsersStatus()
        {
            try
            {
                await Clients.AllExcept(Context.ConnectionId).SendAsync("SetOnlineUsers", Context.User.Identity.Name + " в сети!");
            }
            catch(Exception exception)
            {
                await Clients.All.SendAsync("SetOnlineUsers", "NONE");
            }
        }
    }
}
