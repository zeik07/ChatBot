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
        public ClientController client = new ClientController();

        public async Task GetTokens()
        {
            string tokensUrl = String.Format(
                "https://id.twitch.tv/oauth2/token?client_id=i5p26xmsi1xqaf47rk031z60qns1tj&client_secret=7l89xz347lkgslrbtldz0pw1bau9vw&grant_type=authorization_code&redirect_uri=http://localhost:51083&code={0}",
                Authenticate.AuthorizationCode);

            HttpResponseMessage response = await client.PostRequest(tokensUrl);
            
            Authenticate.ResponseBody = await response.Content.ReadAsStringAsync();
        }

        public async Task GetUser(string access)
        {
            string userUrl = "https://api.twitch.tv/kraken/user";

            HttpResponseMessage response = await client.GetRequest(Headers.Authorization, Headers.ClientId, Headers.Accept, userUrl);

            string responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody != null)
            {
                GetUserInfo(JObject.Parse(responseBody)).Children().ToList();
            }
        }

        public async Task Validate()
        {
            string validateUrl = "https://id.twitch.tv/oauth2/validate";

            HttpResponseMessage response = await client.GetRequest(Headers.Authorization, null, null, validateUrl);
        }

        private IEnumerable<JToken> GetUserInfo(JToken json)
        {
            foreach (var c in json.Children())
            {
                string path = c.Path.ToString();
                yield return c;
                foreach (var cc in GetUserInfo(c))
                {
                    string child = cc.ToString();
                    if (path == "display_name")
                    {
                        Authenticate.UserName = child;
                    }
                    if (path == "_id")
                    {
                        Authenticate.UserId = child;
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
                        Authenticate.InitialTokens = tokens;
                        yield return cc;
                    }
                }
            }
        }
    }
}
