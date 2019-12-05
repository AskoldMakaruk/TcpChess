using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SimpleTCP;

namespace ChessServer
{

    public class Figure
    {
        public Figures Figures { get; set; }
        public static implicit operator Figures(Figure s)
        {
            return s.Figures;
        }
        public static explicit operator Figure(int i)
        {
            return new Figure() { Figures = (Figures) i };
        }
        public static implicit operator int(Figure value)
        {
            return (int) value.Figures;
        }
    }

    [Flags]
    public enum Figures
    {
        Black = 0, //0
        White = 1, //1
        Pawn = 4, //2
        Rook = 8, //3
        Knight = 16, //4    
        Bishop = 32, //5    
        Queen = 64, //6
        King = 128, //7
        PawnsTrail = 256 //8        
    }
    public static class Program
    {
        public static int[] ToPositions(this ulong moves)
        {
            var result = new List<int>();
            for (var i = 0; i < 64; i++)
            {
                if ((moves & (ulong) 1 << i) != 0)
                {
                    result.Add(i);
                }
            }
            return result.ToArray();
        }
        public static string ToStringPositions(this ulong moves)
        {
            var result = "";
            for (var i = 0; i < 64; i++)
            {
                if ((moves & (ulong) 1 << i) != 0)
                {
                    result += i + " ";
                }
            }
            return result;
        }

        static void Main(string[] args)
        {
            var server = new SimpleTcpServer();
            server.Delimiter = Encoding.UTF8.GetBytes("\n") [0];
            server.DelimiterDataReceived += (sender, msg) =>
            {
                Console.WriteLine(msg.MessageString);
                msg.ReplyLine("You said: " + msg.MessageString + "\n");
            };
            server.ClientConnected += (sender, client) =>
            {
                Console.WriteLine("Connected: " + ((IPEndPoint) client.Client.RemoteEndPoint).Address.MapToIPv4().ToString());

            };
            server.ClientDisconnected += (sender, client) =>
            {
                Console.WriteLine("Disconected: " + ((IPEndPoint) client.Client.RemoteEndPoint).Address.MapToIPv4().ToString());
            };
            server.Start(22832);
            // var MaxThreadsCount = Environment.ProcessorCount * 4;
            // ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            // ThreadPool.SetMinThreads(2, 2);
            // new Server(22832);

            Console.ReadLine();
        }

    }
}