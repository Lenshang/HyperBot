using HyperBot.Command;
using HyperBot.Core;
using HyperBot.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Schedule
{
    public class CommonSchedule : BaseSchedule
    {
        public CoreCommand CoreModule { get; set; }
        public CommonSchedule()
        {
            this.Enable = true;
            this.ScheduleName = "全局计划任务";
            this.CoreModule = CommandSystem.GetModule(typeof(CoreCommand)) as CoreCommand;
        }
        public override void Do(GroupClient group, UserClient user)
        {
            this.CoreModule.SyncDB();
        }

        public override DateTime GetNextRunTime(DateTime currents)
        {
            return currents.AddHours(3);//每3小时执行一次
        }
    }
}
