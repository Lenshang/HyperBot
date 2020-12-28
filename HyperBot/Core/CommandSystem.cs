using HyperBot.Model;
using RolePlayChat.Core.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XQ.Net.SDK.EventArgs;

namespace HyperBot.Core
{
    public class CommandSystem
    {
        public static List<BaseCommand> CommandModules = new List<BaseCommand>();
        private static Dictionary<string, Func<string, MessageContent, string>> Actions = new Dictionary<string, Func<string, MessageContent, string>>();
        static CommandSystem()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type _type in types)
            {
                if (_type.BaseType == typeof(BaseCommand))
                {
                    BaseCommand obj = Activator.CreateInstance(_type) as BaseCommand;
                    CommandModules.Add(obj);
                    foreach (var key in obj.Reg.Keys)
                    {
                        Actions[key] = obj.Reg[key];
                    }
                }
            }
        }
        public static void Execute(MessageContent content, Action<string,GroupClient> callback)
        {
            Func<string, MessageContent, string> cmd = null;
            string message = content.Message;
            if (!message.StartsWith("#"))
            {
                return;
            }

            string _cmd = message.IndexOf(" ") > 0 ? message.Substring(1, message.IndexOf(" ") - 1).Trim() : message.Substring(1).Trim();
            if (Actions.TryGetValue(_cmd, out cmd))
            {
                string v = message.Substring(message.IndexOf(" ") + 1).Trim();
                string r = cmd.Invoke(v, content);
                callback.Invoke(r, content.FromGroup);
            }
            //return message;
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

        public static Func<object, object> GetPlugInter(Type type, string plugName)
        {
            var module = GetModule(type);
            if (module == null)
            {
                return null;
            }

            if (!module.PlugInter.ContainsKey(plugName))
            {
                return null;
            }

            return module.PlugInter[plugName];
        }
    }
}
