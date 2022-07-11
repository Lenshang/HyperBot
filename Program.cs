using HyperBot.Command;
using HyperBot.Core;
using HyperBot.Model.ApiModel;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HyperBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("HyperBot v2.0");
            

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            IConfiguration configuration = builder.Build();

            //测试代码======================================================
            bool is_debug = false;
            if (is_debug)
            {
                //测试 添加群信息
                UserInfo u1 = new UserInfo()
                {
                    id = "10000",
                    name = "连殇殇"
                };
                GroupInfo group1 = new GroupInfo()
                {
                    id = "123456",
                    name = "测试群",
                    adminids = new List<string> { "10000" },
                    owner = "10000",
                    members = new List<UserInfo>() { u1 }
                };
                StoreSystem.AddClient(group1);

                //测试 触发机器人逻辑
                var test_group = StoreSystem.GetClient(group1.id);
                var test_user = test_group.GetMemberById(u1.id);
                ScheduleSystem.RunOnce(test_group, test_user);
                while (true)
                {
                    Console.Write("you > ");
                    string input = Console.ReadLine();
                    if (input == "debug")
                    {
                        continue;
                    }
                    if (input == "sync")
                    {
                        var corecmd = CommandSystem.GetModule(typeof(CoreCommand)) as CoreCommand;
                        corecmd.SyncDB();
                    }
                    //步骤1 定时任务系统逻辑
                    ScheduleSystem.RunOnce(test_group, test_user);

                    //步骤2 执行命令
                    CommandSystem.Execute(input, test_group, test_user);

                    //步骤3 返回消息任务
                    foreach (var task in StoreSystem.GetMessages())
                    {
                        if (task.Target.Id == test_group.Id)
                        {
                            Console.WriteLine($"HyberBot > " + task.Message);
                        }
                    }
                }
            }
            //================================================================


            //初始化
            CreateHostBuilder(args).Build().RunAsync();
            MyQQClient.Init(configuration);
            //MyQQClient.Get().SendGroupMsg("hahaha", "584148382");
            foreach (var group in MyQQClient.Get().GetAllGroup())
            {
                StoreSystem.AddClient(group);
            }

            //全局循环
            while (true)
            {
                //步骤3 返回消息任务
                foreach (var task in StoreSystem.GetMessages())
                {
                    MyQQClient.Get().SendGroupMsg(task.Message, task.Target.Id);
                }
                Thread.Sleep(1000);
            }

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
