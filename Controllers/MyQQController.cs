using HyperBot.Core;
using HyperBot.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace HyperBot.Controllers
{
    public class MsgItem
    {
        public string MQ_robot { get; set; }
        public int? MQ_type { get; set; }
        public int? MQ_type_sub { get; set; }
        public string MQ_fromID { get; set; }
        public string MQ_fromQQ { get; set; }
        public string MQ_passiveQQ { get; set; }
        public string MQ_msg { get; set; }
        public string MQ_msgSeq { get; set; }
        public string MQ_msgID { get; set; }
        public string MQ_msgData { get; set; }
        public string MQ_timestamp { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class MyQQController : ControllerBase
    {
        [HttpPost]
        public string Get(MsgItem msg)
        {
            string msg_str = System.Web.HttpUtility.UrlDecode(msg.MQ_msg);
            System.Console.WriteLine("["+msg.MQ_fromID+"]"+msg.MQ_fromQQ+":"+ msg_str);
            GroupClient group = StoreSystem.GetClient(msg.MQ_fromID);
            if(group == null)
            {
                return "NOT INIT";
            }
            group.AddMembers(msg.MQ_fromQQ, "");
            var user = group.GetMemberById(msg.MQ_fromQQ);
            ScheduleSystem.RunOnce(group, user);
            CommandSystem.Execute(msg_str, group, user);
            
            return "OK";
        }
    }
}
