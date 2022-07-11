using System.Collections.Generic;

namespace HyperBot.Extension.Genshin
{
    public class DB_EcUser
    {
        public string Id { get; set; }
        public int Gold { get; set; } = 2000;
        //public XQQQ QQ { get; set; }
        public string QQId { get; set; }
        public string QQName { get; set; }
        public Dictionary<string, UserItem> Items { get; set; } = new Dictionary<string, UserItem>();
    }
}
