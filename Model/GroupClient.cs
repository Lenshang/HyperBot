using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Model
{
    public class GroupClient:IClient
    {
        public List<UserClient> MemberList { get; set; } = new List<UserClient>();
        /// <summary>
        /// 群主ID（QQ号）
        /// </summary>
        public string OwnerId { get; set; }
        /// <summary>
        /// 管理员ID列表（QQ号）
        /// </summary>
        public List<string> AdminIdList { get; set; } = new List<string>();
        public GroupClient(string id, string nickName,string ownerId)
        {
            this.OwnerId= ownerId;
            this.Id = id;
            this.NickName = nickName;
        }
        /// <summary>
        /// 根据ID（QQ号）确认是否在群中
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CheckMemberById(string id)
        {
            return this.MemberList.Where(i => i.Id == Id).Count() > 0;
        }
        /// <summary>
        /// 根据ID（QQ号）获得一个群员对象
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserClient GetMemberById(string id)
        {
            return this.MemberList.Where(i => i.Id == id).FirstOrDefault();
        }
        /// <summary>
        /// 添加一个成员
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="isAdmin"></param>
        public void AddMembers(string id,string name,bool isAdmin = false)
        {
            if (!this.CheckMemberById(id))
            {
                this.MemberList.Add(new UserClient() {
                    Id=id,
                    NickName=name
                });
                if (isAdmin)
                {
                    this.AdminIdList.Add(id);
                }
            }
        }
        /// <summary>
        /// 根据 昵称 获得一个群员对象
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UserClient GetMemberByName(string name)
        {
            return this.MemberList.Where(i => i.NickName == name).FirstOrDefault();
        }
    }
}
