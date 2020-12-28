using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RolePlayChat.Core.Command
{
    public class TestCommand:BaseCommand
    {
        public TestCommand()
        {
            Reg["test"] = (v, content) =>
            {
                //var battleModule=WSCommand.GetModule(typeof(BattleCommand));
                //var r=battleModule.GetPlugInter("gold").Invoke(client);
                //client.SendAsync(new WSSendMessage()
                //{
                //    Value = "test gold:"+r,
                //    SenderName = "System",
                //    Date = DateTime.Now,
                //    SenderId = "",
                //    Action = "msg"
                //});
                return "HyperRobot";
            };
        }
    }
}
