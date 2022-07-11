using HyperBot.Command;
using HyperBot.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HyperBot.Core
{
    public class CommandSystem
    {
        public static string Trigger = "#";
        public static List<BaseCommand> CommandModules = new List<BaseCommand>();
        private static Dictionary<string, Func<string, GroupClient, UserClient, string>> Actions = new Dictionary<string, Func<string, GroupClient, UserClient, string>>();

        static CommandSystem()
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type _type in types)
            {
                if (_type.BaseType == typeof(BaseCommand))
                {
                    BaseCommand obj = Activator.CreateInstance(_type, configuration) as BaseCommand;
                    //obj.RegCmd();
                    CommandModules.Add(obj);
                    //foreach (var key in obj.Reg.Keys)
                    //{
                    //    Actions[key] = obj.Reg[key];
                    //}
                }
            }
            LoadCommand();
        }
        /// <summary>
        /// 重载所有指令
        /// </summary>
        public static void LoadCommand()
        {
            Actions = new Dictionary<string, Func<string, GroupClient, UserClient, string>>();
            foreach(var mod in CommandModules)
            {
                mod.RegCmd();
                foreach (var key in mod.Reg.Keys)
                {
                    Actions[key] = mod.Reg[key];
                }
            }
        }
        public static void Execute(string content,GroupClient group,UserClient user)
        {
            Func<string, GroupClient, UserClient, string> cmd = null;
            string message = content.Trim();
            if (!message.StartsWith(Trigger))
            {
                return;
            }

            string _cmd = message.IndexOf(" ") > 0 ? message.Substring(Trigger.Length, message.IndexOf(" ") - 1).Trim() : message.Substring(Trigger.Length).Trim();
            if (Actions.TryGetValue(_cmd, out cmd))
            {
                string v = message.Substring(message.IndexOf(" ") + 1).Trim();
                if (v.Substring(1) == _cmd)
                {
                    v = null;
                }
                string r = cmd.Invoke(v, group,user);
                if (!string.IsNullOrWhiteSpace(r))
                {
                    if (group != null)
                    {
                        StoreSystem.AddMessage(r, group);
                    }
                    else
                    {
                        StoreSystem.AddMessage(r, user);
                    }
                }
                
            }
        }
        public static BaseCommand GetModule(Type type)
        {
            foreach (var module in CommandModules)
            {
                if (module.GetType() == type)
                {
                    return module;
                }
            }
            return null;
        }
    }
}
