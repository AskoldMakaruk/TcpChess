using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace ChessServer
{
    public class Player
    {
        private TcpClient Client { get; set; }
        public Player(TcpClient client)
        {
            Client = client;
        }
        public void WaitForMessage()
        {
            var Request = "";
            var Buffer = new byte[1024];
            int Count;
            while (Client.Connected && (Count = Client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                Request += Encoding.UTF8.GetString(Buffer, 0, Count);

                if (Request.IndexOf("\n", StringComparison.Ordinal) < 0 && Request.Length <= 4096) continue;
                Console.WriteLine(Request);
                Request = "";
                var answer = "Hi, python!";
                var answerBytes = Encoding.UTF8.GetBytes(answer);
                Client.GetStream().Write(answerBytes, 0, answerBytes.Length);
            }

        }
        public void SendMessage(string message)
        {

        }

        [AttributeUsage(AttributeTargets.Method)]
        public class HandlerAttribute : Attribute
        {
            public HandlerAttribute()
            {

            }
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}