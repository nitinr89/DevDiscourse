﻿//using Microsoft.AspNetCore.SignalR;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;

//namespace Devdiscourse.Hubs
//{
//    public class FileHub : Hub
//    {
//        public override Task OnConnected()
//        {
//            signalConnectionId(this.Context.ConnectionId);
//            return base.OnConnected();
//        }

//        private void signalConnectionId(string signalConnectionId)
//        {
//            Clients.Client(signalConnectionId).signalConnectionId(signalConnectionId);
//        }
//        public void Send(string name, string message)
//        {
//            // Call the addNewMessageToPage method to update clients.
//            Clients.All.addNewMessageToPage(name, message);
//        }
//        public void SendProgresss(string signalConnectionId, string progress)
//        {
//            // Call the addNewMessageToPage method to update clients.
//            Clients.Client(signalConnectionId).FileProgress(progress);
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