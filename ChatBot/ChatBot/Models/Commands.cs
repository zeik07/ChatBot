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

        public static Tuple<string, string, string>[] CommandsTuple =
        {
            Tuple.Create("test", "Testing!", "Command used in testing bot functions."),
            Tuple.Create("game", StreamInfo.Game, "Displays the current set game for the channel"),
            Tuple.Create("title", StreamInfo.Title, "Displays the current set title for the channel")
        };
    }
}

