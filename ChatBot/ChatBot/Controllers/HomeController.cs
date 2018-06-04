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
using ChatBot.ViewModels;

namespace ChatBot.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Index()
        {
            //Check to see if initial tokens have been recieved already to prevent double logins
            if (Models.Authenticate.InitialTokens != null)
            {
                return Redirect("/Home/Dashboard");
            }
            //Redirects to Twitch for Authorization of this app for use with the chosen account login
            Models.Authenticate.AuthorizationCode = HttpContext.Request.Query["code"];
            //Sets the state of the Irc client to false for initial pass through, set to true on Irc client launch to prevent duplicate launches/messages in chat
            Models.Authenticate.IrcState = false;
            //Checks to make sure that the Authorization Code is recieved from Twitch after the app has been authorized, then gets a set of authoization tokens
            if (Models.Authenticate.AuthorizationCode != null)
            {
                AuthController Auth = new AuthController();
                await Auth.GetTokens();
            }
            //Checks to see that tokens were recieved and redirects to the dashboard if they were
            if (Models.Authenticate.ResponseBody != null)
            {                
                return Redirect("/Home/Dashboard");
            }
            return View();
        }
        
        public async Task<IActionResult> Dashboard()
        {
            //Check to see if there is a Response Body with tokens to process
            if (Models.Authenticate.ResponseBody == null)
            {
                return Redirect("/Home");
            }
            AuthController Auth = new AuthController();
            //Sorts the initial tokens and gathers they one that are needed
            Auth.SortInitialTokens(JObject.Parse(Models.Authenticate.ResponseBody)).Children().ToList();
            //Gets the Username and User ID from Twitch for the account that was used to Authenticate
            await Auth.GetUser(Models.Authenticate.InitialTokens["access_token"]);
            StreamInfoController Info = new StreamInfoController();
            //Gets the Twitch channels title and game/catagory
            await Info.GetStreamInfoJson(Models.Authenticate.UserId);
            //Gets the Twitch channels communities
            await Info.GetStreamCommunityJson(Models.Authenticate.UserId);
            //Checks to see if the Irc client is started, if not then it starts the Irc client and sets the state to true to avoid duplicate launches
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
            //Redirects to the url used to login to Twitch to authenticate for the app
            string url = Models.Authenticate.LoginUrl;
            return Redirect(url);
        }

        public async Task<IActionResult> Update(StreamInfoViewModel streamInfoViewModel)
        {
            //Retrieves the current set game, title, and communites from the dashboard webpage
            string game = streamInfoViewModel.Game;
            string title = streamInfoViewModel.Title;
            List<string> communities = streamInfoViewModel.Communities;

            //Validates the login information to make sure the app is still authorized to makes changes
            AuthController Auth = new AuthController();
            await Auth.Validate();

            //Updates the game, title, and communities on Twitch with the information on the dashboard
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
