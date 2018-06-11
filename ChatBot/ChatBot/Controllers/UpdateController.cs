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
        public ClientController client = new ClientController();

        public async Task UpdateTitle(string title)
        {
            string updateUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}", Authenticate.UserId);
            title = "channel[status]=" + title.Replace(" ", "+");
            var content = new StringContent(title, Encoding.UTF8, Headers.ContentType[0].ToString());

            HttpResponseMessage response = await client.PutRequest(Headers.Authorization, Headers.ClientId, Headers.Accept, content, updateUrl);
            
            var ResponseBody = await response.Content.ReadAsStringAsync();
        }

        public async Task UpdateGame(string game)
        {
            string updateUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}", Authenticate.UserId);
            game = "channel[game]=" + game.Replace(" ", "+");
            var content = new StringContent(game, Encoding.UTF8, Headers.ContentType[0].ToString());

            HttpResponseMessage response = await client.PutRequest(Headers.Authorization, Headers.ClientId, Headers.Accept, content, updateUrl);

            var ResponseBody = await response.Content.ReadAsStringAsync();
        }

        public List<string> CommunityIDList = new List<string>();

        public async Task UpdateCommunities(List<string> communities)
        {
            CommunityIDList.Clear();
            string updateUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}/communities", Authenticate.UserId);
            foreach (string community in communities)
            {
                await GetCommunityJson(community);
            }
            string communitiesList = string.Format("{{\"community_ids\":[\"{0}\",\"{1}\",\"{2}\"]}}", CommunityIDList[0], CommunityIDList[1], CommunityIDList[2]);
            var content = new StringContent(communitiesList, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutRequest(Headers.Authorization, null, Headers.Accept, content, updateUrl);
                        
            var ResponseBody = await response.Content.ReadAsStringAsync();
        }

        public async Task GetCommunityJson(string community)
        {
            string updateUrl = String.Format("https://api.twitch.tv/kraken/communities?name={0}", community);

            HttpResponseMessage response = await client.GetRequest(null, Headers.ClientId, Headers.Accept, updateUrl);

            var responseBody = await response.Content.ReadAsStringAsync();

            GetCommunityID(JObject.Parse(responseBody)).Children().ToList();
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