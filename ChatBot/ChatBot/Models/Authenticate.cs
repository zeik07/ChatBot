using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.Models
{
    public class Authenticate
    {
        public static string LoginUrl = "https://id.twitch.tv/oauth2/authorize?client_id=i5p26xmsi1xqaf47rk031z60qns1tj&redirect_uri=http://localhost:51083&response_type=code&scope=chat_login channel_editor";
        public static string UserName { get; set; }
        public static string UserId { get; set; }
        public static Dictionary<string, string> InitialTokens { get; set; }
        public static string ResponseBody { get; set; }
        public static string AuthorizationCode { get; set; }
        public static bool IrcState { get; set; }
    }
}
