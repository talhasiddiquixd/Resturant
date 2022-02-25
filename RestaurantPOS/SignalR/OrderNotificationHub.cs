using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
namespace RestaurantPOS.SignalR
{
    public class OrderNotificationHub : Hub
    {
        private readonly IOrderNotificationsManager _orderNotificationsManager;
        public OrderNotificationHub(IOrderNotificationsManager orderNotificationsManager)
        {
            _orderNotificationsManager = orderNotificationsManager;
        }
        /// <summary>
        /// Register for orders Notifications
        /// </summary>
        /// <returns></returns>
        public string RegisterForOrderNotifications()
        {
            var httpContext = this.Context.GetHttpContext();
            //// var userId = httpContext.Request.Query["userId"];
            _orderNotificationsManager.KeepUserConnection("1", Context.ConnectionId);
            return Context.ConnectionId;
        }
        //// Called when a connection with the hub is terminated....
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            //get the connectionId
            var connectionId = Context.ConnectionId;
            _orderNotificationsManager.RemoveUserConnection(connectionId);
            var value = await Task.FromResult(0);
        }
    }
}
