using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using System;
using System.Linq;
using System.Threading.Tasks;
using TcpExample.Domain.Common;
using TcpExample.Domain.DBModel;

namespace TcpExample.UI.Hubs
{
    public class DataHub : Hub
    {
        public async Task ReciveMessage(string MarkCode, string SerialNumber,string signal,string dateTime)
        {
            long serial = long.Parse(SerialNumber);
            var users=ConstantsVariables.list.Where(a => a.MarkCode == MarkCode && a.SerialNumber == serial).Select(a => a.UserId).ToList();
            await Clients.Clients(users).SendAsync("ReceiveMessage", MarkCode,  SerialNumber,  signal,  dateTime);
        }

        public void GetDeviceActivities(string marckCode, string serialNumber)
        {
            ConstantsVariables.list.Add(new OnlineUserModel()
            {
                UserId = Context.ConnectionId,
                MarkCode = marckCode,
                SerialNumber =long.Parse( serialNumber)
            });

        }
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var users = ConstantsVariables.list.Where(a => a.UserId == Context.ConnectionId).ToList();
            foreach (var item in users)
            {
                ConstantsVariables.list.Remove(item);
            }
            return base.OnDisconnectedAsync(exception);
        }
        

    }
}
