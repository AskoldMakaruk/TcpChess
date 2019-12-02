using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace HTTPServer
{
    class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    class Game
    {
        public Player BlackPlayer { get; set; }
        public Player WhitePlayer { get; set; }
        //00000000 - empty
        //1: 0 - black 1 - white
        //2: isFirstPawn
        //4: isPawn
        //8: isKnight
        //16: isBishop
        //32: isRook
        //64: isQueen
        //128: isKing
        //256: isPawn`s trail
        public int[] Board { get; set; }
        public Game()
        {
            Board = new int[]
            {
                10,
                6,
                8,
                12,
                14,
                8,
                6,
                10,
                2,
                2,
                2,
                2,
                2,
                2,
                2,
                2,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                0,
                3,
                3,
                3,
                3,
                3,
                3,
                3,
                3,
                11,
                7,
                9,
                13,
                15,
                9,
                7,
                11,
            };
        }

        private const ulong one = 1;

        private const ulong Row_1 = 0xFFFFFFFFFFFFFF00;
        private const ulong Row_2 = 0xFFFFFFFFFFFF00FF;
        private const ulong Row_7 = 0xFF00FFFFFFFFFFFF;
        private const ulong Row_8 = 0x00FFFFFFFFFFFFFF;
        private const ulong A_Line = 0xFEFEFEFEFEFEFEFE;
        private const ulong B_Line = 0xFDFDFDFDFDFDFDFD;
        private const ulong H_Line = 0x7F7F7F7F7F7F7F7F;
        private const ulong G_Line = 0xBFBFBFBFBFBFBFBF;

        public ulong GetMoves(int figure, int boardIndex)
        {
            ulong position = (ulong) 1 << boardIndex;
            ulong result = 0;

            //pawn
            if (figure <= 7)
            {
                //todo normal check for pawn`s trail
                //todo check for figure ahead
                //black 
                if ((figure & 1) == 0)
                {
                    result = (ulong) position >> 8;
                    if ((figure & 2) == 2) result |= position >> 16;
                    if (boardIndex - 7 < 64 && Board[boardIndex - 7] != 0 && (Board[boardIndex - 7] & (1 & figure)) == /*is opposite color*/ 0)
                        result |= (position >> 7 & A_Line); //left
                    if (boardIndex - 9 < 64 && Board[boardIndex - 9] != 0 && (Board[boardIndex - 9] & (1 & figure)) == 0)
                        result |= (position >> 9 & H_Line); //right
                }
                //white pawn
                else if ((figure & 1) == 1)
                {
                    result = (ulong) position << 8 | position << 16;
                    if ((figure & 2) == 2) result |= position << 16;
                    if (boardIndex + 7 < 64 && Board[boardIndex + 7] != 0 && (Board[boardIndex + 7] & (1 & figure)) == 0)
                        result |= (position << 7 & H_Line); //left
                    if (boardIndex + 9 < 64 && Board[boardIndex + 9] != 0 && (Board[boardIndex + 9] & (1 & figure)) == 0)
                        result |= (position << 9 & A_Line); //right
                }
            }
            //knight
            else if (figure <= 15)
            {
                result = (ulong)
                    (G_Line & H_Line & Row_1 & position << 6) |
                    (G_Line & H_Line & Row_8 & position >> 10) |
                    (H_Line & Row_1 & Row_2 & position << 15) |
                    (H_Line & Row_7 & Row_8 & position >> 17) |
                    (A_Line & Row_1 & Row_2 & position << 17) |
                    (A_Line & Row_7 & Row_8 & position >> 15) |
                    (A_Line & B_Line & Row_1 & position << 10) |
                    (A_Line & B_Line & Row_8 & position >> 6);
            }
            //bishop
            else if (figure <= 31)
            {
                int i = boardIndex;
                while (((one << i) & H_Line) != 0)
                {
                    i += 9;
                    if (((one << i) & Row_1) == 0) break;
                    result |= one << i;
                    if (Board[i] != 0) break;
                }

                i = boardIndex;
                while (((one << i) & A_Line) != 0)
                {
                    i += 7;
                    if (((one << i) & Row_1) == 0) break;
                    result |= one << i;
                    if (Board[i] != 0) break;

                }

                i = boardIndex;
                while (((one << i) & H_Line) != 0)
                {
                    i -= 7;
                    if (((one << i) & Row_8) == 0) break;
                    result |= one << i;
                    if (Board[i] != 0) break;

                }

                i = boardIndex;
                while (((one << i) & A_Line) != 0)
                {
                    i -= 9;
                    if (((one << i) & Row_8) == 0) break;
                    result |= one << i;
                    if (Board[i] != 0) break;
                }

            }
            //rook
            else if (figure <= 63)
            {
                int i = boardIndex;
                //up
                while (((one << i + 8) & Row_1) != 0)
                {
                    i += 8;
                    result |= one << i;
                    if (Board[i] != 0) break;
                }

                i = boardIndex;
                //right
                while (((one << i + 1) & A_Line) != 0)
                {
                    i += 1;
                    result |= one << i;
                    if (Board[i] != 0) break;

                }

                i = boardIndex;
                //left
                while (((one << i - 1) & H_Line) != 0)
                {
                    i -= 1;
                    result |= one << i;
                    if (Board[i] != 0) break;

                }

                i = boardIndex;
                //down
                while (((one << i - 8) & Row_8) != 0)
                {
                    i -= 8;
                    result |= one << i;
                    if (Board[i] != 0) break;
                }
            }
            for (int i = 0; i < 64; i++)
                if ((result & one << i) != 0 && (Board[i] & (1 & figure)) != 0)
                    //check if any posible move will land on ally
                    result ^= one << i;
            return result;

        }
        public bool CheckMove(byte from, byte to)
        {
            if (from < 64 && to < 64 && from != to)
            {
                var figure = Board[from];
                //if figure is empty
                //if (figure & 0xff) return false;
                //var moves = Ge;
            }
            return false;
        }
        public int turn = 0;

    }

    class Client
    {
        public Client(TcpClient Client)
        {
            string Request = "";
            byte[] Buffer = new byte[1024];
            int Count;
            while (Client.Connected && (Count = Client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                Request += Encoding.UTF8.GetString(Buffer, 0, Count);

                System.Console.Write(DateTime.Now.ToShortTimeString() + ": " + Request + "\nMe:");
                Request = "";
                var answer = Console.ReadLine();
                var answerBytes = Encoding.UTF8.GetBytes(answer);
                Client.GetStream().Write(answerBytes, 0, answerBytes.Length);

            }

            Client.Close();
        }
    }

    [Flags]
    public enum Figures
    {
        Black = 0,
        White = 1,
        Pawn = 4,
        Knight = 8,
        Bishop = 16,
        Rook = 32,
        Queen = 64,
        King = 128,
        PawnsTrail = 256
    }
    class Server
    {
        TcpListener Listener;

        public Server(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start();
            while (true)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ClientThread), Listener.AcceptTcpClient());
            }
        }

        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient) StateInfo);
        }

        ~Server()
        {
            if (Listener != null)
                Listener.Stop();

        }

        static void Main(string[] args)
        {
            //this prints long in 10101 form
            // for (int i = 0; i < 64; i++)
            // {
            //     fstr += (f & (ulong) 1 << i) == 0 ? "0" : "1";
            // }

            // var game = new Game();

            // game.Board = new int[64];
            // game.Board[17] = (int) (Figures.Pawn | Figures.Black);
            // game.Board[10] = (int) (Figures.Pawn | Figures.White);

            // var result = game.GetMoves((int) (Figures.Black | Figures.Rook), 9);
            // ulong count = 1;
            // for (int i = 0; i < 64; i++)
            // {
            //     if ((result & count << i) != 0) System.Console.WriteLine(i);
            // }

            // Console.ReadLine();
            int MaxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            ThreadPool.SetMinThreads(2, 2);
            new Server(22832);

            Console.ReadLine();
        }
    }
}