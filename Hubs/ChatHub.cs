using Microsoft.AspNetCore.SignalR;

namespace Devdiscourse.Hubs
{
    public class ChatHub : Hub
    {
        public Task send(string name, string message)
        {
            // call the addnewmessagetopage method to update clients.
            return Clients.All.SendAsync("addnewmessagetopage", name, message);
        }
        public Task sendnotification(string title, string description, string url)
        {
            return Clients.All.SendAsync("sendnotificationtoall", title, description, url);
        }
        public Task updatemap(string url)
        {
            // call the addnewmessagetopage method to update clients.
            return Clients.All.SendAsync("updatedataonmap", url);
        }
    }
}