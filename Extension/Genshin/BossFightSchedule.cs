using HyperBot.Command;
using HyperBot.Core;
using HyperBot.Model;
using HyperBot.Schedule;
using HyperBot.Utils;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace HyperBot.Extension.Genshin
{
    public class BossModel
    {
        public string Name { get; set; }
        public int Hp { get; set; }
        public int AliveHour { get; set; }
        public int Value { get; set; }
        public string Element { get; set; }
        public DateTime CreateTime { get; set; } = DateTime.Now;
        public Dictionary<string, int> Dps = new Dictionary<string, int>();

        public BossModel GetCopy()
        {
            return new BossModel()
            {
                AliveHour = this.AliveHour,
                Name = this.Name,
                Hp = this.Hp,
                CreateTime = DateTime.Now,
                Dps = new Dictionary<string, int>(),
                Element = this.Element,
                Value = this.Value
            };
        }
    }
    public class BossGroup
    {
        public BossModel CurrentBoss { get; set; }
        public int MinHour { get; set; } = 1;
        public int MaxHour { get; set; } = 4;

    }
    public class BossUser
    {
        public EcUser EcUser { get; set; }
        public DateTime LastAttack { get; set; } = new DateTime(1970, 1, 1);
        public string Preference { get; set; } = "";
        public string EquipWeapon { get; set; } = "";
        public string EquipWeaponType { get; set; } = "";
        public string EquipWeaponLevel { get; set; } = "";
        public int Hp { get; set; } = 1000;
        public int Exp { get; set; } = 0;
        public DateTime LastAttackDate { get; set; } = new DateTime(1990, 1, 1);
    }
    public class BossFightCommand : BaseCommand
    {
        Dictionary<string, BossUser> users = new Dictionary<string, BossUser>();
        IConfiguration config { get; set; }
        public BossFightCommand(IConfiguration configuration)
        {
            this.config = configuration;
        }

        public override void RegCmd()
        {
            Reg["世界BOSS"] = (v, _g, _u) =>
            {
                if (_g.OwnerId != _u.Id && _u.Id != this.config.GetValue<string>("adminQQ"))
                {
                    return null;
                }
                if (v != "开启" && v != "关闭")
                {
                    return "请输入 [#世界BOSS 开启/关闭] 来打开或者关闭世界BOSS系统。";
                }
                var schedule = ScheduleSystem.GetSchedule<BossFightSchedule>();
                if (v == "开启")
                {
                    //schedule.Enable = true;
                    schedule.AddClient(_g);
                    return "世界BOSS系统开启成功";
                }
                else
                {
                    //schedule.Enable = false;
                    schedule.RemoveClient(_g);
                    return "世界BOSS系统已关闭";
                }
            };

            Reg["攻击boss"] = (v, _g, _u) =>
            {
                var schedule = ScheduleSystem.GetSchedule<BossFightSchedule>();
                if (!schedule.CurrentBoss.ContainsKey(_g.Id))
                {
                    return "当前没有BOSS存在哦~ 可能是没有打开此功能或是请等待BOSS刷新！";
                }
                BossModel boss = schedule.CurrentBoss[_g.Id];
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u,_g);

                BossUser bossuser = GetUser(user, _g);

                if ((DateTime.Now - bossuser.LastAttack) < TimeSpan.FromSeconds(60))
                {
                    return $"尊敬的{user.QQId} 每分钟只能攻击一次哦~";
                }
                if (bossuser.Hp <= 0)
                {
                    return $"尊敬的{user.QQId} 您目前正在濒危状态，无法攻击!";
                }

                if (boss.Hp <= 0)
                {
                    return $"尊敬的{user.QQId} BOSS已经被消灭啦~等待下一波刷新吧!";
                }
                var _cards = bossuser.EcUser.Items.Values.Where(i => i.Item.Type == "character").Select(i => i.Item.Name).ToList();
                var cards = GenshinGacha.GetItemFromNames(_cards);
                IGachaItem card;
                int card_count = 1;
                int level = 1;
                if (cards.Count > 0)
                {
                    IGachaItem prefer_card = null;
                    if (!string.IsNullOrEmpty(bossuser.Preference))
                    {
                        prefer_card = cards.Where(i => i.Name.Contains(bossuser.Preference)).FirstOrDefault();
                    }
                    if (prefer_card != null && GenshinGacha.GetRandom(0, 100) <= 60)
                    {
                        card = prefer_card;
                    }
                    else
                    {
                        card = cards[GenshinGacha.GetRandom(0, cards.Count())];
                    }
                    card_count = bossuser.EcUser.Items[card.Name].Count;
                    level = bossuser.EcUser.Items[card.Name].ItemLevel;
                    if (card_count > 7)
                    {
                        card_count = 7;
                    }
                }
                else
                {
                    card = new IGachaItem()
                    {
                        Name = "他自己",
                        Level = "4",
                        Element = "啥都不会",
                    };
                }

                //计算伤害
                int base_damage = GenshinGacha.GetRandom(500 + (level * 10), 2000);
                int extra_damage = 0;
                bool boss_attack = true;
                if (card.Element == "岩" && boss.Element == "岩")
                {
                    extra_damage = Convert.ToInt32(base_damage * 0.5);
                }
                else if (card.Element == "风" && boss.Element == "风")
                {
                    base_damage = 0;
                }
                else if (card.Element == boss.Element)
                {
                    base_damage = Convert.ToInt32(base_damage * 0.2);
                }
                else if (card.Element == "雷" && boss.Element == "火")
                {
                    extra_damage = Convert.ToInt32(base_damage * 0.5);
                }
                else if (card.Element == "雷" && boss.Element == "水")
                {
                    extra_damage = Convert.ToInt32(base_damage * 0.5);
                }
                else if (card.Element == "雷" && boss.Element == "冰")
                {
                    extra_damage = Convert.ToInt32(base_damage * 0.3);
                }
                else if (card.Element == "火" && boss.Element == "水")
                {
                    extra_damage = Convert.ToInt32(base_damage * 0.5);
                }
                else if (card.Element == "水" && boss.Element == "火")
                {
                    extra_damage = Convert.ToInt32(base_damage * 1);
                }
                else if (card.Element == "冰" && boss.Element == "火")
                {
                    extra_damage = Convert.ToInt32(base_damage * 0.5);
                }
                else if (card.Element == "火" && boss.Element == "冰")
                {
                    extra_damage = Convert.ToInt32(base_damage * 1);
                }
                else if (card.Element == "冰" && boss.Element == "水")
                {
                    boss_attack = false;
                }

                extra_damage += Convert.ToInt32(base_damage * (card_count / 7));

                if (card.Level == "5")
                {
                    extra_damage += Convert.ToInt32(base_damage * 0.5);
                }
                base_damage += extra_damage;
                //伤害结算
                if (level > 1)
                {
                    base_damage += base_damage * ((level) / 100);
                }
                bool useWeapon = false;
                if (!string.IsNullOrEmpty(bossuser.EquipWeapon) && bossuser.EquipWeaponType == card.WeaponType)
                {
                    useWeapon = true;
                    if (bossuser.EquipWeaponLevel == "5")
                    {
                        base_damage += Convert.ToInt32(base_damage * 0.5);
                    }
                    else if (bossuser.EquipWeaponLevel == "4")
                    {
                        base_damage += Convert.ToInt32(base_damage * 0.25);
                    }
                    else
                    {
                        base_damage += Convert.ToInt32(base_damage * 0.1);
                    }
                }
                if (boss.Hp < base_damage)
                {
                    base_damage = boss.Hp;
                }


                //MISS计算
                bool isMiss = false;
                double missPer = 15.0 - 30.0 * (Convert.ToDouble(level) / 100.0);
                isMiss = GenshinGacha.GetRandom(1, 100) < missPer;
                if (isMiss)
                {
                    base_damage = 0;
                }

                boss.Hp -= base_damage;
                if (!boss.Dps.ContainsKey(user.QQId))
                {
                    boss.Dps.Add(user.QQId, 0);
                }
                boss.Dps[user.QQId] += base_damage;

                string r = "";
                if (boss.Hp <= 0)
                {
                    if (useWeapon)
                    {
                        r += $"[@{user.QQId}] 的伙伴 {card_count - 1}命的{card.Name}({card.Element}Lv:{level}) 使用 {bossuser.EquipWeapon} 对BOSS {boss.Name} 造成了 {base_damage} 伤害\n";
                    }
                    else
                    {
                        r += $"[@{user.QQId}] 的伙伴 {card_count - 1}命的{card.Name}({card.Element}Lv:{level}) 对BOSS {boss.Name} 造成了 {base_damage} 伤害\n";
                    }
                    r += "成功击破了BOSS！\n";
                    r += "本次BOSS战奖励如下:\n";
                    //奖励
                    int kill_v = Convert.ToInt32((boss.Value / 10) * 2);
                    int dps1 = Convert.ToInt32((boss.Value / 10) * 2);
                    user.Gold += kill_v;
                    r += $"[击杀奖励][@{user.QQId}] {kill_v}金币\n";

                    string dps1QQ = boss.Dps.OrderByDescending(i => i.Value).First().Key;
                    GetUser(dps1QQ, _g).EcUser.Gold += dps1;
                    r += $"[DPS奖励][@{dps1QQ}] {dps1}金币\n";

                    int g = Convert.ToInt32((boss.Value / 10) * 6) / boss.Dps.Count;
                    foreach (var key in boss.Dps.Keys)
                    {
                        GetUser(key, _g).EcUser.Gold += g;
                        r += $"[@{key}] {g}金币\n";
                    }
                }
                else
                {
                    if (isMiss)
                    {
                        r += $"[@{user.QQId}] 的伙伴 {card_count - 1}命的{card.Name}({card.Element}Lv:{level}) 的攻击被BOSS躲开了！没有造成任何伤害！\n";
                    }
                    else if (useWeapon)
                    {
                        r += $"[@{user.QQId}] 的伙伴 {card_count - 1}命的{card.Name}({card.Element}Lv:{level}) 使用 {bossuser.EquipWeapon} 对BOSS {boss.Name} 造成了 {base_damage} 伤害\n";
                        r += $"BOSS剩余血量:{boss.Hp}\n";
                    }
                    else
                    {
                        r += $"[@{user.QQId}] 的伙伴 {card_count - 1}命的{card.Name}({card.Element}Lv:{level}) 对BOSS {boss.Name} 造成了 {base_damage} 伤害\n";
                        r += $"BOSS剩余血量:{boss.Hp}\n";
                    }

                }

                if (boss_attack && !isMiss)
                {
                    int dmg = GenshinGacha.GetRandom(1, 500);
                    r += "因为同时受到BOSS的反击\n";
                    r += $"{user.QQName} 受到了 {dmg} 点伤害\n";
                    bossuser.Hp -= dmg;
                    if (bossuser.Hp <= 0)
                    {
                        r += $"{user.QQName}已进入濒危状态，请输入#买活 复活，费用5000G";
                    }
                }
                bossuser.LastAttack = DateTime.Now;
                return r;
            };

            Reg["讨伐"] = Reg["攻击boss"];

            Reg["刷新BOSS"] = (v, _g, _u) => {
                if (_g.OwnerId != _u.Id && _u.Id != this.config.GetValue<string>("adminQQ"))
                {
                    return null;
                }
                var schedule = ScheduleSystem.GetSchedule<BossFightSchedule>();
                schedule.CreateBoss(_g);
                return "";
            };

            Reg["偏好角色"] = (v, _g, _u) =>
            {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u,_g);
                BossUser bossuser = GetUser(user,_g);
                if (string.IsNullOrEmpty(v))
                {
                    return "输入 [#偏好角色 角色名] 则该角色在BOSS战中出场的几率更大哟~ 每次收费600G";
                }
                if (user.Gold < 600)
                {
                    return $"{user.QQName} 呀 你的钱还不够呀！";
                }
                user.Gold -= 600;
                bossuser.Preference = v;
                return $"{user.QQName} 设置成功！可以输入[#清除偏好角色]来清除";
            };

            Reg["清除偏好角色"] = (v, _g, _u) =>
            {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u, _g);
                BossUser bossuser = GetUser(user, _g);
                bossuser.Preference = "";
                return $"{user.QQName} 清除成功！";
            };

            Reg["装备武器"] = (v, _g, _u) => {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u, _g);
                BossUser bossuser = GetUser(user, _g);
                if (string.IsNullOrEmpty(v))
                {
                    return "输入 [#装备武器 武器名称] 装备武器~ 每次收费600G";
                }
                if (user.Gold < 600)
                {
                    return $"{user.QQName} 呀 你的钱还不够呀！";
                }
                user.Gold -= 600;
                var weapon = user.Items.Values.Where(i => i.Item.Type == "weapon").Where(i => i.Item.Name == v).FirstOrDefault();
                if (weapon == null)
                {
                    return $"{user.QQName} 呀 你没有这个武器哦~";
                }
                var dbWeapon = GenshinGacha.GetItemFromName(weapon.Item.Name);
                bossuser.EquipWeapon = dbWeapon.Name;
                bossuser.EquipWeaponType = dbWeapon.WeaponType;
                bossuser.EquipWeaponLevel = dbWeapon.Level;
                return $"{user.QQName} 设置成功！可以输入[#清除装备武器]来清除";
            };

            Reg["清除装备武器"] = (v, _g, _u) =>
            {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u, _g);
                BossUser bossuser = GetUser(user, _g);
                bossuser.EquipWeapon = "";
                bossuser.EquipWeaponType = "";
                return $"{user.QQName} 清除成功！";
            };

            Reg["状态"] = (v, _g, _u) =>
            {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u, _g);
                BossUser bossuser = GetUser(user, _g);
                return $"尊敬的{bossuser.EcUser.QQName} 您目前\n" +
                $"HP：{bossuser.Hp},\n" +
                $"金币：{bossuser.EcUser.Gold},\n" +
                $"经验：{bossuser.Exp},\n" +
                $"当前等级：{this.LevelCalc(bossuser)} \n" +
                $"装备武器：{bossuser.EquipWeapon}({bossuser.EquipWeaponType}) \n" +
                $"BOSS战偏好角色:{bossuser.Preference}";
            };

            Reg["升级"] = (v, _g, _u) =>
            {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u, _g);
                BossUser bossuser = GetUser(user, _g);
                if (string.IsNullOrEmpty(v))
                {
                    return "输入 [#升级 角色名] 给该角色提升Level！每次升级收取 [当前等级*1000] 金币的费用";
                }

                var character = user.Items.Values.Where(i => i.Item.Type == "character").Where(i => i.Item.Name == v).FirstOrDefault();
                if (character == null)
                {
                    return $"{user.QQName} 呀 这个角色还没有成为你的伙伴哦！";
                }
                var cost = character.ItemLevel * 1000;
                if (Convert.ToInt32(character.Item.Level) < 5)
                {
                    cost = Convert.ToInt32(cost * 0.7);
                }
                if (user.Gold < cost)
                {
                    return $"{user.QQName} 呀 你的钱还不够呀！";
                }

                user.Gold -= cost;
                character.ItemLevel += 1;
                return $"恭喜 {user.QQName} 的伙伴 {character.Item.Name} 成功升到了 {character.ItemLevel} 级！(已扣除{cost}金,剩余{user.Gold}金)";
            };
            Reg["确认买活"] = (v, _g, _u) => {
                EcUser user = (CommandSystem.GetModule(typeof(EcommerceCommand)) as EcommerceCommand).GetUser(_u, _g);
                BossUser source = GetUser(user, _g);
                if (source.Hp > 0)
                {
                    return $"尊敬的{user.QQName} 您目前很健康!无需买活!";
                }
                if (source.EcUser.Gold < 5000)
                {
                    return $"尊敬的{user.QQName} 所需金币不够!无法买活! 您可以输入/打工 来赚取金币";
                }

                source.EcUser.Gold -= 5000;
                source.Hp = 1000;
                return $"尊敬的{user.QQName} 买活成功!您已经恢复健康啦!";
            };
            Reg["买活"] = Reg["确认买活"];
        }

        BossUser GetUser(EcUser user, GroupClient group)
        {
            string id = group.Id + "_" + user.QQId;
            if (users.ContainsKey(id))
            {
                return users[id];
            }

            BossUser _user = new BossUser()
            {
                EcUser = user
            };
            users.Add(id, _user);
            return users[id];
        }
        BossUser GetUser(string qqid, GroupClient group)
        {
            string id = group.Id + "_" + qqid;
            if (users.ContainsKey(id))
            {
                return users[id];
            }
            return null;
        }
        public int LevelCalc(BossUser user)
        {
            return Convert.ToInt32(Math.Floor(user.Exp / 100f));
        }
    }
    public class BossFightSchedule : BaseSchedule
    {
        List<BossModel> BossList { get; set; }
        List<BossModel> KuaBossList { get; set; }
        public Dictionary<string, BossModel> CurrentBoss { get; set; } = null;
        public BossFightSchedule()
        {
            this.Load();
            this.Enable = true;
            this.ScheduleName = "BOSS战";
            this.CurrentBoss = new Dictionary<string, BossModel>();
        }
        public void Load()
        {
            string dataStr = File.ReadAllText("./Extension/Genshin/data/boss_data.json");
            var jtoken = JsonConvert.DeserializeObject<JToken>(dataStr);
            this.BossList = jtoken["通常BOSS"].ToObject<List<BossModel>>();
            this.KuaBossList = jtoken["跨服BOSS"].ToObject<List<BossModel>>();
        }
        public override void Do(GroupClient group, UserClient user)
        {
            foreach (var client in this.clients)
            {
                if (!CurrentBoss.ContainsKey(client.Id))
                {
                    CurrentBoss.Add(client.Id, null);
                }

                if (CurrentBoss[client.Id] == null || (DateTime.Now - CurrentBoss[client.Id].CreateTime) >= TimeSpan.FromHours(6))
                {
                    this.CreateBoss((client as GroupClient));
                }
            }
        }

        public void CreateBoss(GroupClient client)
        {
            CurrentBoss[client.Id] = BossList[GenshinGacha.GetRandom(0, BossList.Count)].GetCopy();
            CurrentBoss[client.Id].CreateTime = DateTime.Now;
            List<string> msg = new List<string>();
            msg.Add($"[BOSS战]新的BOSS刷新啦!");
            msg.Add($"名称：{CurrentBoss[client.Id].Name}");
            msg.Add($"血量：{CurrentBoss[client.Id].Hp}");
            msg.Add($"奖励掉落：{CurrentBoss[client.Id].Value}");
            msg.Add("快输入#讨伐 进行讨伐吧~");
            msg.Add("(可以输入 [#偏好角色 角色名] 使指定角色出场几率更高哦~)");
            try
            {
                client.SendMessage(string.Join("\n", msg));
            }
            catch
            {

            }
        }

        public override DateTime GetNextRunTime(DateTime currents)
        {
            return currents.AddMinutes(1);//这里选择使用每分钟执行一次，从主代码中判断生成间隔
        }
    }
}
