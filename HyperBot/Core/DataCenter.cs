using HyperBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XQ.Net.SDK.Models;

namespace HyperBot.Core
{
    public class DataCenter
    {
        public static Dictionary<string, GroupClient> GroupClients = new Dictionary<string, GroupClient>();

        public static GroupClient GetClient(XQGroup client,string robot_qq)
        {
            if (GroupClients.ContainsKey(client.Id))
            {
                return GroupClients[client.Id];
            }

            var GroupClient = new GroupClient() {
                Group=client,
                RobotQQ= robot_qq
            };
            GroupClients.Add(client.Id, GroupClient);
            return GroupClient;
        }
    }
}
