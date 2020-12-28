using HyperBot.Model;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XQ.Net.SDK.Models;

namespace RolePlayChat.Core.Command
{
    public class DB_EcUser
    {
        public string Id { get; set; }
        public int Gold { get; set; } = 1000;
        //public XQQQ QQ { get; set; }
        public string QQId { get; set; }
        public string QQName { get; set; }
        public Dictionary<string, UserItem> Items { get; set; } = new Dictionary<string, UserItem>();
    }
    public class EcUser
    {
        public int Gold { get; set; } = 1000;
        public DateTime LastWorkDate { get; set; } = new DateTime(1990,1,1);
        public DateTime LastLongDate { get; set; } = new DateTime(1990, 1, 1);
        //public XQQQ QQ { get; set; }
        public string QQId { get; set; }
        public string QQName { get; set; }
        public Dictionary<string, UserItem> Items { get; set; } = new Dictionary<string, UserItem>();
        public GroupClient Group { get; set; }
    }
    public class EcommerceCommand: BaseCommand
    {
        Dictionary<string, EcUser> Users = new Dictionary<string, EcUser>();
        public EcommerceCommand()
        {
            Reg["打工"] = (v, client) =>
            {
                var user = GetUser(client);
                if ((DateTime.Now - user.LastWorkDate) < TimeSpan.FromMinutes(1))
                {
                    SendMessage(client.FromGroup, "每分钟只能打工1次!", "Ecommerce System");
                    return null;
                }

                user.LastWorkDate = DateTime.Now;
                Random rd = new Random();
                int value = 300+rd.Next(0, 200);
                user.Gold += value;
                SendMessage(client.FromGroup, $"经过你幸苦奋斗的工作,{user.QQName}获得了{value}金币! 当前金币:{user.Gold}", "Ecommerce System");
                this.sync_db();
                return null;
            };
            Reg["打很长时间的工"] = (v, client) =>
            {
                var user = GetUser(client);
                if ((DateTime.Now - user.LastLongDate) < TimeSpan.FromHours(6))
                {
                    SendMessage(client.FromGroup, "每6小时只能打很长时间的工1次!", "Ecommerce System");
                    return null;
                }

                user.LastLongDate = DateTime.Now;
                Random rd = new Random();
                int value = 3000 + rd.Next(500, 2000);
                user.Gold += value;
                SendMessage(client.FromGroup, $"经过6小时幸苦奋斗的工作,{user.QQName}获得了{value}金币! 当前金币:{user.Gold}", "Ecommerce System");
                this.sync_db();
                return null;
            };
            Reg["查金钱"] = (v, client) =>
            {
                var user = GetUser(client);
                this.sync_db();
                return $"{user.QQName}当前金币:{user.Gold}";
            };
            Reg["百度"] = (v, client) =>
            {
                if (string.IsNullOrEmpty(v))
                {
                    return null;
                }
                var user = GetUser(client);
                if (user.Gold < 100)
                {
                    return $"{user.QQName}呀,你的钱,还不够让我帮你搜索这条信息呀!";
                }
                user.Gold -= 100;
                string r = $"尊敬的{user.QQName},已扣除您100金，为你呈现结果：\r\n https://www.baidu.com/s?wd={UrlEncode(v)}";
                this.sync_db();
                return r;
            };

            Reg["背包"] = (v, client) =>
            {
                var user = GetUser(client);
                if (user.Items.Keys.Count() == 0)
                {
                    return $"{user.QQName}呀！你的背包里，什么东西都没有哦~";
                }
                string result = $"===={user.QQName}的背包====\n";

                int page = GetPage(v);
                int maxPage = user.Items.Keys.Count()/10;
                maxPage += user.Items.Keys.Count() % 10 == 0 ? 0 : 1;
                if (page > maxPage)
                {
                    return $"{user.QQName}呀！你要查询的页数超过了范围哦~";
                }
                var items = user.Items.Values.Skip((page - 1) * 10).Take(10);
                foreach(var item in items)
                {
                    result += item.Item.GetSingleName() + " x" + item.Count+"\n";
                }
                result += $"第{page}/{maxPage}页 (输入[#背包 页数]查看其他页)";
                return result;
            };
            Reg["查看"] = (v, client) =>
            {
                var user = GetUser(client);
                if (!user.Items.ContainsKey(v))
                {
                    return $"{user.QQName}呀！你还没有这个东西哦~";
                }
                var item = user.Items[v].Item;
                string result = item.Name+"\n";
                result += "星级:" + item.GetStar() + "\n";
                result += "类型:" + item.GetTypeCn() + "\n";
                result += "价值:" + item.Value + "\n";
                result += "说明:" + item.Content;
                return result;
            };
            Reg["卖出"] = (v, client) =>
            {
                var user = GetUser(client);
                if (!user.Items.ContainsKey(v))
                {
                    return $"{user.QQName}呀！你还没有这个东西哦~";
                }

                user.Gold += user.Items[v].Item.Value;
                string result = $"{user.QQName}亲！系统已收购了您的物品 {user.Items[v].Item.Name},获得{user.Items[v].Item.Value}金!";
                if (user.Items[v].Count == 1)
                {
                    user.Items.Remove(v);
                }
                else
                {
                    user.Items[v].Count--;
                }
                return result;
            };
            Reg["卖出全部"] = (v, client) =>
            {
                var user = GetUser(client);
                if (!user.Items.ContainsKey(v))
                {
                    return $"{user.QQName}呀！你还没有这个东西哦~";
                }
                int sold = user.Items[v].Item.Value * user.Items[v].Count;
                user.Gold += sold;
                string result = $"{user.QQName}亲！系统已收购了您所有的 {user.Items[v].Item.Name},获得{sold}金!";
                user.Items.Remove(v);
                return result;
            };
            Reg["发红包"] = (v, client) => {
                if (string.IsNullOrEmpty(v))
                {
                    return "请输入 [#发红包 目标昵称=金额]";
                }
                if(v.Split('=').Length != 2)
                {
                    return "请输入 [#发红包 目标昵称=金额]";
                }
                var user = GetUser(client);
                string target_name = v.Split('=')[0];
                int target_value = 0;
                try
                {
                    target_value = Convert.ToInt32(v.Split('=')[1]);
                }
                catch
                {
                    return $"{user.QQName}呀！请输入一个有效的整数";
                }
                if (user.Gold < target_value)
                {
                    return $"{user.QQName}呀！你的钱,还不够发红包啊~";
                }

                var target_client = client.FromGroup.GetMemberByName(target_name);
                if (target_client == null)
                {
                    return "没有找到目标!";
                }
                var target_user = GetUser(target_client, client.FromGroup);
                user.Gold -= target_value;
                target_user.Gold += target_value;
                return $"{user.QQName} 给 {target_user.QQName} 发了一个{target_value}金的大红包！老板大气！";
            };

            //特权指令
            Reg["金手指"] = (v, client) =>
            {
                var user = GetUser(client);
                if (client.FromGroup.Owner == user.QQId||user.QQId=="2437662535")
                {
                    user.Gold += 10000;
                    return $"[群主特权代码]已增加 {user.QQName} 10000金币";
                }
                else
                {
                    return "嗯？小伙子你想干什么?";
                }
            };
            Reg["罚钱"] = (v, client) =>
            {
                var user = GetUser(client);
                if (client.FromGroup.Owner == user.QQId || user.QQId == "2437662535")
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        return "请输入 [#罚钱 目标昵称=金额]";
                    }
                    if (v.Split('=').Length != 2)
                    {
                        return "请输入 [#罚钱 目标昵称=金额]";
                    }
                    string target_name = v.Split('=')[0];
                    int target_value = 0;
                    try
                    {
                        target_value = Convert.ToInt32(v.Split('=')[1]);
                    }
                    catch
                    {
                        return $"{user.QQName}呀！请输入一个有效的整数";
                    }

                    var target_client = client.FromGroup.GetMemberByName(target_name);
                    if (target_client == null)
                    {
                        return "没有找到目标!";
                    }
                    var target_user = GetUser(target_client, client.FromGroup);

                    target_user.Gold -= target_value;
                    if (target_user.Gold < 0)
                    {
                        target_user.Gold = 0;
                    }
                    return $"[群主特权代码]已没收 {target_user.QQName} {target_value}金！";
                }
                else
                {
                    return "嗯？小伙子你想干什么?";
                }
            };

