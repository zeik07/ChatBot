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
            if (AuthViewModel.InitialTokens != null)
            {
                return Redirect("/Home/Dashboard");
            }
            AuthViewModel.AuthorizationCode = HttpContext.Request.Query["code"];
            if (AuthViewModel.AuthorizationCode != null)
            {
                AuthController Auth = new AuthController();
                await Auth.GetTokens();
            }
            if (AuthViewModel.ResponseBody != null)
            {
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
            Auth.SortInitialTokens(JObject.Parse(AuthViewModel.ResponseBody)).Children().ToList();
            await Auth.GetUsernameJson(AuthViewModel.InitialTokens["access_token"]);
            await Auth.GetUserIdJson(AuthViewModel.UserName);
            StreamInfoController Info = new StreamInfoController();
            await Info.GetStreamInfoJson(AuthViewModel.UserId);
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
