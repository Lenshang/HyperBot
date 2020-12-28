using HyperBot.Core;
using HyperBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XQ.Net.SDK.Attributes;
using XQ.Net.SDK.EventArgs;
namespace HyperBot
{
    [Plugin]
    public class Genshin
    {
        [GroupMsgEvent]
        public static void onGroupMsg(object sender, XQAppGroupMsgEventArgs e)
        {
            // XQ.Net.SDK.XQAPI.OutPutLog(e.FromGroup.GetGroupMemberList_B(e.RobotQQ));
            //GroupClient client = new GroupClient()
            //{
            //    EventArgs = e,
            //    Group = e.FromGroup,
            //    QQ = e.FromQQ,
            //    RobotQQ = e.RobotQQ
            //};
            //string r=CommandSystem.Execute(e.Message.Text, client);
            //if (!string.IsNullOrEmpty(r))
            //{
            //    e.FromGroup.SendMessage(e.RobotQQ, r);
            //}
            //if (e.Message.Text == "hello")
            //{
            //    e.FromGroup.SendMessage(e.RobotQQ, "Hi nice to meet you!");
            //}
            try
            {
                GroupClient client = DataCenter.GetClient(e.FromGroup, e.RobotQQ);
                client.Update(e.FromGroup);

                MessageContent content = new MessageContent()
                {
                    FromQQ = e.FromQQ,
                    FromGroup = client,
                    Message = e.Message.Text
                };

                CommandSystem.Execute(content, (msg, target) =>
                {
                    if (!string.IsNullOrEmpty(msg))
                    {
                        target.Group.SendMessage(target.RobotQQ, msg);
                    }
                });
            }
            catch(Exception ex)
            {
                XQ.Net.SDK.XQAPI.OutPutLog(ex.ToString());
            }

        }
    }
}
