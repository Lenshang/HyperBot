using HyperBot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Model
{
    public abstract class IClient
    {
        /// <summary>
        /// 昵称/群名称
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// ID 群号/QQ号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 最后消息时间
        /// </summary>
        private DateTime LastMsgDate { get; set; } = new DateTime(2000, 1, 1);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(string content)
        {
            StoreSystem.AddMessage(content, this);
        }
    }
}
