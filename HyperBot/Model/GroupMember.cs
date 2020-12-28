using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperBot.Model
{
    public class GroupMember
    {
        public string QQ { get; set; }
        public string NickName { get; set; }
        public string GroupNickName { get; set; }
        public bool IsOwner { get; set; } = false;
        public DateTime LastMessageDate { get; set; } = new DateTime();
    }
}
