using System;
using System.Text;
using System.Text.RegularExpressions;

namespace HyperBot.Extension.Genshin
{
    public class Util
    {
        /// <summary>
        /// URL编码
        /// </summary>
        /// <param name="str">要进行编码的字符串</param>
        /// <returns></returns>
        public static string UrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(@"%" + Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString());
        }
        public static int GetRandom(int min, int max)
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            Random random = new Random(BitConverter.ToInt32(bytes, 0));
            int v = random.Next(min, max);
            return v;
        }
        static Regex QQ_Pattern = new Regex("\\[@[0-9]+\\]");
        public static bool MatchQQ(string v)
        {
            return QQ_Pattern.IsMatch(v);
        }
        public static string GetQQFromAt(string v)
        {
            var r = Regex.Match(v, "\\d+");
            return r.Value;
        }
    }
}
