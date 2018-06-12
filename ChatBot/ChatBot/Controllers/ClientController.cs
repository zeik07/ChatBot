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
            //Applies required headers to perform a GET request to Twitch
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

            //Performs GET request to Twitch
            HttpResponseMessage response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> PostRequest(string url)
        {
            //Performs a POST request to Twitch
            HttpContent content = null;
            HttpResponseMessage response = await client.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            return response;
        }

        public async Task<HttpResponseMessage> PutRequest(List<KeyValuePair<string, string>> auth, List<KeyValuePair<string, string>> id, List<KeyValuePair<string, string>> accept, StringContent content, string url)
        {
            //Applies the required headers to perform a PUT request to Twitch
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

            //Performs a PUT request to Twitch
            HttpResponseMessage response = await client.PutAsync(url, content);
            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}