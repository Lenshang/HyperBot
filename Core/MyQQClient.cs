using HyperBot.Model.ApiModel;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HyperBot.Core
{
    public class MyQQClient
    {
        private HttpClient http { get; set; }
        private IConfiguration config { get; set; }
        public string robot_qq { get; set; }
        private MyQQClient(IConfiguration configuration)
        {
            this.http = new HttpClient();
            this.config = configuration;
            this.robot_qq = this.config.GetValue<string>("RobotQQ");
        }
        public List<GroupInfo> GetAllGroup()
        {
            List<GroupInfo> list = new List<GroupInfo>();
            string response_str = this.Post("Api_GetGroupList_B", new { c1 = this.robot_qq });
            dynamic response = JsonConvert.DeserializeObject<dynamic>(response_str);
            if (response.code != 200)
            {
                System.Console.WriteLine("群列表获取失败！");
                return list;
            }
            List<dynamic> all_groups = new List<dynamic>();
            all_groups.AddRange(response.data.ret.join);
            all_groups.AddRange(response.data.ret.manage);
            all_groups.AddRange(response.data.ret.create);
            foreach (var item in all_groups)
            {
                GroupInfo g = new GroupInfo() {
                    id=item.gc.ToString(),
                    name=item.gn.ToString(),
                    adminids=new List<string>(),
                    members=new List<UserInfo>(),
                    owner=item.owner.ToString()
                };
                list.Add(g);
            }
            return list;
        }

        public async void SendGroupMsg(string msg,string groupid)
        {
            await this.PostAsync("Api_SendMsg", new { 
                c1 = this.robot_qq,
                c2=2,
                c3=groupid,
                c4="",
                c5=msg
            });
        }
        private string Post(string function, Dictionary<string, object> param)
        {
            string url = this.config.GetValue<string>("MyQQApi");
            Dictionary<string,object> param_dic=new Dictionary<string, object>();
            param_dic.Add("function", function);
            param_dic.Add("token", "");
            param_dic.Add("params", param);
            var requestContent = new StringContent(JsonConvert.SerializeObject(param_dic), Encoding.UTF8, "application/json");
            var _t = this._Post(url, requestContent);
            _t.Wait();
            return _t.Result;
        }
        private string Post(string function, object param)
        {
            string url = this.config.GetValue<string>("MyQQApi");
            Dictionary<string, object> param_dic = new Dictionary<string, object>();
            param_dic.Add("function", function);
            param_dic.Add("token", "");
            param_dic.Add("params", param);
            var requestContent = new StringContent(JsonConvert.SerializeObject(param_dic), Encoding.UTF8, "application/json");
            var _t=this._Post(url,requestContent);
            _t.Wait();
            return _t.Result;
        }
        private async Task<string> PostAsync(string function, object param)
        {
            string url = this.config.GetValue<string>("MyQQApi");
            Dictionary<string, object> param_dic = new Dictionary<string, object>();
            param_dic.Add("function", function);
            param_dic.Add("token", "");
            param_dic.Add("params", param);
            var requestContent = new StringContent(JsonConvert.SerializeObject(param_dic), Encoding.UTF8, "application/json");
            return await this._Post(url, requestContent);
        }
        private async Task<string> _Post(string? requestUri, HttpContent content)
        {
            HttpResponseMessage t=await this.http.PostAsync(requestUri, content);
            var r=await t.Content.ReadAsStringAsync();
            return r;
        }
        private static MyQQClient myqq = null;
        public static void Init(IConfiguration configuration)
        {
            myqq = new MyQQClient(configuration);
        }
        public static MyQQClient Get()
        {
            return myqq;
        }
    }
}
