using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.Models
{
    public class Headers
    {
        public static  Dictionary<string, string> ClientId = new Dictionary<string, string>
        {
            { "Client-ID", "i5p26xmsi1xqaf47rk031z60qns1tj" }
        };

        public static Dictionary<string, string> Accept = new Dictionary<string, string>
        {
            { "Accept", "application/vnd.twitchtv.v5+json"}
        };

        public static Dictionary<string, string> Authorization = new Dictionary<string, string>
        {
            {"Authorization", " OAuth " + Authenticate.InitialTokens["access_token"] }
        };

        public static List<string> ContentType = new List<string>
        {
            "application/x-www-form-urlencoded",
            "application/json"
        };

    }
}
