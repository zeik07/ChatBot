using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.Models
{
    public class CommandList
    {
        public static Dictionary<string, string> CommandsList = new Dictionary<string, string>
        {
            { "test", "Testing!" },
            { "game", StreamInfo.Game },
            { "title", StreamInfo.Title }
        };
}
}
