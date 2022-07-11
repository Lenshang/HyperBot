using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Utils
{
    public class HttpClient: System.Net.Http.HttpClient
    {
        public string GetString(string? requestUri)
        {
            var t=this.GetStringAsync(requestUri);
            t.Wait();
            return t.Result;
        }
    }
}