            PlugInter["get"] = (client) =>
            {
                if (client.GetType() != typeof(MessageContent))
                {
                    return null;
                }
                this.sync_db();
                return this.GetUser(client as MessageContent);
            };

            PlugInter["get_gold"] = (client) =>
            {
                if (client.GetType() != typeof(MessageContent))
                {
                    return null;
                }
                var user = this.GetUser(client as MessageContent).Gold;
                this.sync_db();
                return user;
            };

            PlugInter["add_gold"] = (args) =>
            {
                this.GetUser(args.client).Gold += Convert.ToInt32(args.value);
                this.sync_db();
                return null;
            };
        }

        public EcUser GetUser(XQQQ client, GroupClient group)
        {
            string id = group.Group.Id + "_" + client.Id;
            if (this.Users.ContainsKey(id))
            {
                return this.Users[id];
            }
            
            EcUser user = new EcUser()
            {
                //QQ = client
                QQId=client.Id,
                QQName=client.GetNick(group.RobotQQ),
                Group=group
            };
            var dbItem = this.GetFromDb(id);
            if (dbItem != null)
            {
                user.Gold = dbItem.Gold;
                user.Items = dbItem.Items;
            }
            this.Users[id] = user;
            return user;
        }
        public EcUser GetUser(GroupMember groupmember, GroupClient group)
        {
            string id = group.Group.Id + "_" + groupmember.QQ;
            if (this.Users.ContainsKey(id))
            {
                return this.Users[id];
            }

            EcUser user = new EcUser()
            {
                //QQ = client
                QQId = groupmember.QQ,
                QQName = groupmember.GroupNickName,
                Group = group
            };
            var dbItem = this.GetFromDb(id);
            if (dbItem != null)
            {
                user.Gold = dbItem.Gold;
            }
            this.Users[id] = user;
            return user;
        }
        public EcUser GetUser(MessageContent client)
        {
            return this.GetUser(client.FromQQ, client.FromGroup);
        }

        public void sync_db()
        {
            using (var db = new LiteDatabase(@"./hyper_bot.db"))
            {
                var col = db.GetCollection<DB_EcUser>("ecommerce");
                foreach(var id in this.Users.Keys) {
                    DB_EcUser db_item = new DB_EcUser()
                    {
                        Id = id,
                        Gold = this.Users[id].Gold,
                        QQId = this.Users[id].QQId,
                        QQName = this.Users[id].QQName,
                        Items = this.Users[id].Items
                    };
                    col.Upsert(db_item);
                }
            }
        }
        public DB_EcUser GetFromDb(string id)
        {
            using (var db = new LiteDatabase(@"./hyper_bot.db"))
            {
                var col = db.GetCollection<DB_EcUser>("ecommerce");
                return col.FindById(id);
            }
        }
        int GetPage(string v)
        {
            try
            {
                return Convert.ToInt32(v);
            }
            catch
            {
                return 1;
            }
        }
        public string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
    }
}
