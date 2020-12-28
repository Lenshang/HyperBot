using HyperBot.Core;
using HyperBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XQ.Net.SDK.Models;

namespace RolePlayChat.Core.Command
{
    public class DB_BattleUser
    {
        public string Id { get; set; }
        public int Hp { get; set; } = 500;
        public int Exp { get; set; } = 0;
        //public XQQQ QQ { get; set; }
        public string QQId { get; set; }
        public string QQName { get; set; }
        public List<string> Bag { get; set; }
    }
    public class BattleUser
    {
        public int Hp { get; set; } = 500;
        public int Exp { get; set; } = 0;
        //public int Gold { get; set; } = 1000;
        public EcUser EcUser { get; set; }
        public DateTime LastAttackDate { get; set; } = new DateTime(1990, 1, 1);
        //public WSModel client { get; set; }
        //public XQQQ QQ { get; set; }
        public string QQId { get; set; }
        public string QQName { get; set; }
        public GroupClient Group { get; set; }
    }
    public class BattleResult
    {
        public int SourceDamage { get; set; } = 0;
        public int TargetDamage { get; set; } = 0;
        public int SourceGoldAdd { get; set; } = 0;
        public int TargetGoldAdd { get; set; } = 0;
    }
    public class BattleCommand:BaseCommand
    {
        Dictionary<string, BattleUser> Users = new Dictionary<string, BattleUser>();
        public BattleCommand()
        {
            Reg["攻击"] = (v, client) =>
            {
                var source = GetUser(client);
                string sName = source.QQName;
                if (sName == v)
                {
                    this.SendMessage(source.Group, $"{sName}呀！你不能攻击自己!","Battle System");
                    return null;
                }
                var target_client = client.FromGroup.GetMemberByName(v);
                if (target_client == null)
                {
                    this.SendMessage(source.Group, $"亲爱的{sName} 您要攻击的用户不存在!", "Battle System");
                    return null;
                }
                var target = GetUserByGroupMember(target_client,client);

                //ATTACK
                if (source.Hp<=0)
                {
                    this.SendMessage(source.Group, $"尊敬的{sName} 您目前正在濒危状态，无法攻击!", "Battle System");
                    return null;
                }
                if (target.Hp <= 0)
                {
                    this.SendMessage(source.Group, $"尊敬的{sName} 目标对象正在濒危状态，无法攻击!", "Battle System");
                    return null;
                }

                if ((DateTime.Now - source.LastAttackDate) < TimeSpan.FromMinutes(1))
                {
                    this.SendMessage(source.Group, $"尊敬的{sName} 每分钟只能攻击一次!请稍后再试!", "Battle System");
                    return null;
                }
                var result = ProcessBattle(source, target);
                //WSSendMessage sendMsg = new WSSendMessage()
                //{
                //    Value = $"[{client.Name}]攻击了[{v}],造成了[{v}]损失{result.TargetDamage}的HP。[{v}]剩余{target.Hp}点HP。",
                //    SenderName = client.Name,
                //    Date = DateTime.Now,
                //    SenderId = client.Id,
                //    Action = "msg"
                //};
                //client.SendToRoomClients(sendMsg);
                source.LastAttackDate = DateTime.Now;
                return $"[{sName}]攻击了[{v}],造成了[{v}]损失{result.TargetDamage}的HP。[{v}]剩余{target.Hp}点HP。";
            };
            Reg["状态"] = (v, client) =>
            {
                var source = GetUser(client);
                this.SendMessage(client.FromGroup, $"尊敬的{source.QQName} 您目前HP：{source.Hp},金币：{source.EcUser.Gold},经验：{source.Exp},当前等级：{LevelCalc(source)}", "Battle System");
                return null;
            };
            Reg["买活"] = (v, client) => {
                var source = GetUser(client);
                this.SendMessage(client.FromGroup, $"尊敬的{source.QQName} 买活将消耗你 400 金币,您当前金币剩余{source.EcUser.Gold},请输入#确认买活 来进行买活", "Battle System");
                return null;
            };
            Reg["确认买活"] = (v, client) => {
                var source = GetUser(client);
                if (source.Hp > 0)
                {
                    this.SendMessage(client.FromGroup, $"尊敬的{source.QQName} 您目前很健康!无需买活!", "Battle System");
                    return null;
                }
                if (source.EcUser.Gold < 400)
                {
                    this.SendMessage(client.FromGroup, $"尊敬的{source.QQName} 所需金币不够!无法买活! 您可以输入/打工 来赚取金币", "Battle System");
                    return null;
                }

                source.EcUser.Gold -= 400;
                source.Hp = 500;
                this.SendMessage(client.FromGroup, $"尊敬的{source.QQName} 买活成功!您已经恢复健康啦!", "Battle System");
                return null;
            };
        }
        private BattleResult ProcessBattle(BattleUser source,BattleUser target)
        {
            string sName = source.QQName;
            string tName = target.QQName;
            BattleResult result = new BattleResult();
            int sourceLevel = LevelCalc(source);
            int targetLevel = LevelCalc(target);

            int damage = DamageCalc(sourceLevel, targetLevel);
            result.TargetDamage = damage;
            //如果攻击对象等级比自己高，自己受到反伤
            if (sourceLevel < targetLevel)
            {
                result.SourceDamage = Convert.ToInt32(damage / 2);
            }

            //处理角色
            target.Hp -= result.TargetDamage;
            source.Hp -= result.SourceDamage;
            if (source.Hp < 0)
            {
                SendMessage(source.Group, $"{sName}已进入濒危状态，请输入#买活 复活，费用100G", "Battle System");
            }
            if (target.Hp < 0)
            {
                SendMessage(target.Group, $"{tName}已进入濒危状态，请输入#买活 复活，费用100G", "Battle System");
                SendMessage(source.Group, $"{sName}成功击破了[{tName}] 获得200G，30EXP", "Battle System");
                source.EcUser.Gold += 200;
                source.Exp += 30;

                int newLevel= LevelCalc(source);
                if (newLevel > sourceLevel)
                {
                    SendMessage(source.Group, $"恭喜{sName}升级，当前等级{newLevel},您的HP已全部回复", "Battle System");
                    source.Hp = 500;
                }
            }
            return result;
        }
        private BattleUser GetUser(MessageContent client)
        {
            string id = client.FromGroup.Group.Id + "_" + client.FromQQ.Id;
            if (Users.ContainsKey(id))
            {
                return Users[id];
            }

            Users[id] = new BattleUser()
            {
                QQId = client.FromQQ.Id,
                QQName = client.FromQQ.GetNick(client.FromGroup.RobotQQ),
                EcUser = CommandSystem.GetModule(typeof(EcommerceCommand)).GetPlugInter("get").Invoke(client),
                Group=client.FromGroup
            };

            return Users[id];
        }

        private BattleUser GetUserByGroupMember(GroupMember member,MessageContent client)
        {
            string id = client.FromGroup.Group.Id + "_" + member.QQ;
            if (Users.ContainsKey(id))
            {
                Users[id].QQName = !string.IsNullOrEmpty(member.GroupNickName) ? member.GroupNickName : member.NickName;
                return Users[id];
            }

            Users[id] = new BattleUser()
            {
                QQId = member.QQ,
                QQName = string.IsNullOrEmpty(member.GroupNickName)? member.GroupNickName:member.NickName,
                EcUser = CommandSystem.GetModule(typeof(EcommerceCommand)).GetPlugInter("get").Invoke(client),
                Group = client.FromGroup
            };

            return Users[id];
        }

        /// <summary>
        /// 伤害计算
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private int DamageCalc(int sourceLevel, int targetLevel)
        {
            int baseDamage = 50;
            Random rd = new Random();
            baseDamage += rd.Next(0, 100);
            baseDamage += Convert.ToInt32(baseDamage * (sourceLevel - targetLevel) * 0.7);
            return baseDamage;
        }

        private int LevelCalc(BattleUser user)
        {
            return Convert.ToInt32(Math.Floor(user.Exp / 100f));
        }
    }
}
