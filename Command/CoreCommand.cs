using HyperBot.Model;
using LiteDB;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Command
{

    public class CoreCommand : BaseCommand
    {
        Dictionary<string, CoreUser> Users { get; set; }
        Dictionary<string, CoreUser> ChangeList { get; set; }
        public CoreCommand(IConfiguration configuration)
        {
            Users = new Dictionary<string, CoreUser>();
            ChangeList = new Dictionary<string, CoreUser>();
        }
        public override void RegCmd()
        {
            //Reg["查金币"] = (v, group, user) => {
            //    var coreUser = this.GetUser(group, user);

            //    return $"{coreUser.QQName}当前金币:{coreUser.Gold}";
            //};
            //Reg["查金钱"] = Reg["查金币"];
        }
        public CoreUser GetUser(GroupClient group,UserClient user)
        {
            string id = group.Id + "_" + user.Id;
            if (this.Users.ContainsKey(id))
            {
                var r= this.Users[id];
                this.ChangeList[id] = r;
                return r;
            }
            var dbItem = this.GetFromDb(id);
            if (dbItem == null)
            {
                dbItem = new CoreUser()
                {
                    Id=id,
                    QQId=user.Id,
                    QQName=user.NickName
                };
            }
            this.Users[id] = dbItem;
            this.ChangeList[id] = dbItem;
            return dbItem;
        }
        public CoreUser GetUser(GroupClient group,string userId)
        {
            UserClient uc = new UserClient()
            {
                Id = userId,
                NickName= $"[@{userId}]",
            };
            return GetUser(group, uc);
        }
        public CoreUser GetFromDb(string id)
        {
            using (var db = new LiteDatabase(@"./hyper_bot.db"))
            {
                var col = db.GetCollection<CoreUser>("CoreUser");
                var r= col.FindById(id);
                if (r==null)
                {
                    return null;
                }
                return r;
            }
        }
        /// <summary>
        /// 同步数据库
        /// </summary>
        public void SyncDB()
        {
            using (var db = new LiteDatabase(@"./hyper_bot.db"))
            {
                var col = db.GetCollection<CoreUser>("CoreUser");
                foreach (var id in this.ChangeList.Keys)
                {
                    var dbItem = this.ChangeList[id].GetDbItem();
                    col.Upsert(dbItem);
                }
            }
            this.ChangeList = new Dictionary<string, CoreUser>();
        }
    }
}
