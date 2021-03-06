﻿using System;
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
        public ClientController client = new ClientController();
    
        public async Task GetStreamInfoJson(string userId)
        {
            //Requests channel data from Twitch (games, title, etc)
            string streamUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}", userId);

            HttpResponseMessage response = await client.GetRequest(null, Headers.ClientId, Headers.Accept, streamUrl);

            string ResponseBody = await response.Content.ReadAsStringAsync();
            if (ResponseBody != null)
            {
                GetStreamInfo(JObject.Parse(ResponseBody)).Children().ToList();
            }
        }

        public async Task GetStreamCommunityJson(string userId)
        { 
            //Requests channel community data from Twitch
            string streamUrl = String.Format("https://api.twitch.tv/kraken/channels/{0}/communities", userId);

            HttpResponseMessage response = await client.GetRequest(null, Headers.ClientId, Headers.Accept, streamUrl);

            string ResponseBody = await response.Content.ReadAsStringAsync();
            if (ResponseBody != null)
            {
                GetCommunityInfo(JObject.Parse(ResponseBody)).Children().ToList();
            }
            StreamInfo.Communities = communities;
        }

        private IEnumerable<JToken> GetStreamInfo(JToken json)
        {
            //Loops through stream info Json from Twitch to get the channel title and channel game/catagory
            foreach (var c in json.Children())
            {
                string path = c.Path.ToString();
                yield return c;
                foreach (var cc in GetStreamInfo(c))
                {
                    string child = cc.ToString();
                    if (path == "status")
                    {
                        StreamInfo.Title = child;
                    }
                    if (path == "game")
                    {
                        StreamInfo.Game = child;
                    }
                    yield return cc;
                }
            }
        }

        List<string> communities = new List<string>();

        private IEnumerable<JToken> GetCommunityInfo(JToken json)
        {
            //Loops through community Json from Twitch to find the communities for the channel
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