using HyperBot.Core;
using HyperBot.Model;
using HyperBot.Utils;
using LiteDB;
using RolePlayChat.Core.Command;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HyperBot.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //var jsonStr=File.ReadAllText("C:\\Users\\Len\\Desktop\\文档\\test.json");
            //var groupmember = JsonSerializer.Deserialize<XQMembers>(jsonStr);

            //Console.WriteLine("over");

            //CommandSystem.Execute(new MessageContent() {
            //    Message="#抽签"
            //}, null);

            //for(int i = 0; i < 10; i++)
            //{
            //    var r = GenshinGacha.GetOne(1);
            //    Console.WriteLine(r.Name +" "+r.Level);
            //}
            bool hasup = false;
            int count = 0;
            //using (var db = new LiteDatabase(@"./hyper_bot.db"))
            //{
            //    var col = db.GetCollection<DB_EcUser>("ecommerce");
            //    foreach(var item in col.FindAll())
            //    {
            //        Console.WriteLine(item.Id);
            //    }
            //}

            Dictionary<string, string> a = new Dictionary<string, string>();
            a.Add("a", "1");
            a.Add("b", "2");
            int maxPage = a.Keys.Count() / 10;
            maxPage += a.Keys.Count() % 10 == 0 ? 0 : 1;
            Console.WriteLine(maxPage);
            while (true)
            {
                Console.WriteLine("======按回车抽10连======");
                Console.ReadLine();
                
                foreach (var r in TouhouGacha.Get(2, ref count))
                {
                    Console.WriteLine(r.GetSingleName());
                }
            }

        }
    }
}
