using HyperBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XQ.Net.SDK.Models;

namespace RolePlayChat.Core.Command
{
    public class BaseCommand
    {
        public Dictionary<string, Func<string, MessageContent, string>> Reg = new Dictionary<string, Func<string, MessageContent, string>>();
        public Dictionary<string, Func<dynamic, dynamic>> PlugInter = new Dictionary<string, Func<dynamic, dynamic>>();

        public Func<dynamic, dynamic> GetPlugInter(string name)
        {
            if (!this.PlugInter.ContainsKey(name))
            {
                return null;
            }
            return this.PlugInter[name];
        }
        protected void SendMessage(GroupClient client, string msg,string systemName="Command System")
        {
            client.Group.SendMessage(client.RobotQQ, msg);
            //client.SendAsync(new WSSendMessage()
            //{
            //    Value = msg,
            //    SenderName = systemName,
            //    Date = DateTime.Now,
            //    SenderId = "",
            //    Action = "msg"
            //});
        }
    }
}
