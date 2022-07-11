using HyperBot.Command;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HyperBot.Extension.Genshin
{
    public class HelpCommand : BaseCommand
    {
        public HelpCommand(IConfiguration config)
        {

        }
        public override void RegCmd()
        {
            List<string> content = new List<string>();
            content.Add("++经济系统指令++");
            content.Add("[#打工] 每小时限1次,获得一些金钱");
            content.Add("[#打很长时间的工] 每18小时限1次，获得大量金钱");
            content.Add("[#乞讨] 本机器人也许会给你赏点钱？");
            content.Add("[#查金钱] 查看当前拥有的金钱");
            content.Add("[#百度 关键词] 百度搜索一个内容 消耗100金");
            content.Add("[#背包 页码] 查看你当前的背包");
            content.Add("[#查看 物品名称] 查看你拥有的物品的说明");
            content.Add("[#卖出 物品名称] 卖出一个你拥有的指定物品");
            content.Add("[#卖出全部 物品名称] 卖出所有你拥有的指定物品");
            content.Add("[#发红包 at目标 金额] 发一个大红包");
            content.Add("++世界BOSS系统指令++");
            content.Add("[#讨伐] 讨伐当前的BOSS");
            content.Add("[#偏好角色] 设置讨伐BOSS的偏好角色");
            content.Add("[#清除偏好角色] 清除设置讨伐BOSS的偏好角色");
            content.Add("[#状态] 查看自己的状态");
            content.Add("[#买活] 消耗金币复活自己");
            content.Add("[#升级] 消耗金币给自己的伙伴升级");
            content.Add("++抽卡系统指令++");
            content.Add("[#原神抽卡] 查看原神抽卡相关说明");
            //content.Add("[#幻想乡抽卡] 查看幻想乡抽卡相关说明");
            content.Add("++战斗系统指令++");
            //content.Add("[#攻击 at目标] 攻击目标,每分钟只能攻击一次");
            content.Add("[#状态] 查看自己的状态");
            content.Add("[#买活] 消耗金币复活自己");
            content.Add("++工具指令++");
            content.Add("[#原神百科 内容] 搜索原神百科");
            content.Add("[#MD5加密 内容] 加密一条内容，每次消耗100金");
            content.Add("[#算卦] 就是算卦....一次10金");
            content.Add("[#抽签] 同算卦");
            content.Add("[#算命] 同算卦");
            //content.Add("[#色图 关键词] 你懂的,一次300金");
            //content.Add("[#搜图 关键词] 你懂的,一次300金");
            Reg["帮助"] = (v, _g, _u) =>
            {
                int page = GetPage(v);
                int maxPage = content.Count() / 10;
                maxPage += content.Count() % 10 == 0 ? 0 : 1;
                if (page > maxPage)
                {
                    page = 1;
                }
                string result = "===HyperBot机器人帮助===\n" + string.Join("\n", content.Skip((page - 1) * 10).Take(10));
                result += $"\n第{page}/{maxPage}页 (输入[#帮助 页数]查看其他页)";
                return result;
            };
        }
        int GetPage(string v)
        {
            try
            {
                if (string.IsNullOrEmpty(v))
                {
                    return 1;
                }
                return Convert.ToInt32(v);
            }
            catch
            {
                return 1;
            }
        }
    }
}
