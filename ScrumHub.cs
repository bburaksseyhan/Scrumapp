using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace scrum_ui
{
    public class ScrumHub : Hub
    {
        private static HashSet<string> ActiveUsers = new HashSet<string>();

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"OnConnectionAsync {Context.ConnectionId}");
            ActiveUsers.Add(Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"OnDisconnectAsync {Context.ConnectionId}");
            ActiveUsers.Remove(Context.ConnectionId);

            return base.OnDisconnectedAsync(exception);
        }
    }
}