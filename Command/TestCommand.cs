using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HyperBot.Command
{
    public class TestCommand : BaseCommand
    {
        public TestCommand(IConfiguration configuration)
        {

        }

        public override void RegCmd()
        {
            Reg["test"] = (v, group, user) =>
            {
                return "HyperRobot";
            };
        }
    }
}
