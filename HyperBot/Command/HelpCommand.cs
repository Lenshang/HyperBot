using RolePlayChat.Core.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperBot.Command
{
    public class HelpCommand : BaseCommand
    {
        public HelpCommand()
        {
            List<string> content = new List<string>();
            content.Add("++经济系统指令++");
            content.Add("[#打工] 每分钟限1次,获得一些金钱");
            content.Add("[#打很长时间的工] 每6小时限1次，获得大量金钱");
            content.Add("[#查金钱] 查看当前拥有的金钱");
            content.Add("[#百度 关键词] 百度搜索一个内容 消耗100金");
            content.Add("[#背包 页码] 查看你当前的背包");
            content.Add("[#查看 物品名称] 查看你拥有的物品的说明");
            content.Add("[#卖出 物品名称] 卖出一个你拥有的指定物品");
            content.Add("[#卖出全部 物品名称] 卖出所有你拥有的指定物品");
            content.Add("[#发红包 目标昵称=金额] 发一个大红包");
            content.Add("++抽卡系统指令++");
            content.Add("[#原神抽卡] 查看原神抽卡相关说明");
            content.Add("[#幻想乡抽卡] 查看幻想乡抽卡相关说明");
            content.Add("++战斗系统指令++");
            content.Add("[#攻击 目标昵称] 攻击目标,每分钟只能攻击一次");
            content.Add("[#状态] 查看自己的状态");
            content.Add("[#买活] 消耗金币复活自己");
            content.Add("++工具指令++");
            content.Add("[#MD5加密 内容] 加密一条内容，每次消耗100金");
            content.Add("[#算卦] 就是算卦....一次100金");
            content.Add("[#抽签] 同算卦");
            content.Add("[#算命] 同算卦");
            Reg["帮助"] = (v, client) =>
            {
                int page = GetPage(v);
                int maxPage = content.Count() / 10;
                maxPage += content.Count() % 10 == 0 ? 0 : 1;
                if (page > maxPage)
                {
                    page = 1;
                }
                string result = "===HyperBot机器人帮助===\n"+ string.Join("\n", content.Skip((page - 1) * 10).Take(10));
                result += $"\n第{page}/{maxPage}页 (输入[#帮助 页数]查看其他页)";
                return result;
            };
        }
        int GetPage(string v)
        {
            try
            {
                return Convert.ToInt32(v);
            }
            catch
            {
                return 1;
            }
        }
    }
}
