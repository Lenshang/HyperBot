using HyperBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Model
{
    public class CoreUser
    {
        public string Id { get; set; }
        public int Gold { get; set; } = 2000;
        public string QQId { get; set; }
        public string QQName { get; set; }
        public Dictionary<string, dynamic> JSStorage { get; set; } = new Dictionary<string, dynamic>();
        public CoreUser GetDbItem()
        {
            return new CoreUser(){ Id=this.Id,Gold=this.Gold,QQId=this.QQId};
        }
        public static CoreUser Create(CoreUser user)
        {
            return new CoreUser() { Id = user.Id, Gold = user.Gold, QQId = user.QQId};
        }
    }
}
