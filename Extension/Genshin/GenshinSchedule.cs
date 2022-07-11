using HyperBot.Command;
using HyperBot.Core;
using HyperBot.Model;
using HyperBot.Schedule;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyperBot.Extension.Genshin
{
    class PostModel
    {
        public string subject { get; set; }
        public string content { get; set; }
        public string url { get; set; }
        public string type { get; set; }
        public long created_at { get; set; }
    }
    public class GenshinScheduleCommand : BaseCommand
    {
        IConfiguration config { get; set; }
        public GenshinScheduleCommand(IConfiguration configuration)
        {
            this.config= configuration;
        }

        public override void RegCmd()
        {
            Reg["原神通知"] = (v, _g,_u) =>
            {
                if (_g.OwnerId != _u.Id && _u.Id != this.config.GetValue<string>("adminQQ"))
                {
                    return null;
                }
                if (v != "开启" && v != "关闭")
                {
                    return "请输入 [#原神通知 开启/关闭] 来打开或者关闭原神官方通知系统。";
                }
                var schedule = ScheduleSystem.GetSchedule<GenshinSchedule>();
                if (v == "开启")
                {
                    //schedule.Enable = true;
                    schedule.AddClient(_g);
                    return "原神官方通知开启成功";
                }
                else
                {
                    //schedule.Enable = false;
                    schedule.RemoveClient(_g);
                    return "原神官方通知已关闭";
                }
            };
        }
    }
    public class GenshinSchedule : BaseSchedule
    {

        HttpClient http = new HttpClient();

        PostModel last_gg { get; set; }
        PostModel last_hd { get; set; }
        PostModel last_zx { get; set; }
        public GenshinSchedule()
        {
            this.Enable = true;
            last_gg = new PostModel()
            {
                created_at = 0
            };
            last_hd = new PostModel()
            {
                created_at = 0
            };
            last_zx = new PostModel()
            {
                created_at = 0
            };
            this.ScheduleName = "米游社原神通知";
        }
        public override void Do(GroupClient group, UserClient user)
        {
            lock (locker)
            {
                //if (clients.Count() <= 0)
                //{
                //    return;
                //}

                //公告
                var new_gg = this.GetPost(1);
                if (new_gg != null && new_gg.created_at > last_gg.created_at)
                {
                    this.last_gg = new_gg;
                    this.SendPost(new_gg);
                }
                Thread.Sleep(500);
                //活动
                var new_hd = this.GetPost(2);
                if (new_hd != null && new_hd.created_at > last_hd.created_at)
                {
                    this.last_hd = new_hd;
                    this.SendPost(new_hd);
                }
                Thread.Sleep(500);
                //咨询
                var new_zx = this.GetPost(3);
                if (new_zx != null && new_zx.created_at > last_zx.created_at)
                {
                    this.last_zx = new_zx;
                    this.SendPost(new_zx);
                }
            }
        }

        private void SendPost(PostModel post)
        {
            foreach (var client in this.clients)
            {
                List<string> msg = new List<string>() {
                    $"[原神{post.type}]{post.subject}",
                    //post.content.Length>10?post.content.Substring(0,10)+"...":post.content,
                    post.url
                };
                client.SendMessage(string.Join("\n", msg));
            }
        }

        public override DateTime GetNextRunTime(DateTime currents)
        {
            //return currents.AddMinutes(1);
            return currents.AddMinutes(5);
        }

        /// <summary>
        /// 抓取公告 1=公告 2=活动 3=咨询
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private PostModel GetPost(int type)
        {
            try
            {
                string url = "https://api-takumi.mihoyo.com/post/wapi/getNewsList?gids=2&page_size=1&type=" + type;
                Task<string> t = http.GetStringAsync(url);
                t.Wait();
                var v = t.Result;
                JToken obj = JsonConvert.DeserializeObject<JToken>(v);
                if (obj?["data"]?["list"].Count() > 0)
                {
                    PostModel r = new PostModel();
                    if (obj["data"]["list"][0]?["post"]?["created_at"].ToObject<long>() == null)
                    {
                        return null;
                    }
                    r.created_at = obj["data"]["list"][0]["post"]["created_at"].ToObject<long>();
                    r.subject = obj["data"]["list"][0]["post"]["subject"].ToString();
                    r.content = obj["data"]["list"][0]["post"]["content"].ToString();
                    r.url = "https://bbs.mihoyo.com/ys/article/" + obj["data"]["list"][0]["post"]["post_id"].ToString();
                    switch (type)
                    {
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
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
