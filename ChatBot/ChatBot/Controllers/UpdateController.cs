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
            //var content = new FormUrlEncodedContent(postData);
            var test = content.ReadAsStringAsync();
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("Client-ID", "i5p26xmsi1xqaf47rk031z60qns1tj");
            client.DefaultRequestHeaders.Add("Accept", "application/vnd.twitchtv.v5+json");
            client.DefaultRequestHeaders.Add("Authorization", " OAuth " + Authenticate.InitialTokens["access_token"]);
            HttpResponseMessage response = await client.PutAsync(updateUrl, content);
            response.EnsureSuccessStatusCode();
            var ResponseBody = await response.Content.ReadAsStringAsync();
            return null;
        }
    }
}