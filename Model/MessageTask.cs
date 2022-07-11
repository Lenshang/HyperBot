using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Model
{
    public class MessageTask
    {
        public string Message { get; set; }
        public IClient Target { get; set; }
    }
}
