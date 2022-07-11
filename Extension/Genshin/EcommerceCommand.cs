using HyperBot.Command;
using HyperBot.Model;
using LiteDB;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyperBot.Extension.Genshin
{
    public class EcUser
    {
        public DateTime LastWorkDate { get; set; } = new DateTime(1990, 1, 1);
        public DateTime LastLongDate { get; set; } = new DateTime(1990, 1, 1);
        public DateTime LastQiTao { get; set; } = new DateTime(1990, 1, 1);
        public string QQId { get; set; }
        public string QQName { get; set; }
        public GroupClient Group { get; set; }

        public DB_EcUser DbUser { get; set; }
        public int Gold
        {
            get
            {
                return this.DbUser.Gold;
            }
            set
            {
                this.DbUser.Gold = value;
            }
        }

        public Dictionary<string, UserItem> Items
        {
            get
            {
                return this.DbUser.Items;
            }
            set
            {
                this.DbUser.Items = value;
            }
        }

        public int SoldCount { get; set; } = 0;
    }

    public class EcommerceCommand : BaseCommand
    {
        public IConfiguration config { get; set; }
        public EcommerceCommand(IConfiguration configuration)
        {
            this.config=configuration;
        }
        Dictionary<string, EcUser> Users = new Dictionary<string, EcUser>();
        public override void RegCmd()
        {
            Reg["打工"] = (v, _g, _u) => {
                var user = GetUser(_u,_g);
                if ((DateTime.Now - user.LastWorkDate) < TimeSpan.FromHours(1))
                {
                    int ts = Convert.ToInt32(60 - (DateTime.Now - user.LastWorkDate).TotalMinutes);
                    return $"每1小时只能打工1次!还需要等待{ts}分钟";
                }
                if ((DateTime.Now - user.LastLongDate) < TimeSpan.FromHours(16))
                {
                    return "你正在打很长时间的工!不能同时打其他工!";
                }
                user.LastWorkDate = DateTime.Now;
                Random rd = new Random();
                int value = 100 + rd.Next(0, 500);
                user.DbUser.Gold += value;
                
                //this.sync_db();
                return $"经过你幸苦奋斗的工作,{user.QQName}获得了{value}金币! 当前金币:{user.DbUser.Gold}";
            };
            Reg["打很长时间的工"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                if ((DateTime.Now - user.LastLongDate) < TimeSpan.FromHours(16))
                {
                    int ts = Convert.ToInt32(16 * 60 - (DateTime.Now - user.LastLongDate).TotalMinutes);
                    return $"每16小时只能打很长时间的工1次! 还需要等待{ts}分钟";
                }

                user.LastLongDate = DateTime.Now;
                Random rd = new Random();
                int value = 1000 + rd.Next(500, 3500);
                user.DbUser.Gold += value;
                return $"经过16小时幸苦奋斗的工作,{user.QQName}获得了{value}金币! 当前金币:{user.DbUser.Gold}";
            };
            Reg["打很长时间工"] = Reg["打很长时间的工"];
            Reg["打长工"] = Reg["打很长时间的工"];
            Reg["打很长很长时间的工"] = Reg["打很长时间的工"];
            Reg["乞讨"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                if ((DateTime.Now - user.LastQiTao) < TimeSpan.FromHours(24))
                {
                    return "什么什么？还想要？走开走开,没有没有~";
                }
                if (user.Gold >= 10000)
                {
                    return $"{user.QQName} 呀 你这么有钱还乞讨个啥？";
                }
                Random rd = new Random();
                int value = rd.Next(5000, 10000);
                user.DbUser.Gold += value;
                user.LastQiTao = DateTime.Now;
                return $"{user.QQName}呀,我今天高兴，就赏你{value}金币吧~";
            };
            Reg["讨饭"] = Reg["乞讨"];
            Reg["查金钱"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                //this.sync_db();
                return $"{user.QQName}当前金币:{user.DbUser.Gold}";
            };
            Reg["查金币"] = Reg["查金钱"];
            Reg["百度"] = (v, _g, _u) =>
            {
                if (string.IsNullOrEmpty(v))
                {
                    return null;
                }
                var user = GetUser(_u,_g);
                if (user.DbUser.Gold < 100)
                {
                    return $"{user.QQName}呀,你的钱,还不够让我帮你搜索这条信息呀!";
                }
                user.DbUser.Gold -= 100;
                string r = $"尊敬的{user.QQName},已扣除您100金，为你呈现结果：\r\n https://www.baidu.com/s?wd={UrlEncode(v)}";
                return r;
            };
            Reg["背包"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                if (user.DbUser.Items.Keys.Count() == 0)
                {
                    return $"{user.QQName}呀！你的背包里，什么东西都没有哦~";
                }
                string result = $"===={user.QQName}的背包====\n";

                int page = GetPage(v);
                int maxPage = user.DbUser.Items.Keys.Count() / 10;
                maxPage += user.DbUser.Items.Keys.Count() % 10 == 0 ? 0 : 1;
                if (page > maxPage)
                {
                    return $"{user.QQName}呀！你要查询的页数超过了范围哦~";
                }
                var items = user.DbUser.Items.Values.Skip((page - 1) * 10).Take(10);
                foreach (var item in items)
                {
                    result += item.Item.GetSingleName() + " x" + item.Count + "\n";
                }
                result += $"第{page}/{maxPage}页 (输入[#背包 页数]查看其他页)";
                return result;
            };
            Reg["查看"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                if (!user.DbUser.Items.ContainsKey(v))
                {
                    return $"{user.QQName}呀！你还没有这个东西哦~";
                }
                var item = user.DbUser.Items[v].Item;
                string result = item.Name + "\n";
                result += "星级:" + item.GetStar() + "\n";
                result += "类型:" + item.GetTypeCn() + "\n";
                result += "属性:" + item.Element + "\n";
                result += "等级:" + user.DbUser.Items[v].ItemLevel + "\n";
                result += "价值:" + item.Value + "\n";
                result += "说明:" + item.Content;
                return result;
            };
            Reg["卖出"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                if (!user.DbUser.Items.ContainsKey(v))
                {
                    return $"{user.QQName}呀！你还没有这个东西哦~";
                }
                int totalv = CalcSoldDiscount(user, user.DbUser.Items[v].Item.Value);

                user.DbUser.Gold += totalv;
                string result = $"{user.QQName}亲！系统已收购了您的物品 {user.DbUser.Items[v].Item.Name},获得{totalv}金!";
                if (user.DbUser.Items[v].Count == 1)
                {
                    user.DbUser.Items.Remove(v);
                }
                else
                {
                    user.DbUser.Items[v].Count--;
                }
                return result;
            };
            Reg["卖出全部"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                if (!user.DbUser.Items.ContainsKey(v))
                {
                    return $"{user.QQName}呀！你还没有这个东西哦~";
                }
                int sold = CalcSoldDiscount(user, user.DbUser.Items[v].Item.Value, user.DbUser.Items[v].Count);
                user.DbUser.Gold += sold;
                string result = $"{user.QQName}亲！系统已收购了您所有的 {user.DbUser.Items[v].Item.Name},获得{sold}金!";
                user.DbUser.Items.Remove(v);
                return result;
            };
            Reg["卖出全部3星"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                var keys = user.DbUser.Items.Keys.ToList();
                int sold = 0;
                foreach (var key in keys)
                {
                    var item = user.DbUser.Items[key];
                    if (Convert.ToInt32(item.Item.Level) > 3)
                    {
                        continue;
                    }
                    sold += item.Item.Value * item.Count;
                    user.DbUser.Items.Remove(key);
                }
                user.DbUser.Gold += sold;
                string result = $"{user.QQName}亲！系统已收购了您所有的 三星以及三星以内的物品,获得{sold}金!";
                return result;
            };
            Reg["卖出全部4星"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                var keys = user.DbUser.Items.Keys.ToList();
                int sold = 0;
                foreach (var key in keys)
                {
                    var item = user.DbUser.Items[key];
                    if (Convert.ToInt32(item.Item.Level) > 4)
                    {
                        continue;
                    }
                    sold += item.Item.Value * item.Count;
                    user.DbUser.Items.Remove(key);
                }
                user.DbUser.Gold += sold;
                string result = $"{user.QQName}亲！系统已收购了您所有的 四星以及四星以内的物品,获得{sold}金!";
                return result;
            };
            Reg["卖出全部三星"] = Reg["卖出全部3星"];
            Reg["卖出所有三星"] = Reg["卖出全部3星"];
            Reg["卖出所有3星"] = Reg["卖出全部3星"];
            Reg["卖出全部四星"] = Reg["卖出全部4星"];
            Reg["卖出所有四星"] = Reg["卖出全部4星"];
            Reg["卖出所有4星"] = Reg["卖出全部4星"];
            Reg["卖出全部4星武器"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                var keys = user.DbUser.Items.Keys.ToList();
                int sold = 0;
                foreach (var key in keys)
                {
                    var item = user.DbUser.Items[key];
                    if (Convert.ToInt32(item.Item.Level) > 4)
                    {
                        continue;
                    }
                    if (item.Item.Type != "weapon")
                    {
                        continue;
                    }
                    sold += item.Item.Value * item.Count;
                    user.DbUser.Items.Remove(key);
                }
                user.DbUser.Gold += sold;
                string result = $"{user.QQName}亲！系统已收购了您所有的 三星以及三星以内的物品,获得{sold}金!";
                return result;
            };
            Reg["卖出全部4星除了云堇"] = (v, _g, _u) => {
                var user = GetUser(_u,_g);
                var keys = user.DbUser.Items.Keys.ToList();
                int sold = 0;
                foreach (var key in keys)
                {
                    var item = user.DbUser.Items[key];
                    if (Convert.ToInt32(item.Item.Level) > 4)
                    {
                        continue;
                    }
                    if (item.Item.Name == "云堇")
                    {
                        continue;
                    }
                    sold += item.Item.Value * item.Count;
                    user.DbUser.Items.Remove(key);
                }
                user.DbUser.Gold += sold;
                string result = $"{user.QQName}亲！系统已收购了您所有的 四星以及四星以内的物品,获得{sold}金!";
                return result;
            };
            Reg["发红包"] = (v, _g, _u) => {
                v = v.Replace("  ", " ");
                if (string.IsNullOrEmpty(v))
                {
                    return "请输入 [#发红包 目标 金额]";
                }
                if (v.Split(' ').Length != 2)
                {
                    return "请输入 [#发红包 目标 金额]";
                }
                var user = GetUser(_u,_g);
                string target_name = v.Split(' ')[0];
                int target_value = 0;
                try
                {
                    target_value = Convert.ToInt32(v.Split(' ')[1]);
                }
                catch
                {
                    return $"{user.QQName}呀！请输入一个有效的整数";
                }
                if (target_value < 0)
                {
                    return $"{user.QQName}呀！请输入一个有效的正整数";
                }
                if (user.DbUser.Gold < target_value)
                {
                    return $"{user.QQName}呀！你的钱,还不够发红包啊~";
                }
                if (!Util.MatchQQ(target_name))
                {
                    return "请输入 [#发红包 目标 金额]";
                }
                //var target_client = client.FromGroup.GetMemberByQQ(Util.GetQQFromAt(target_name));
                //if (target_client == null)
                //{
                //    return "没有找到目标!";
                //}
                //var target_user = GetUser(target_client, client.FromGroup);
                var target_user = GetUser(Util.GetQQFromAt(target_name), _g);
                user.DbUser.Gold -= target_value;

                var fee = Convert.ToInt32(Convert.ToDouble(target_value) * 0.1);
                target_user.DbUser.Gold += (target_value - fee);
                return $"{user.QQName} 给 {target_user.QQName} 发了一个{target_value}金的大红包！老板大气！(手续费：{fee}金)";
            };

            Reg["金手指"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                if (_g.OwnerId == user.QQId || user.QQId == this.config.GetValue<string>("adminQQ"))
                {
                    user.DbUser.Gold += 10000;
                    return $"[群主特权代码]已增加 {user.QQName} 10000金币";
                }
                else
                {
                    return "嗯？小伙子你想干什么?";
                }
            };
            Reg["罚钱"] = (v, _g, _u) =>
            {
                var user = GetUser(_u, _g);
                if (_g.OwnerId == user.QQId || user.QQId == this.config.GetValue<string>("adminQQ"))
                {
                    if (string.IsNullOrEmpty(v))
                    {
                        return "请输入 [#罚钱 目标 金额]";
                    }
                    if (v.Split(' ').Length != 2)
                    {
                        return "请输入 [#罚钱 目标 金额]";
                    }
                    string target_name = v.Split(' ')[0];
                    int target_value = 0;
                    try
                    {
                        target_value = Convert.ToInt32(v.Split(' ')[1]);
                    }
                    catch
                    {
                        return $"{user.QQName}呀！请输入一个有效的整数";
                    }

                    var target_user = GetUser(Util.GetQQFromAt(target_name), _g);
                    if (target_user == null)
                    {
                        return "没有找到目标!";
                    }

                    target_user.DbUser.Gold -= target_value;
                    if (target_user.DbUser.Gold < 0)
                    {
                        target_user.DbUser.Gold = 0;
                    }
                    return $"[群主特权代码]已没收 {target_user.QQName} {target_value}金！";
                }
                else
                {
                    return "嗯？小伙子你想干什么?";
                }
            };
            Reg["同步数据库"] = (v, _g, _u) =>
            {
                var user = GetUser(_u,_g);
                if (user.QQId == this.config.GetValue<string>("adminQQ"))
                {
                    this.sync_db();
                    return "同步数据库完成";
                }
                return "";
            };
        }
        public EcUser GetUser(UserClient client, GroupClient group)
        {
            string id = group.Id + "_" + client.Id;
            if (this.Users.ContainsKey(id))
            {
                return this.Users[id];
            }

            EcUser user = new EcUser()
            {
                //QQ = client
                QQId = client.Id,
                //QQName=client.GetNick(group.RobotQQ),
                QQName = $"[@{client.Id}]",
                Group = group
            };
            var dbItem = this.GetFromDb(id);
            if (dbItem != null)
            {
                //user.DbUser.Gold = dbItem.Gold;
                //user.DbUser.Items = dbItem.Items;
                user.DbUser = dbItem;
            }
            else
            {
                user.DbUser = new DB_EcUser()
                {
                    Id = id,
                    QQId = user.QQId,
                    QQName = user.QQName
                };
            }
            this.Users[id] = user;
            return user;
        }
        public EcUser GetUser(string qqid, GroupClient group)
        {
            string id = group.Id + "_" + qqid;
            if (this.Users.ContainsKey(id))
            {
                return this.Users[id];
            }

            EcUser user = new EcUser()
            {
                //QQ = client
                QQId = qqid,
                QQName = $"[@{qqid}]",
                Group = group
            };
            var dbItem = this.GetFromDb(id);
            if (dbItem != null)
            {
                //user.DbUser.Gold = dbItem.Gold;
                //user.DbUser.Items = dbItem.Items;
                user.DbUser = dbItem;
            }
            else
            {
                user.DbUser = new DB_EcUser()
                {
                    Id = id,
                    QQId = user.QQId,
                    QQName = user.QQName
                };
            }
            this.Users[id] = user;
            return user;
        }
        public int CalcSoldDiscount(EcUser user, int totalV, int count = 1)
        {
            if (totalV < 10000)
            {
                return totalV * count;
            }
            user.SoldCount += count;
            if (user.SoldCount > 50)
            {
                return Convert.ToInt32(totalV * 0.4) * count;
            }
            else if (user.SoldCount > 30)
            {
                return Convert.ToInt32(totalV * 0.5) * count;
            }
            else if (user.SoldCount > 20)
            {
                return Convert.ToInt32(totalV * 0.7) * count;
            }
            else if (user.SoldCount > 10)
            {
                return Convert.ToInt32(totalV * 0.8) * count;
            }
            else
            {
                return totalV * count;
            }
        }
        public void sync_db()
        {
            using (var db = new LiteDatabase(@"./hyper_bot.db"))
            {
                var col = db.GetCollection<DB_EcUser>("ecommerce");
                foreach (var id in this.Users.Keys)
                {
                    //DB_EcUser db_item = new DB_EcUser()
                    //{
                    //    Id = id,
                    //    Gold = this.Users[id].DbUser.Gold,
                    //    QQId = this.Users[id].QQId,
                    //    QQName = this.Users[id].QQName,
                    //    Items = this.Users[id].DbUser.Items
                    //};
                    col.Upsert(this.Users[id].DbUser);
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
                if (string.IsNullOrEmpty(v))
                {
                    return 1;
                }
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
