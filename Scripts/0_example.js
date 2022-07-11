(function () {
    let count = 0;
    let http = new HttpClient();
    //注册指令例子 #js count
    Hyper.reg("js", new CommandAction((v, group, user) => {
        //传入v=参数 group=群相关信息 user=发言用户相关信息
        switch (v) {
            case "count":
                count += 1;
                return count.toString();
            case "group":
                return JSON.stringify(group);
            case "member":
                return JSON.stringify(group.MemberList[0]);
        }
        return "Hello JS";
    }));

    //注册计划任务例子
    Hyper.regSchedule(
        "test_schedule",
        new NextRunDateAction(dt => {
            //传入当前时间，定义下一次运行的时间
            return dt.AddSeconds(5);
        }),
        new ScheduleAction(groups => {
            //传入 groups=群相关信息
            for (let i = 0; i < groups.Count; i++) {
                groups[i].SendMessage("JS Schedule");
            }
            return "";
        }));

    Hyper.log("Example Script Loaded");
})();