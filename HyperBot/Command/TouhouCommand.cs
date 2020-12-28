using HyperBot.Core;
using HyperBot.Model;
using HyperBot.Utils;
using RolePlayChat.Core.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperBot.Command
{
    public class TouhouUser
    {
        public int UpCount { get; set; } = 0;
        public int WeaponCount { get; set; } = 0;
        public int CZCount { get; set; } = 0;
        public EcUser EcUser { get; set; }
    }
    public class TouhouCommand : BaseCommand
    {
        Dictionary<string, TouhouUser> users = new Dictionary<string, TouhouUser>();
        public TouhouCommand()
        {
            Reg["幻想乡抽卡"] = (v, content) =>
            {
                return "幻想乡抽卡系统v0.00001\n" +
                "单次抽卡只需60金币! 10连仅需550金币\n" +
                "[可选指令]\n" +
                "#幻想乡单抽 卡池名称\n" +
                "#幻想乡十连 卡池名称\n" +
                "[可选卡池]\n" +
                "常驻,活动\n" +
                "[最新资讯]\n" +
                "TBK&RMR角色活动池UP中!";
            };
            Reg["幻想乡单抽"] = (v, content) =>
            {
                EcUser user = CommandSystem.GetModule(typeof(EcommerceCommand)).GetPlugInter("get").Invoke(content);
                TouhouUser genshinuser = GetUser(user, content.FromGroup);
                if (user.Gold < 60)
                {
                    return $"{user.QQName}呀,你的钱,还不够呀!";
                }

                int pool = GetPool(v);
                if (pool < 0)
                {
                    return $"{user.QQName}呀,没有这个卡池，可选卡池为[常驻,角色,武器]!";
                }

                user.Gold -= 60;
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
                return result;
            };
            Reg["幻想乡十连"] = (v, content) =>
            {
                EcUser user = CommandSystem.GetModule(typeof(EcommerceCommand)).GetPlugInter("get").Invoke(content);
                TouhouUser genshinuser = GetUser(user, content.FromGroup);
                if (user.Gold < 550)
                {
                    return $"{user.QQName}呀,你的钱,还不够呀!";
                }

                int pool = GetPool(v);
                if (pool < 0)
                {
                    return $"{user.QQName}呀,没有这个卡池，可选卡池为[常驻,角色,武器]!";
                }

                user.Gold -= 550;
                var items = Gacha10(pool, genshinuser);
                string result = $"{user.QQName} 本次抽到的10连如下\n";
                bool has4char = false;
                bool has5 = false;
                foreach (var item in items)
                {
                    result += item.GetFullName() + "\n";
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
                return result;
            };
        }
        TouhouUser GetUser(EcUser user, GroupClient group)
        {
            string id = group.Group.Id + "_" + user.QQId;
            if (users.ContainsKey(id))
            {
                return users[id];
            }

            TouhouUser _user = new TouhouUser();
            _user.EcUser = user;
            users.Add(id, _user);
            return users[id];
        }
        IGachaItem Gacha(int pool, TouhouUser user)
        {
            IGachaItem[] result = null;
            if (pool == 1)
            {
                int count = user.CZCount;
                result = TouhouGacha.Get(pool, ref count);
                user.CZCount = count;
            }
            else if (pool == 2)
            {
                int count = user.UpCount;
                result = TouhouGacha.Get(pool, ref count);
                user.UpCount = count;
            }
            else if (pool == 3)
            {
                int count = user.WeaponCount;
                result = TouhouGacha.Get(pool, ref count);
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
        IGachaItem[] Gacha10(int pool, TouhouUser user)
        {
            IGachaItem[] result = null;
            if (pool == 1)
            {
                int count = user.CZCount;
                result = TouhouGacha.Get(pool, ref count);
                user.CZCount = count;
            }
            else if (pool == 2)
            {
                int count = user.UpCount;
                result = TouhouGacha.Get(pool, ref count);
                user.UpCount = count;
            }
            else if (pool == 3)
            {
                int count = user.WeaponCount;
                result = TouhouGacha.Get(pool, ref count);
                user.WeaponCount = count;
            }
            foreach(var item in result)
            {
                if (user.EcUser.Items.ContainsKey(item.Name))
                {
                    user.EcUser.Items[item.Name].Count++;
                }
                else
                {
                    user.EcUser.Items.Add(item.Name, new UserItem()
                    {
                        Item = item,
                        Count = 1
                    });
                }
            }
            return result;
        }
        int GetPool(string v)
        {
            if (v == "常驻")
            {
                return 1;
            }
            else if (v == "活动")
            {
                return 2;
            }
            else
            {
                return -1;
            }
        }
    }
}
