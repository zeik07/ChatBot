using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.Models
{
    public class Headers
    {
        public static List<KeyValuePair<string, string>> ClientId = new List<KeyValuePair<string, string>>()
        {
            new KeyValuePair<string, string>("Client-ID", "i5p26xmsi1xqaf47rk031z60qns1tj")
        };

        public static List<KeyValuePair<string, string>> Accept = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Accept", "application/vnd.twitchtv.v5+json")
        };

        public static List<KeyValuePair<string, string>> Authorization = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("Authorization", " OAuth " + Authenticate.InitialTokens["access_token"])
        };

        public static List<string> ContentType = new List<string>
        {
            "application/x-www-form-urlencoded",
            "application/json"
        };

    }
}
