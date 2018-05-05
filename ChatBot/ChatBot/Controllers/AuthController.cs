using ChatBot.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ChatBot.Controllers
{
    public class AuthController : Controller
    {
        public async Task<IActionResult> GetTokens()
        {
            string tokensUrl = String.Format(
                "https://id.twitch.tv/oauth2/token?client_id=i5p26xmsi1xqaf47rk031z60qns1tj&client_secret=7l89xz347lkgslrbtldz0pw1bau9vw&grant_type=authorization_code&redirect_uri=http://localhost:51083&code={0}",
                AuthViewModel.AuthorizationCode);
            HttpClient client = new HttpClient();
            HttpContent content = null;
            HttpResponseMessage response = await client.PostAsync(tokensUrl, content);
            response.EnsureSuccessStatusCode();
            AuthViewModel.ResponseBody = await response.Content.ReadAsStringAsync();
            return null;
        }

        public async Task<IActionResult> GetUsernameJson(string access)
        {
            string responseBody = null;
            string userUrl = String.Format("https://api.twitch.tv/kraken?oauth_token={0}", access);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(userUrl);
            response.EnsureSuccessStatusCode();
            responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody != null)
            {
                GetUsername(JObject.Parse(responseBody)).Children().ToList();
            }
            return null;
        }

        public async Task<IActionResult> GetUserIdJson(string name)
        {
            string responseBody = null;
            string userUrl = String.Format("https://api.twitch.tv/helix/users?login={0}", name);
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthViewModel.InitialTokens["access_token"]);
            HttpResponseMessage response = await client.GetAsync(userUrl);
            response.EnsureSuccessStatusCode();
            responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody != null)
            {
                GetUserId(JObject.Parse(responseBody)).Children().ToList();
            }
            return null;
        }

        private IEnumerable<JToken> GetUserId(JToken json)
        {
            foreach (var c in json.Children())
            {
                string path = c.Path.ToString();
                yield return c;
                foreach (var cc in GetUserId(c))
                {
                    string child = cc.ToString();
                    if (path == "data[0].id")
                    {
                        AuthViewModel.UserId = cc.ToString();
                    }
                    yield return cc;
                }
            }
        }

        private IEnumerable<JToken> GetUsername(JToken json)
        {
            foreach (var c in json.Children())
            {
                string path = c.Path.ToString();
                yield return c;
                foreach (var cc in GetUsername(c))
                {
                    string child = cc.ToString();
                    if (path == "token.user_name")
                    {
                        AuthViewModel.UserName = child;
                    }
                    yield return cc;
                }
            }
        }
        
        public IEnumerable<JToken> SortInitialTokens(JToken json)
        {            
            Dictionary<string, string> tokens = new Dictionary<string, string>();
            foreach (var c in json.Children())
            {
                if (c.ToString().Contains("scope") is false)
                {
                    yield return c;
                    foreach (var cc in SortInitialTokens(c))
                    {
                        string path = c.Path.ToString();
                        string child = cc.ToString();
                        tokens.Add(path, child);
                        AuthViewModel.InitialTokens = tokens;
                        yield return cc;
                    }
                }
            }
        }
    }
}
