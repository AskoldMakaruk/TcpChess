using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChessServer
{
    public class Server
    {
        private readonly TcpListener Listener;
        public static List<Player> Clients = new List<Player>();
        public Server(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start();
            while (true)
            {
                ThreadPool.QueueUserWorkItem(ClientThread, Listener.AcceptTcpClient());
            }
        }

        static void ClientThread(object StateInfo)
        {
            Clients.Add(new Player((TcpClient) StateInfo));
        }

        ~Server()
        {
            if (Listener != null)
                Listener.Stop();

        }

    }
}