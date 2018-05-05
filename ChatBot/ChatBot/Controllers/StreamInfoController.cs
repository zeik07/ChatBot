using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ChatBot.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ChatBot.Controllers
{
    public class StreamInfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> GetStreamInfoJson(string userId)
        {
            string responseBody = null;
            string streamUrl = null;
            HttpResponseMessage response = null;
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
            client.DefaultRequestHeaders.Add("Client-Id", "i5p26xmsi1xqaf47rk031z60qns1tj");
            streamUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}", userId);
            response = await client.GetAsync(streamUrl);
            response.EnsureSuccessStatusCode();
            responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody != null)
            {
                GetStreamInfo(JObject.Parse(responseBody)).Children().ToList();
            }
            streamUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}/communities", userId);
            response = await client.GetAsync(streamUrl);
            response.EnsureSuccessStatusCode();
            responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody != null)
            {
                GetCommunityInfo(JObject.Parse(responseBody)).Children().ToList();
            }
            StreamInfoModel.Communities = communities;
            return null;
        }

        private IEnumerable<JToken> GetStreamInfo(JToken json)
        {
            foreach (var c in json.Children())
            {
                string path = c.Path.ToString();
                yield return c;
                foreach (var cc in GetStreamInfo(c))
                {
                    string child = cc.ToString();
                    if (path == "status")
                    {
                        StreamInfoModel.Title = child;
                    }
                    if (path == "game")
                    {
                        StreamInfoModel.Game = child;
                    }
                    yield return cc;
                }
            }
        }

        List<string> communities = new List<string>();

        private IEnumerable<JToken> GetCommunityInfo(JToken json)
        {
            foreach (var c in json.Children())
            {
                string path = c.Path.ToString();
                yield return c;
                foreach (var cc in GetCommunityInfo(c))
                {
                    string child = cc.ToString();
                    if (path == "communities[0].name")
                    {
                        communities.Add(child);
                    }
                    if (path == "communities[1].name")
                    {
                        communities.Add(child);
                    }
                    if (path == "communities[2].name")
                    {
                        communities.Add(child);
                    }
                    yield return cc;
                }
            }
        }
    }
}