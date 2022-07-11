using HyperBot.Command;
using HyperBot.Core;
using HyperBot.Model;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Schedule
{
    public class TestScheduleCommand : BaseCommand
    {
        public TestScheduleCommand(IConfiguration configuration)
        {   
        }

        public override void RegCmd()
        {
            Reg["测试计划任务"] = (v, group, client) => {
                var schedule = ScheduleSystem.GetSchedule<TestSchedule>();
                if (v == "开启")
                {
                    //schedule.Enable = true;
                    schedule.AddClient(group);
                    return "测试计划任务开启成功";
                }
                else
                {
                    //schedule.Enable = false;
                    schedule.RemoveClient(group);
                    return "测试计划任务已关闭";
                }
            };
        }
    }
    public class TestSchedule : BaseSchedule
    {
        public TestSchedule()
        {
            this.Enable = true;
            this.ScheduleName = "测试计划任务";
        }
        public override void Do(GroupClient group, UserClient user)
        {
            foreach(var client in this.clients)
            {
                client.SendMessage("现在时间:"+DateTime.Now.ToString());
            }
        }

        public override DateTime GetNextRunTime(DateTime currents)
        {
            return currents.AddSeconds(10);
        }
    }
}
