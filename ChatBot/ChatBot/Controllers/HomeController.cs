using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatBot.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace ChatBot.Controllers
{
    public class HomeController : Controller
    {
        public static string ResponseData { get; set; }
        public static string Name { get; set; }
        public static Dictionary<string, string> Tokens = new Dictionary<string, string>();

        public async Task<IActionResult> Index()
        {
            ViewBag.Login = "https://id.twitch.tv/oauth2/authorize?client_id=i5p26xmsi1xqaf47rk031z60qns1tj&redirect_uri=http://localhost:51083&response_type=code&scope=chat_login%20user_read";
            string responseBody = null;
            string codeCheck = HttpContext.Request.Query["code"];
            if (codeCheck != null)
            {
                string tokensUrl = String.Format("https://id.twitch.tv/oauth2/token?client_id=i5p26xmsi1xqaf47rk031z60qns1tj&client_secret=7l89xz347lkgslrbtldz0pw1bau9vw&grant_type=authorization_code&redirect_uri=http://localhost:51083&code={0}", codeCheck);
                HttpClient client = new HttpClient();
                HttpContent content = null;
                HttpResponseMessage response = await client.PostAsync(tokensUrl, content);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();                
            }

            if (responseBody != null)
            {
                ResponseData = responseBody;
                return Redirect("/Home/Dashboard");
            }

            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            string json = ResponseData;
            AllChildren(JObject.Parse(json)).Children().ToList();
            string access = Tokens["access_token"];

            await UsernameJson(access);
            
            ViewBag.Username = Name;
            
            return View();
        }

        private async Task<IActionResult> UsernameJson(string access)
        {
            string responseBody = null;
            string userUrl = String.Format("https://api.twitch.tv/kraken?oauth_token={0}", access);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(userUrl);
            response.EnsureSuccessStatusCode();
            responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody != null)
            {
                Username(JObject.Parse(responseBody)).Children().ToList();
            }

            return this.Content("test");
        }

        private IEnumerable<JToken> Username(JToken json)
        {            
            foreach (var c in json.Children())
            {
                string path = c.Path.ToString();
                yield return c;
                foreach (var cc in Username(c))
                {                        
                    string child = cc.ToString();
                    if (path == "token.user_name")
                    {
                        Name = child;
                    }
                    yield return cc;
                }       
            }

        }

        private IEnumerable<JToken> AllChildren(JToken json)
        {
            Dictionary<string, string> tokens = new Dictionary<string, string>();
            foreach (var c in json.Children())
            {
                if (c.ToString().Contains("scope") is false)
                {
                    yield return c;
                    foreach (var cc in AllChildren(c))
                    {
                        string path = c.Path.ToString();
                        string child = cc.ToString();                        
                        tokens.Add(path, child);
                        Tokens = tokens;
                        yield return cc;
                    }
                }
            }            
        }

        public IActionResult About()
        {
            return View();
        }
                
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
