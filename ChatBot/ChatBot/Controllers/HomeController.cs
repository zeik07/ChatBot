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
        public async Task<IActionResult> Index()
        {
            if (AuthViewModel.Tokens != null)
            {
                return Redirect("/Home/Dashboard");
            }
            ViewBag.Login = "https://id.twitch.tv/oauth2/authorize?client_id=i5p26xmsi1xqaf47rk031z60qns1tj&redirect_uri=http://localhost:51083&response_type=code&scope=chat_login%20user_read";
            AuthViewModel.CodeCheck = HttpContext.Request.Query["code"];
            if (AuthViewModel.CodeCheck != null)
            {
                AuthController Auth = new AuthController();
                await Auth.GetToken();
            }

            if (AuthViewModel.ResponseBody != null)
            {
                AuthViewModel.ResponseData = AuthViewModel.ResponseBody;
                return Redirect("/Home/Dashboard");
            }

            return View();
        }
        
        public async Task<IActionResult> Dashboard()
        {
            if (AuthViewModel.ResponseBody == null)
            {
                return Redirect("/Home");
            }
            AuthController Auth = new AuthController();
            Auth.AllChildren(JObject.Parse(AuthViewModel.ResponseData)).Children().ToList();
            await Auth.UsernameJson(AuthViewModel.Tokens["access_token"]);
            
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
