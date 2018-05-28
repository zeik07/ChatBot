using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatBot.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json.Linq;

namespace ChatBot.Controllers
{
    public class UpdateController : Controller
    {
        public HttpClient client = new HttpClient();

        public async Task<IActionResult> Update(string game, string title)
        {
            string updateUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}", Authenticate.UserId);
            game = game.Replace(" ", "+");
            title = title.Replace(" ", "+");
            game = "channel[game]=" + game;
            title = "channel[status]=" + title;
            var content = new StringContent((title + "&" + game), Encoding.UTF8, "application/x-www-form-urlencoded");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-ID", "i5p26xmsi1xqaf47rk031z60qns1tj");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
            client.DefaultRequestHeaders.Add("Authorization", " OAuth " + Authenticate.InitialTokens["access_token"]);
            HttpResponseMessage response = await client.PutAsync(updateUrl, content);
            response.EnsureSuccessStatusCode();
            var ResponseBody = await response.Content.ReadAsStringAsync();
            return null;
        }

        public List<string> CommunityIDList = new List<string>();

        public async Task<IActionResult> UpdateCommunities(List<string> communities)
        {
            CommunityIDList.Clear();
            string updateUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}/communities", Authenticate.UserId);
            foreach (string community in communities)
            {
                await GetCommunityJson(community);
            }
            string communitiesList = string.Format("{{\"community_ids\":[\"{0}\",\"{1}\",\"{2}\"]}}", CommunityIDList[0], CommunityIDList[1], CommunityIDList[2]);
            var content = new StringContent(communitiesList, Encoding.UTF8, "application/json");
            var test = content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
            client.DefaultRequestHeaders.Add("Authorization", " OAuth " + Authenticate.InitialTokens["access_token"]);
            HttpResponseMessage response = await client.PutAsync(updateUrl, content);
            response.EnsureSuccessStatusCode();
            var ResponseBody = await response.Content.ReadAsStringAsync();
            return null;
        }

        public async Task<IActionResult> GetCommunityJson(string community)
        {
            string updateUrl = String.Format("https://api.twitch.tv/kraken/communities?name={0}", community);
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-ID", "i5p26xmsi1xqaf47rk031z60qns1tj");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
            HttpResponseMessage response = await client.GetAsync(updateUrl);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            GetCommunityID(JObject.Parse(responseBody)).Children().ToList();

            return null;
        }

        private IEnumerable<JToken> GetCommunityID(JToken json)
        {
            foreach (var c in json.Children())
            {
                string path = c.Path.ToString();
                yield return c;
                foreach (var cc in GetCommunityID(c))
                {
                    string child = cc.ToString();
                    if (path == "_id")
                    {
                        CommunityIDList.Add(child);
                    }                    
                    yield return cc;
                }
            }
        }
    }
}