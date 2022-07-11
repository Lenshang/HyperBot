using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Model.ApiModel
{
    public class GroupInfo
    {
        public string id { get; set; }
        public string name { get; set; }
        public string owner { get; set; }
        public List<string> adminids { get; set; }
        public List<UserInfo> members { get; set; }
    }
    public class UserInfo
    {
        public string id { get; set; }
        public string name { get; set; }
    }
}
