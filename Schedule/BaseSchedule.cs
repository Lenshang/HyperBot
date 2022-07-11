using HyperBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Schedule
{
    public abstract class BaseSchedule
    {
        public bool Enable { get; set; } = false;
        public DateTime NextRunTime { get; set; } = new DateTime();
        public string ScheduleName { get; set; } = "";
        public abstract void Do(GroupClient group, UserClient user);
        public abstract DateTime GetNextRunTime(DateTime currents);
        public List<IClient> clients = new List<IClient>();
        public object locker = new object();
        public virtual void AddClient(IClient client)
        {
            lock (locker)
            {
                if (this.clients.Where(i => i.Id == client.Id).FirstOrDefault() == null)
                {
                    this.clients.Add(client);
                }
            }
        }
        public virtual void RemoveClient(IClient client)
        {
            lock (locker)
            {
                this.clients.RemoveAll(i => i.Id == client.Id);
            }
        }
    }
}
