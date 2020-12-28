using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using XQ.Net.SDK.EventArgs;
using XQ.Net.SDK.Models;

namespace HyperBot.Model
{
    public class XQMembers
    {
        public class XQMemberItem
        {
            public long lst { get; set; }
            public long jt { get; set; }
            public int rm { get; set; }
            public int lad { get; set; }
            public int ll { get; set; }
            /// <summary>
            /// 昵称
            /// </summary>
            public string nk { get; set; }
            /// <summary>
            /// 群名片
            /// </summary>
            public string cd { get; set; }
        }
        public int ec { get; set; }
        public int errcode { get; set; }
        public string em { get; set; }
        public string c { get; set; }
        public int ext_num { get; set; }
        public int level { get; set; }
        public int mem_num { get; set; }
        public int max_num { get; set; }
        public int max_admin { get; set; }
        public long owner { get; set; }
        public Dictionary<string,string> levelname { get; set; }
        public Dictionary<string, XQMemberItem> members { get; set; }
    }
    public class GroupClient
    {
        public XQGroup Group { get; set; }
        public string RobotQQ { get; set; }
        public string Owner { get; set; }
        public List<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
        private DateTime LastUpdateDate { get; set; } = new DateTime(2000,1,1);
        public void Update(XQGroup group)
        {
            this.Group = group;
            if (DateTime.Now - this.LastUpdateDate > TimeSpan.FromHours(1))
            {
                var groupmemberstr = group.GetGroupMemberList_B(this.RobotQQ);
                var groupmember= JsonSerializer.Deserialize<XQMembers>(groupmemberstr);
                this.Owner = groupmember.owner.ToString();
                this.GroupMembers = new List<GroupMember>();
                foreach(var qid in groupmember.members.Keys)
                {
                    GroupMember _mb = new GroupMember()
                    {
                        QQ = qid
                    };
                    var xqmb = groupmember.members[qid];
                    _mb.NickName = xqmb.nk;
                    _mb.GroupNickName = xqmb.cd;
                    if (string.IsNullOrEmpty(_mb.GroupNickName))
                    {
                        _mb.GroupNickName = _mb.NickName;
                    }
                    if (this.Owner == qid)
                    {
                        _mb.IsOwner = true;
                    }
                    _mb.LastMessageDate=TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.FindSystemTimeZoneById("China Standard Time"));
                    _mb.LastMessageDate.AddSeconds(xqmb.lst);
                    this.GroupMembers.Add(_mb);
                }
            }
        }
        public GroupMember GetMemberByName(string name)
        {
            //Group.GetGroupMemberList(this.RobotQQ);
            return this.GroupMembers.Where(i => i.GroupNickName == name).FirstOrDefault();
        }

        
    }
}
