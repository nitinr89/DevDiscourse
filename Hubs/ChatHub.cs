using Microsoft.AspNet.SignalR;

namespace Devdiscourse.Hubs
{
    public class ChatHub : Hub
    {
        public void send(string name, string message)
        {
            // call the addnewmessagetopage method to update clients.
            Clients.All.addnewmessagetopage(name, message);
        }
        public void sendnotification(string title, string description, string url)
        {
            Clients.All.sendnotificationtoall(title, description, url);
        }
        public void updatemap(string url)
        {
            // call the addnewmessagetopage method to update clients.
            Clients.All.updatedataonmap(url);
        }
    }
}