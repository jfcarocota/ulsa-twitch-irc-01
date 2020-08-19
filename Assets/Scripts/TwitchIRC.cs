using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;

public class TwitchIRC : MonoBehaviour
{
   TcpClient twitchClient;
   StreamReader reader;
   StreamWriter writer;

   [SerializeField]
   string username, password, channelName;

   [SerializeField]
   Transform trsCube;

   float rotSpeed = 0;

    void Start()
    {
        Connect();
    }

    void Update()
    {
        if(!Connected)
        {
            Connect();
        }
        trsCube.Rotate(Vector3.forward * rotSpeed * Time.deltaTime);
        ReadChat();
    }

   void Connect()
   {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        writer = new StreamWriter(twitchClient.GetStream());
        reader = new StreamReader(twitchClient.GetStream());

        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER " + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName);
        writer.Flush();
   }

   void ReadChat()
   {
       if(HasMessage)
       {
           string message = reader.ReadLine();
           if(message.Contains("PRIVMSG"))
           {
               int splitPoint = message.IndexOf(":", 1);
               message = message.Substring(splitPoint + 1).ToLower();
               if(message.Equals("!rot"))
               {
                   rotSpeed = 180f;
               }
               if(message.Equals("!stop"))
               {
                   rotSpeed = 0f;
               }
           }
       }
   }

   bool Connected => twitchClient.Connected;

   bool HasMessage => twitchClient.Available > 0;
}
