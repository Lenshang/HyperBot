using HyperBot.Command;
using HyperBot.Core;
using HyperBot.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

namespace HyperBot.Extension.Genshin
{
    public class GenshinUser
    {
        public int UpCount { get; set; } = 0;
        public int WeaponCount { get; set; } = 0;
        public int CZCount { get; set; } = 0;
        public EcUser EcUser { get; set; }
        public DateTime LastGacha { get; set; } = new DateTime(2000, 1, 1);
    }
    public class GenshinCommand : BaseCommand
    {
        Dictionary<string, GenshinUser> users = new Dictionary<string, GenshinUser>();
        IConfiguration config { get; set; }
        public GenshinCommand(IConfiguration configuration)
        {
            this.config = configuration;
        }
        public override void RegCmd()
        {
            Reg["原神抽卡"] = (v, group, user) =>
            {
                return "原神抽卡系统\n" +
                "单次抽卡只需160金币! 10连仅需1600金币\n" +
                "[可选指令]\n" +
                "#原神单抽 卡池名称\n" +
                "#原神十连 卡池名称\n" +
                "#原神50连 卡池名称\n" +
                "#原神90连 卡池名称\n" +
                "[可选卡池]\n" +
                "常驻,角色,武器\n" +
                "[最新资讯]\n" +
                "金币不够的可以先输入 #打很长时间的工 获得金币哟!";
            };
            Reg["原神单抽"] = (v, _g, _u) =>
            {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u, _g);
                GenshinUser genshinuser = GetUser(user, _g);
                if (user.Gold < 160)
                {
                    return $"{user.QQName}呀,你的钱,还不够呀!";
                }

                if ((DateTime.Now - genshinuser.LastGacha) <= TimeSpan.FromMinutes(10))
                {
                    var ts = 10 - Convert.ToInt32((DateTime.Now - genshinuser.LastGacha).TotalMinutes);
                    return $"{user.QQName}呀,每10分钟才能抽一次哦! 你还需要等待{ts}分钟!";
                }
                int pool = GetPool(v);
                if (pool < 0)
                {
                    return $"{user.QQName}呀,没有这个卡池，可选卡池为[常驻,角色,武器]!";
                }

                user.Gold -= 160;
                var item = Gacha(pool, genshinuser);
                string result = $"{user.QQName} 本次抽到的结果如下\n";
                result += item.GetFullName() + "\n";
                if (item.Level == "5")
                {
                    result += "哇！单抽出奇迹啊!";
                }
                else if (item.Level == "4" && item.Type == "character")
                {
                    result += "单抽出4星角色血赚!";
                }
                else
                {
                    result += "很遗憾..什么都没出,不过单抽也是正常的嘛...不试试10连么？";
                }

                genshinuser.LastGacha = DateTime.Now;
                return result;
            };
            Reg["原神十连"] = (v, _g, _u) =>
            {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u, _g);
                GenshinUser genshinuser = GetUser(user, _g);
                if (user.Gold < 1600)
                {
                    return $"{user.QQName}呀,你的钱,还不够呀!";
                }
                if ((DateTime.Now - genshinuser.LastGacha) <= TimeSpan.FromMinutes(10))
                {
                    var ts = 10 - Convert.ToInt32((DateTime.Now - genshinuser.LastGacha).TotalMinutes);
                    return $"{user.QQName}呀,每10分钟才能抽一次哦! 你还需要等待{ts}分钟!";
                }
                int pool = GetPool(v);
                if (pool < 0)
                {
                    return $"{user.QQName}呀,没有这个卡池，可选卡池为[常驻,角色,武器]!";
                }

                user.Gold -= 1600;
                var items = Gacha10(pool, genshinuser);
                string result = $"{user.QQName} 本次抽到的10连如下\n";
                bool has4char = false;
                bool has5 = false;
                foreach (var item in items)
                {
                    result += item.GetFullName();
                    if (item.IsNew)
                    {
                        result += "(new)";
                    }
                    result += "\n";
                    if (item.Level == "4" && item.Type == "character")
                    {
                        has4char = true;
                    }
                    else if (item.Level == "5")
                    {
                        has5 = true;
                    }
                }

                if (has5)
                {
                    result += "哇！你的运气不错哦~";
                }
                else if (has4char)
                {
                    result += "嘛~ 出了4星角色也不亏啦";
                }
                else
                {
                    result += "很遗憾..什么都没出";
                }
                genshinuser.LastGacha = DateTime.Now;
                return result;
            };
            Reg["原神10连"] = Reg["原神十连"];
        }
        IGachaItem Gacha(int pool, GenshinUser user)
        {
            IGachaItem[] result = null;
            if (pool == 1)
            {
                int count = user.CZCount;
                result = GenshinGacha.Get(pool, ref count);
                user.CZCount = count;
            }
            else if (pool == 2)
            {
                int count = user.UpCount;
                result = GenshinGacha.Get(pool, ref count);
                user.UpCount = count;
            }
            else if (pool == 3)
            {
                int count = user.WeaponCount;
                result = GenshinGacha.Get(pool, ref count);
                user.WeaponCount = count;
            }
            if (user.EcUser.Items.ContainsKey(result[0].Name))
            {
                user.EcUser.Items[result[0].Name].Count++;
            }
            else
            {
                user.EcUser.Items.Add(result[0].Name, new UserItem()
                {
                    Item = result[0],
                    Count = 1
                });
            }
            return result[0];
        }
        IGachaItem[] Gacha10(int pool, GenshinUser user)
        {
            IGachaItem[] result = null;
            if (pool == 1)
            {
                int count = user.CZCount;
                result = GenshinGacha.Get(pool, ref count);
                user.CZCount = count;
            }
            else if (pool == 2)
            {
                int count = user.UpCount;
                result = GenshinGacha.Get(pool, ref count);
                user.UpCount = count;
            }
            else if (pool == 3)
            {
                int count = user.WeaponCount;
                result = GenshinGacha.Get(pool, ref count);
                user.WeaponCount = count;
            }
            foreach (var item in result)
            {
                if (user.EcUser.Items.ContainsKey(item.Name))
                {
                    user.EcUser.Items[item.Name].Count++;
                    item.IsNew = false;
                }
                else
                {
                    user.EcUser.Items.Add(item.Name, new UserItem()
                    {
                        Item = item,
                        Count = 1
                    });
                    item.IsNew = true;

                }
            }
            return result;
        }
        GenshinUser GetUser(EcUser user, GroupClient group)
        {
            string id = group.Id + "_" + user.QQId;
            if (users.ContainsKey(id))
            {
                return users[id];
            }

            GenshinUser _user = new GenshinUser();
            _user.EcUser = user;
            users.Add(id, _user);
            return users[id];
        }
        int GetPool(string v)
        {
            if (v == "常驻")
            {
                return 1;
            }
            else if (v == "角色")
            {
                return 2;
            }
            else if (v == "武器")
            {
                return 3;
            }
            else
            {
                return -1;
            }
        }
    }
}
