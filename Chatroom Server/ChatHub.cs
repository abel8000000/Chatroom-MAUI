using Microsoft.AspNetCore.SignalR;

namespace Chatroom_Server.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly HashSet<string> ConnectedUsers = new HashSet<string>();

        public async Task ConnectUser(string username)
        {
            if (!ConnectedUsers.Contains(username))
            {
                ConnectedUsers.Add(username);
                await Clients.All.SendAsync("UserConnected", username);
            }
        }

        public async Task DisconnectUser(string username)
        {
            if (ConnectedUsers.Contains(username))
            {
                ConnectedUsers.Remove(username);
                await Clients.All.SendAsync("UserDisconnected", username);
            }
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
