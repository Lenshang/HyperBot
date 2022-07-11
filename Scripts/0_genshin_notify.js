/*
 * 原神官方通知模块
 */
(function () {
    let http = new HttpClient();
    let black_list=[];//黑名单，不想发送群号的名单在这里添加
    let white_list = ["584148382"];//白名单，有值时，只有白名单内存在的群号会发送通知
    let last_gg = { created_at: 0 };//最后一条公告信息
    let last_hd = { created_at: 0 };//最后一条活动信息
    let last_zx = { created_at: 0 };//最后一条资讯信息
    /**
     * 抓取一条信息
     * @param {number} type 类型 1=公告 2=活动 3=资讯
     * @returns 
     */
    function getPost(type) {
        let url = "https://api-takumi.mihoyo.com/post/wapi/getNewsList?gids=2&page_size=1&type=" + type;
        let v = http.GetString(url);
        let obj = JSON.parse(v);
        if (obj["data"]["list"].length > 0) {
            let r = {
                created_at: obj["data"]["list"][0]["post"]["created_at"],
                subject: obj["data"]["list"][0]["post"]["subject"],
                content: obj["data"]["list"][0]["post"]["content"],
                url: "https://bbs.mihoyo.com/ys/article/" + obj["data"]["list"][0]["post"]["post_id"]
            }
            switch (type) {
                case 1:
                    r.type = "公告";
                    break;
                case 2:
                    r.type = "活动";
                    break;
                case 3:
                    r.type = "资讯";
                    break;
            }
            return r;
        }
        return null;
    }

    /**
     * 向所有群发送消息
     * @param {Array} groups 群组信息数组
     * @param {*} post 消息对象
     */
    function send(groups,post){
        for (let i = 0; i < groups.Count; i++) {
            if(groups.Id in black_list){
                continue;
            }
            if(white_list.length>0&&(!groups.Id in white_list)){
                continue;
            }
            let msg=`[原神${post.type}]${post.subject}\n${post.url}`
            groups[i].SendMessage(msg);
        }
    }

    /**
     * 主要入口
     * @param {Array} groups 群组信息
     * @returns 
     */
    function main(groups) {
        //公告
        var new_gg = getPost(1);
        if (new_gg != null && new_gg.created_at > last_gg.created_at)
        {
            last_gg = new_gg;
            send(groups,new_gg);
        }
        //活动
        var new_hd = getPost(2);
        if (new_hd != null && new_hd.created_at > last_hd.created_at)
        {
            last_hd = new_hd;
            send(groups,new_hd);
        }
        //咨询
        var new_zx = getPost(3);
        if (new_zx != null && new_zx.created_at > last_zx.created_at)
        {
            last_zx = new_zx;
            send(groups,new_zx);
        }
        return "";
    }

    Hyper.regSchedule(
        "genshin_notify",
        new NextRunDateAction(dt => {
            //传入当前时间，定义下一次运行的时间
            return dt.AddSeconds(60*5);
        }),
        new ScheduleAction(main));

    Hyper.log("原神官方通知模块加载完毕~");
})();