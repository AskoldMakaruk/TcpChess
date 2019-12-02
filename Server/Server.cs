using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChessServer
{
    public class Server
    {
        TcpListener Listener;

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
            var client = new Client((TcpClient) StateInfo);
        }

        ~Server()
        {
            if (Listener != null)
                Listener.Stop();

        }

    }
}