using HyperBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Core
{
    public class StoreSystem
    {
        public static Dictionary<string, GroupClient> GroupClients = new Dictionary<string, GroupClient>();
        public static List<MessageTask> MessageTasks = new List<MessageTask>();
        public static GroupClient GetClient(string id)
        {
            if (GroupClients.ContainsKey(id))
            {
                return GroupClients[id];
            }
            return null;
        }
        public static GroupClient AddClient(Model.ApiModel.GroupInfo group)
        {
            if (GroupClients.ContainsKey(group.id))
            {
                return GroupClients[group.id];
            }
            else
            {
                GroupClient client = new GroupClient(group.id,group.name,group.owner);
                foreach(var member in group.members)
                {
                    client.AddMembers(member.id, member.name, group.adminids.Contains(member.id));
                }
                GroupClients[group.id] = client;
                return client;
            }
        }
        public static void AddMessage(string message,IClient target)
        {
            MessageTasks.Add(new MessageTask()
            {
                Message=message,
                Target=target
            });
        }
        public static List<MessageTask> GetMessages()
        {
            var r = MessageTasks;
            MessageTasks = new List<MessageTask>();
            return r;
        }
    }   
}
