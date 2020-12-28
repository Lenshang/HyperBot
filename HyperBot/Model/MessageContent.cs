using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XQ.Net.SDK.Models;

namespace HyperBot.Model
{
    public class MessageContent
    {
        public string Message { get; set; }
        public XQQQ FromQQ { get; set; }
        public GroupClient FromGroup { get; set; }
    }
}
