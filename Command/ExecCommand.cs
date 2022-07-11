using HyperBot.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace HyperBot.Command
{
    public class ExecPluginModel
    {
        public string name { get;set;}
        public string description { get;set;}
        public string version { get;set;}
        public List<string> cmd_list { get;set;}
    }
    class ResponseMessageTask
    {
        public string group_id { get; set; }
        public string msg { get; set; } 
    }

    class ResponseMessage
    {
        public string msg { get; set; }
        public List<ResponseMessageTask> tasks { get; set; }
    }
    public class ExecCommand : BaseCommand
    {
        public List<FileInfo> scriptFiles { get; set; }
        public ExecCommand(IConfiguration configuration)
        {

        }
        public string ResponseAction(string cmd,ExecPluginModel plugin, RunnerModule runner, string msg, GroupClient group, UserClient user)
        {
            object item = new
            {
                cmd = cmd,
                msg = new
                {
                    msg=msg,
                    group=group,
                    user=user
                }
            };
            string raw_str = runner.SendCmdWait("$cmd:"+ JsonSerializer.Serialize<object>(item));
            raw_str = System.Web.HttpUtility.UrlDecode(raw_str);
            ResponseMessage response= JsonSerializer.Deserialize<ResponseMessage>(raw_str);
            foreach(var task in response.tasks)
            {
                if (string.IsNullOrWhiteSpace(task.group_id)||task.group_id==group.Id)
                {
                    group.SendMessage(task.msg);
                }
                else
                {
                    //不支持
                }
            }
            return response.msg;
        }
        public override void RegCmd()
        {
            //加载所有cmd js文件
            scriptFiles = new List<FileInfo>();
            if (!Directory.Exists("Scripts"))
            {
                Directory.CreateDirectory("Scripts");
            }
            DirectoryInfo script_folder = new DirectoryInfo("Scripts");
            foreach (FileInfo file in script_folder.GetFiles())
            {
                scriptFiles.Add(file);
            }

            //运行并加载所有脚本
            foreach (FileInfo file in scriptFiles)
            {
                if (!file.Name.ToLower().StartsWith("hybot_"))
                {
                    continue;
                }
                
                string file_type=file.Name.Substring(file.Name.LastIndexOf("."));
                string prog = "";
                switch (file_type.ToLower())
                {
                    case ".py":
                        prog = "python";
                        break;
                    case ".js":
                        prog = "node";
                        break;
                    default:break;
                }
                if (string.IsNullOrEmpty(prog))
                {
                    continue;
                }
                RunnerModule runner = new RunnerModule(prog, file.Name, script_folder.FullName, true);

                //1.初始化主窗口，向窗口发送$init_{host地址}消息，返回所有指令
                runner.Run();
                string raw_str=runner.SendCmdWait("$init:localhost:5150");
                raw_str = System.Web.HttpUtility.UrlDecode(raw_str);
                ExecPluginModel item= JsonSerializer.Deserialize<ExecPluginModel>(raw_str);
                foreach (string _cmditem in item.cmd_list)
                {
                    this.Reg[_cmditem] = (string msg, GroupClient group, UserClient user) =>
                     {
                         return ResponseAction(_cmditem, item, runner, msg, group, user);
                     };
                }
                //System.Console.WriteLine(item.name);
            }
        }
    }
}
