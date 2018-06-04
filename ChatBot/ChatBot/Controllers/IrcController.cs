using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChatBot.Controllers
{
    public class IrcController : Controller
    {
        private static string UserName = Authenticate.UserName.ToLower();
        private static string Password = Authenticate.InitialTokens["access_token"];

        IrcClient Irc = new IrcClient("irc.chat.twitch.tv", 6667, UserName, Password);
        NetworkStream ServerStream = default(NetworkStream);
        string ReadData = "";
        Thread ChatThread;

        public void StartIrc()
        {            
            //Starts the Irc client
            Irc.JoinRoom(UserName);            
            ChatThread = new Thread(GetMessage);
            ChatThread.Start();
        }
        
        private void GetMessage()
        {
            //Gets messages from the Irc client data stream
            ServerStream = Irc.tcpClient.GetStream();
            int buffSize = 0;
            byte[] inStream = new byte[10025];
            buffSize = Irc.tcpClient.ReceiveBufferSize;
            while (true)
            {
                try
                {
                    ReadData = Irc.ReadMessage();
                    Msg();
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
            foreach (KeyValuePair<string, string> commands in CommandList.CommandsList)
            {
                if (command.ToLower() == commands.Key.ToString().ToLower())
                {
                    Irc.SendChatMessage(commands.Value.ToString());
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

        public IrcClient(string ip, int port, string userName, string password)
        {
            //Sends the required information to Twitch to connect to their Irc server
            tcpClient = new TcpClient(ip, port);
            inputStream = new StreamReader(tcpClient.GetStream());
            outputStream = new StreamWriter(tcpClient.GetStream());

            outputStream.WriteLine("PASS oauth:" + password);
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
            string userName = Authenticate.UserName.ToLower();
            SendIrcMessage(":" + userName + "!" + userName + "@" + userName + ".tmi.twitch.tv PRIVMSG #" + userName + " :" + message);
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