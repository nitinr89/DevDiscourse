//using Microsoft.AspNetCore.SignalR;

//namespace Devdiscourse.Hubs
//{
//    public class ChatHub : Hub
//    {
//        public void Send(string name, string message)
//        {
//            // Call the addNewMessageToPage method to update clients.
//            Clients.All.addNewMessageToPage(name, message);
//        }
//        public void SendNotification(string title, string description, string url)
//        {
//            Clients.All.sendNotificationToAll(title, description, url);
//        }
//        public void UpdateMap(string url)
//        {
//            // Call the addNewMessageToPage method to update clients.
//            Clients.All.updateDataonMap(url);
//        }
//    }

//}