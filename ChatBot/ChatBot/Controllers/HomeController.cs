﻿using System;
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
using ChatBot.ViewModels;

namespace ChatBot.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            if (Models.Authenticate.InitialTokens != null)
            {
                return Redirect("/Home/Dashboard");
            }
            Models.Authenticate.AuthorizationCode = HttpContext.Request.Query["code"];
            Models.Authenticate.IrcState = false;
            if (Models.Authenticate.AuthorizationCode != null)
            {
                AuthController Auth = new AuthController();
                await Auth.GetTokens();
            }
            if (Models.Authenticate.ResponseBody != null)
            {                
                return Redirect("/Home/Dashboard");
            }
            return View();
        }
        
        public async Task<IActionResult> Dashboard()
        {
            if (Models.Authenticate.ResponseBody == null)
            {
                return Redirect("/Home");
            }
            AuthController Auth = new AuthController();
            Auth.SortInitialTokens(JObject.Parse(Models.Authenticate.ResponseBody)).Children().ToList();
            await Auth.GetUser(Models.Authenticate.InitialTokens["access_token"]);
            StreamInfoController Info = new StreamInfoController();
            await Info.GetStreamInfoJson(Models.Authenticate.UserId);
            await Info.GetStreamCommunityJson(Models.Authenticate.UserId);
            if (Models.Authenticate.IrcState == false)
            {
                IrcController Irc = new IrcController();
                Irc.StartIrc();
                Models.Authenticate.IrcState = true;
            }
            
            return View();
        }

        public IActionResult Authenticate()
        {
            string url = Models.Authenticate.LoginUrl;
            return Redirect(url);
        }

        public async Task<IActionResult> Update(StreamInfoViewModel streamInfoViewModel)
        {
            string game = streamInfoViewModel.Game;
            string title = streamInfoViewModel.Title;
            List<string> communities = streamInfoViewModel.Communities;

            AuthController Auth = new AuthController();
            await Auth.Validate();

            UpdateController UpdateInfo = new UpdateController();
            //TODO: Only update title/game if changed, update all communities is any change
            await UpdateInfo.UpdateGame(game);
            await UpdateInfo.UpdateTitle(title);
            await UpdateInfo.UpdateCommunities(communities);

            return Redirect("/Home/Dashboard");
        }

        public IActionResult About()
        {
            return View();
        }
                
        public IActionResult Error()
        {
            return View(new Error { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Commands()
        {
            return View();
        }
    }
}
