using HyperBot.Core;
using HyperBot.Model;
using HyperBot.Utils;
using RolePlayChat.Core.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HyperBot.Command
{
    public class ToolUser
    {
        public EcUser EcUser { get; set; }
        public DateTime LastGua { get; set; } = new DateTime(1970, 1, 1);
    }
    public class ToolCommand: BaseCommand
    {
        Dictionary<string, ToolUser> users = new Dictionary<string, ToolUser>();
        public ToolCommand()
        {
            Reg["MD5加密"] = (v, content) => {
                EcUser user =CommandSystem.GetModule(typeof(EcommerceCommand)).GetPlugInter("get").Invoke(content);
                if (user.Gold < 100)
                {
                    return $"{user.QQName}呀,你的钱,还不够呀!";
                }
                user.Gold -= 100;
                MD5 md5 = MD5.Create();
                byte[] md5buffer = md5.ComputeHash(Encoding.UTF8.GetBytes(v));
                string str = "";
                foreach (byte b in md5buffer)
                {
                    str += b.ToString("x2");
                }
                return $"已扣除[{user.QQName}]100金\r\n[{v}]的加密结果是[{str}]";
            };
            Reg["算卦"] = (v, content) =>
            {
                EcUser user = CommandSystem.GetModule(typeof(EcommerceCommand)).GetPlugInter("get").Invoke(content);

                ToolUser tooluser = GetUser(user, content.FromGroup);
                if (DateTime.Now.ToString("yyyy-MM-dd") == tooluser.LastGua.ToString("yyyy-MM-dd"))
                {
                    return $"{user.QQName}施主！今天你已经算过卦啦！明天再来吧！";
                }
                if (user.Gold < 100)
                {
                    return $"{user.QQName}施主!您的香火钱不够呀!";
                }
                tooluser.LastGua = DateTime.Now;
                user.Gold -= 100;
                return YiJing.GetGua();
            };
            Reg["算命"] = Reg["算卦"];
            Reg["抽签"] = Reg["算卦"];
        }

        ToolUser GetUser(EcUser user, GroupClient group)
        {
            string id = group.Group.Id + "_" + user.QQId;
            if (users.ContainsKey(id))
            {
                return users[id];
            }

            ToolUser _user = new ToolUser() {
                EcUser=user
            };
            users.Add(id, _user);
            return users[id];
        }
    }
}
