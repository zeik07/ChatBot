using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChatBot.Models;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace ChatBot.Controllers
{
    public class HomeController : Controller
    {
        public static string ResponseData { get; set; }
        
        public async Task<IActionResult> Index()
        {
            ViewBag.Login = "https://id.twitch.tv/oauth2/authorize?client_id=i5p26xmsi1xqaf47rk031z60qns1tj&redirect_uri=http://localhost:51083&response_type=code&scope=chat_login%20user_read";
            string responseBody = null;
            string codeCheck = HttpContext.Request.Query["code"];
            if (codeCheck != null)
            {
                string tokensUrl = String.Format("https://id.twitch.tv/oauth2/token?client_id=i5p26xmsi1xqaf47rk031z60qns1tj&client_secret=7l89xz347lkgslrbtldz0pw1bau9vw&grant_type=authorization_code&redirect_uri=http://localhost:51083&code={0}", codeCheck);
                HttpClient client = new HttpClient();
                HttpContent content = null;
                HttpResponseMessage response = await client.PostAsync(tokensUrl, content);
                response.EnsureSuccessStatusCode();
                responseBody = await response.Content.ReadAsStringAsync();                
            }

            if (responseBody != null)
            {
                ResponseData = responseBody;
                return Redirect("/Home/Dashboard");
            }

            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            AuthController Auth = new AuthController();
            string json = ResponseData;
            Auth.AllChildren(JObject.Parse(json)).Children().ToList();
            string access = AuthViewModel.Tokens["access_token"];

            await Auth.UsernameJson(access);
            
            ViewBag.Username = AuthViewModel.Name;
            
            return View();
        }
        
        public IActionResult About()
        {
            return View();
        }
                
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
