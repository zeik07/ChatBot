using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Models;
using ChatBot.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Controllers
{
    public class IrcController : Controller
    {
        private static string UserName;
        private static string Password;
        private static bool IrcState;

        IrcClient Irc = new IrcClient("irc.chat.twitch.tv", 6667, UserName, Password);
        NetworkStream ServerStream = default(NetworkStream);
        public string ReadData = "";
        Thread ChatThread;
        
        internal static void IrcSet()
        {
            UserName = Models.Irc.BotName;
            Password = Models.Irc.BotOAuth;
        }

        public void StartIrc()
        {            
            //Starts the Irc client
            if (Irc == null)
            {
                Irc = new IrcClient("irc.chat.twitch.tv", 6667, UserName, Password);
            }
            Irc.JoinRoom(Authenticate.UserName);
            ChatThread = new Thread(GetMessage);
            ChatThread.Start();
            IrcState = true;
        }

        public void StopIrc()
        {
            Irc.LeaveRoom();
            IrcState = false;
        }
        
        private void GetMessage()
        {
            //Gets messages from the Irc client data stream
            ServerStream = Irc.tcpClient.GetStream();
            int buffSize = 0;
            byte[] inStream = new byte[10025];
            buffSize = Irc.tcpClient.ReceiveBufferSize;
            while (IrcState == true)
            {                
                try
                {                    
                    ReadData = Irc.ReadMessage();
                    if (ReadData != null)
                    {
                        var test = ReadData;
                    }
                    if (IrcState != false)
                    {
                        Msg();
                    }
                }
                catch (Exception e)
                {

                }
            }
        }

        private void Msg()
        {
            //Processes the message data from the data stream
            string[] separator = new string[] { "#zeik07 :" };
            string[] singlesep = new string[] { ":", "!" };

            //Only processes actual chat messages and not Irc messages
            if (ReadData.Contains("PRIVMSG"))
            {
                string username = ReadData.Split(singlesep, StringSplitOptions.None)[1];
                string message = ReadData.Split(separator, StringSplitOptions.None)[1];

                //Looks for messages that start with ! to process them for command calls
                if (message[0] == '!')
                {
                    Commands(username, message);
                }
            }

            //Processes Irc messages looking for "PING" in order to respond
            if (ReadData.Contains("PING"))
            {
                Irc.PingResponse();
            }
        }        

        private void Commands(string userName, string message)
        {
            string command = message.Split(new[] { ' ', '!' }, StringSplitOptions.None)[1];
            //Checks to see if message containing the ! is actually a command and if so processes it for a response
            foreach (Tuple<string, string, string> commands in CommandList.CommandsTuple)
            {
                if (command.ToLower() == commands.Item1.ToString().ToLower())
                {
                    Irc.SendChatMessage(commands.Item2.ToString());
                    break;
                }
            }

            /*switch (command.ToLower())
            {
                case "test":
                    Irc.SendChatMessage("Testing!");
                    break;
            }*/
        }
    }

    class IrcClient
    {
        private string channel;

        public TcpClient tcpClient;
        public StreamReader inputStream;
        public StreamWriter outputStream;
        public string UserName;

        public IrcClient(string ip, int port, string userName, string password)
        {
            UserName = userName;
            //Sends the required information to Twitch to connect to their Irc server
            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine("PASS " + password);
            outputStream.WriteLine("NICK " + userName);
            outputStream.WriteLine("USER " + userName + " 8 * :" + userName);
            outputStream.WriteLine("CAP REQ :twitch.tv/membership");
            outputStream.WriteLine("CAP REQ :twitch.tv/commands");
            outputStream.Flush();
        }

        public void JoinRoom(string channel)
        {
            //Joins the channel of the user that connect to the bot app
            this.channel = channel;
            outputStream.WriteLine("JOIN #" + channel);
            outputStream.Flush();
        }

        public void LeaveRoom()
        {
            //Leaves the channel the bot is in         
            outputStream.Close();
            inputStream.Close();
        }

        public void SendIrcMessage(string message)
        {
            //Sends an Irc message to the Twitch Irc server
            outputStream.WriteLine(message);
            outputStream.Flush();
        }

        public void SendChatMessage(string message)
        {
            //Formats a message to send to the Twitch Irc server
            string userName = UserName.ToLower();
            SendIrcMessage(":" + userName + "!" + userName + "@" + userName + ".tmi.twitch.tv PRIVMSG #" + channel + " :" + message);
        }

        public void PingResponse()
        {
            //Responds to the "PING" request from Twitch with "PONG"
            SendIrcMessage("PONG tmi.twitch.tv");
        }

        public string ReadMessage()
        {
            // Clears the message variable and processes the current message
            string message = "";
            message = inputStream.ReadLine();
            return message;
        }
    }
}