using HyperBot.Model;
using HyperBot.Schedule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HyperBot.Core
{
    public class ScheduleSystem
    {
        public static List<BaseSchedule> Schedules = new List<BaseSchedule>();
        static ScheduleSystem()
        {
            //查找所有模块添加
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type _type in types)
            {
                if (_type.BaseType == typeof(BaseSchedule))
                {
                    BaseSchedule obj = Activator.CreateInstance(_type) as BaseSchedule;
                    Schedules.Add(obj);
                }
            }
        }
        public static T GetSchedule<T>() where T : BaseSchedule
        {
            foreach (var task in Schedules)
            {
                if (task.GetType() == typeof(T))
                {
                    return task as T;
                }
            }
            return null;
        }

        public static void RunOnce(GroupClient group,UserClient user)
        {
            var now = DateTime.Now;
            foreach (var task in Schedules)
            {
                if (!task.Enable)
                {
                    continue;
                }
                if (now >= task.NextRunTime)
                {
                    //Log.Write($"[{task.ScheduleName}]Run once");
                    task.Do(group,user);
                    task.NextRunTime = task.GetNextRunTime(now);
                }
            }
        }
    }
}
