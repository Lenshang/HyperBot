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

            //���Դ���======================================================
            bool is_debug = false;
            if (is_debug)
            {
                //���� ���Ⱥ��Ϣ
                UserInfo u1 = new UserInfo()
                {
                    id = "10000",
                    name = "������"
                };
                GroupInfo group1 = new GroupInfo()
                {
                    id = "123456",
                    name = "����Ⱥ",
                    adminids = new List<string> { "10000" },
                    owner = "10000",
                    members = new List<UserInfo>() { u1 }
                };
                StoreSystem.AddClient(group1);

                //���� �����������߼�
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
                    //����1 ��ʱ����ϵͳ�߼�
                    ScheduleSystem.RunOnce(test_group, test_user);

                    //����2 ִ������
                    CommandSystem.Execute(input, test_group, test_user);

                    //����3 ������Ϣ����
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


            //��ʼ��
            CreateHostBuilder(args).Build().RunAsync();
            MyQQClient.Init(configuration);
            //MyQQClient.Get().SendGroupMsg("hahaha", "584148382");
            foreach (var group in MyQQClient.Get().GetAllGroup())
            {
                StoreSystem.AddClient(group);
            }

            //ȫ��ѭ��
            while (true)
            {
                //����3 ������Ϣ����
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
