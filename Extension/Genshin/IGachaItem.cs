using System;

namespace HyperBot.Extension.Genshin
{
    public class IGachaItem
    {
        public string Name { get; set; }
        public string Level { get; set; }
        public string Type { get; set; }
        public string Content { get; set; }
        public int Value { get; set; }
        public string Element { get; set; } = "正常";
        public string WeaponType { get; set; } = "";
        public bool IsNew { get; set; } = false;
        public virtual string GetStar()
        {
            string r = "";
            int level = Convert.ToInt32(this.Level);
            for (int i = 0; i < level; i++)
            {
                r += "★";
            }
            return r;
        }

        public virtual string GetFullName()
        {
            string r = "[";
            r += GetTypeCn(true);
            r += "] ";
            r += this.Name + " ";
            r += this.GetStar();
            return r;
        }

        public virtual string GetSingleName()
        {
            string r = "[";
            r += this.Level + "星";
            r += GetTypeCn();
            r += "] ";
            r += this.Name;
            return r;
        }
        public virtual string GetTypeCn(bool highlight = false)
        {
            string r = "";
            if (this.Type == "character")
            {
                if (highlight)
                {
                    r += "+";
                    r += "角色";
                    r += "+";
                }
                else
                {
                    r += "角色";
                }

            }
            else if (this.Type == "item")
            {
                r += "道具";
            }
            else if (this.Type == "card")
            {
                if (highlight)
                {
                    r += "+";
                    r += "卡片";
                    r += "+";
                }
                else
                {
                    r += "卡片";
                }
            }
            else
            {
                r += "武器";
            }
            return r;
        }
    }

    public class UserItem
    {
        public IGachaItem Item { get; set; }
        public int ItemLevel { get; set; } = 1;
        public int Count { get; set; }
    }
}
