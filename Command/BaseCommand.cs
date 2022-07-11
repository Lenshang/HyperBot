using HyperBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Command
{
    public abstract class BaseCommand
    {
        /// <summary>
        /// 注册的方法
        /// </summary>
        public Dictionary<string, Func<string, GroupClient, UserClient, string>> Reg = new Dictionary<string, Func<string, GroupClient, UserClient, string>>();
        /// <summary>
        /// 动态接口
        /// </summary>
        public Dictionary<string, Func<dynamic, dynamic>> PlugInter = new Dictionary<string, Func<dynamic, dynamic>>();
        public Func<dynamic, dynamic> GetPlugInter(string name)
        {
            if (!this.PlugInter.ContainsKey(name))
            {
                return null;
            }
            return this.PlugInter[name];
        }
        public abstract void RegCmd();
    }
}
