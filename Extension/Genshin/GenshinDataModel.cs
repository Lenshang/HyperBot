﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace HyperBot.Extension.Genshin
{
    public class GenshinDataModel
    {
        public List<IGachaItem> 常驻 { get; set; } = new List<IGachaItem>();
        public List<IGachaItem> 角色up { get; set; } = new List<IGachaItem>();
        public List<IGachaItem> 武器up { get; set; } = new List<IGachaItem>();
        public List<IGachaItem> 未开放 { get; set; } = new List<IGachaItem>();
        public List<IGachaItem> 未开放武器 { get; set; } = new List<IGachaItem>();

        public Dictionary<string, double> 常驻概率 { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> 角色up概率 { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> 武器up概率 { get; set; } = new Dictionary<string, double>();


    }
    public static class GenshinGacha
    {
        public static GenshinDataModel Data;
        public static List<IGachaItem> CardDb = new List<IGachaItem>();
        static GenshinGacha()
        {
            string dataStr = File.ReadAllText("./Extension/Genshin/data/genshin_data.json");
            Data = JsonSerializer.Deserialize<GenshinDataModel>(dataStr);

            CardDb.AddRange(Data.常驻);
            CardDb.AddRange(Data.角色up);
            CardDb.AddRange(Data.武器up);
            CardDb.AddRange(Data.未开放);
            CardDb.AddRange(Data.未开放武器);
        }
        public static void Reload()
        {
            string dataStr = File.ReadAllText("./Extension/Genshin/data/genshin_data.json");
            Data = JsonSerializer.Deserialize<GenshinDataModel>(dataStr);

            CardDb = new List<IGachaItem>();
            CardDb.AddRange(Data.常驻);
            CardDb.AddRange(Data.角色up);
            CardDb.AddRange(Data.武器up);
            CardDb.AddRange(Data.未开放);
            CardDb.AddRange(Data.未开放武器);
        }
        /// <summary>
        /// 抽一张卡
        /// </summary>
        /// <param name="pool">1=常驻,2=角色UP,3=武器UP</param>
        /// <param name="hasup">结果是否包含UP</param>
        /// <param name="baodi">是否只出4星或5星 一般用在10连没有4星或90连没有5星</param>
        /// <param name="isUp">当为UP池时如果出5星是否必定为UP角色</param>
        /// <returns></returns>
        public static IGachaItem GetOne(int pool, ref bool hasup, int baodi = 0, bool isUp = false)
        {
            Dictionary<string, double> pr = null;
            List<IGachaItem> cards = null;
            hasup = false;
            if (pool == 1)
            {
                pr = Data.常驻概率;
                cards = Data.常驻;
            }
            else if (pool == 2)
            {
                pr = Data.角色up概率;
                cards = new List<IGachaItem>();
                cards.AddRange(Data.角色up);
                cards.AddRange(Data.角色up.Where(i => i.Level == "4"));
                cards.AddRange(Data.角色up.Where(i => i.Level == "4"));
                cards.AddRange(Data.角色up.Where(i => i.Level == "4"));
                cards.AddRange(Data.常驻.Where(i => i.Level == "4"));
            }
            else
            {
                pr = Data.武器up概率;
                cards = new List<IGachaItem>();
                cards.AddRange(Data.武器up);
                cards.AddRange(Data.武器up.Where(i => i.Level == "4"));
                cards.AddRange(Data.武器up.Where(i => i.Level == "4"));
                cards.AddRange(Data.武器up.Where(i => i.Level == "4"));
                cards.AddRange(Data.常驻.Where(i => i.Level == "4"));
            }

            int rd = GetRandom(0, 1000);

            if (rd < pr["5"] * 1000 || baodi == 5)//如果抽中5星
            {
                if (isUp || GetRandom(0, 2) == 1)
                {
                    hasup = true;
                    var _cards = cards.Where(i => i.Level == "5").ToList();
                    return _cards[GetRandom(0, _cards.Count())];
                }
                else if (pool == 2)
                {
                    var _cards = Data.常驻.Where(i => i.Level == "5" && i.Type == "character").ToList();
                    return _cards[GetRandom(0, _cards.Count())];
                }
                else if (pool == 3)
                {
                    var _cards = Data.常驻.Where(i => i.Level == "5" && i.Type == "weapon").ToList();
                    return _cards[GetRandom(0, _cards.Count())];
                }
                var _cards2 = Data.常驻.Where(i => i.Level == "5").ToList();
                return _cards2[GetRandom(0, _cards2.Count())];
            }
            else if (rd < pr["4"] * 1000 || baodi == 4)//如果抽中4星
            {
                var _cards = cards.Where(i => i.Level == "4").ToList();
                return _cards[GetRandom(0, _cards.Count())];
            }
            else//给三星
            {
                var _cards = Data.常驻.Where(i => i.Level == "3").ToList();
                return _cards[GetRandom(0, _cards.Count())];
            }
        }

        //public static GenshinItem GetOne(int pool)
        //{
        //    bool hasup = false;
        //    return GetOne(pool, ref hasup);
        //}
        public static IGachaItem[] Get(int pool, ref int count, int getcount = 10)
        {
            IGachaItem[] result = new IGachaItem[getcount];
            bool _hasup = false;//单次抽取是否有UP
            bool has4 = false;
            for (int i = 0; i < getcount; i++)
            {
                count += 1;
                if (count == 90)
                {
                    result[i] = GetOne(pool, ref _hasup, 5);
                    has4 = true;
                }
                else if (count == 180)
                {
                    result[i] = GetOne(pool, ref _hasup, 5, true);
                    has4 = true;
                }
                else if (i == 9 && !has4)
                {
                    result[i] = GetOne(pool, ref _hasup, 4);
                }
                else
                {
                    result[i] = GetOne(pool, ref _hasup, 0);
                    if (result[i].Level == "4" || result[i].Level == "5")
                    {
                        has4 = true;
                    }
                }

                if (result[i].Level == "5" && !_hasup)
                {
                    count = 90;
                }
                if (_hasup)
                {
                    count = 0;
                }
            }
            return result;
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

        public static IGachaItem GetItemFromName(string name)
        {
            return CardDb.Where(i => i.Name == name).FirstOrDefault();
        }
        public static List<IGachaItem> GetItemFromNames(List<string> names)
        {
            //List<IGachaItem> r = new List<IGachaItem>();
            //foreach(var name in names)
            //{
            //    var card= CardDb.Where(i => i.Name == name).FirstOrDefault();
            //    if (card != null)
            //    {
            //        r.Append(card);
            //    }
            //}
            return CardDb.Where(i => names.Contains(i.Name)).ToList();
        }
    }
}
