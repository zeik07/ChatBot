using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBot.Models
{
    public class AuthViewModel
    {
        public static string Name { get; set; }
        public static Dictionary<string, string> Tokens { get; set; }
        public static string ResponseBody { get; set; }
        public static string CodeCheck { get; set; }
        public static string ResponseData { get; set; }
    }
}
