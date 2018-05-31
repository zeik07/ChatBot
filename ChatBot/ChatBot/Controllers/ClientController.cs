using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ChatBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Controllers
{
    public class ClientController : Controller
    {
        public HttpClient client = new HttpClient();        

        public async Task<HttpResponseMessage> GetRequest(List<KeyValuePair<string, string>> auth, List<KeyValuePair<string, string>> id, List<KeyValuePair<string, string>> accept, string url)
        {
            client.DefaultRequestHeaders.Clear();
            if (id != null)
            {
                client.DefaultRequestHeaders.Add(id[0].Key.ToString(), id[0].Value.ToString());
            }
            if (accept != null)
            {
                client.DefaultRequestHeaders.Add(accept[0].Key.ToString(), accept[0].Value.ToString());
            }
            if (auth != null)
            {
                client.DefaultRequestHeaders.Add(auth[0].Key.ToString(), auth[0].Value.ToString());
            }

            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> PostRequest(string url)
        {
            HttpContent content = null;
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<IActionResult> PutRequest(List<KeyValuePair<string, string>> auth, List<KeyValuePair<string, string>> id, List<KeyValuePair<string, string>> accept, string content, string url)
        {
            return null;
        }
    }
}