using ChatBot.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChatBot.Controllers
{
    public class AuthController
    {
        public async Task<IActionResult> UsernameJson(string access)
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
            return null;
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
                        AuthViewModel.Name = child;
                    }
                    yield return cc;
                }
            }

        }

        public IEnumerable<JToken> AllChildren(JToken json)
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
                        AuthViewModel.Tokens = tokens;
                        yield return cc;
                    }
                }
            }
        }

    }

}
