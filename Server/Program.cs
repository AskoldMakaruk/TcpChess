using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ChessServer
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

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
        public static string ToPositions(this ulong moves)
        {
            var res = "";
            for (var i = 0; i < 64; i++)
            {
                if ((moves & (ulong) 1 << i) != 0)
                {
                    res += i + " ";
                }
            }
            return res;
        }
        static void Main(string[] args)
        {
            //this prints long in 10101 form
            // for (int i = 0; i < 64; i++)
            // {
            //     fstr += (f & (ulong) 1 << i) == 0 ? "0" : "1";
            // }

            var game = new Game() { Board = new int[64] };
            game.Board[37] = (int) (Figures.White | Figures.Pawn);
            game.Board[35] = (int) (Figures.White | Figures.Pawn);

            game.Board[52] = (int) (Figures.Black | Figures.Pawn);

            game.Move(52, 36);
            var result = game.GetMoves(35);
            System.Console.WriteLine(result.ToPositions());
            var moves = new List<int>();
            for (var i = 0; i < 64; i++)
            {
                if ((result & (ulong) 1 << i) != 0)
                {
                    moves.Add(i);
                    //System.Console.WriteLine(i);
                }

            }
            Image<Rgba32> Sprites;
            using(var file = File.OpenRead("sprites.png"))
            {
                var bytes = new byte[file.Length];
                file.Read(bytes, 0, (int) file.Length);
                Sprites = Image.Load(bytes);
            }

            var cellSide = 32;
            var size = 8;
            var image = new Image<Rgba32>(size * cellSide, size * cellSide);
            int x = 0, y = cellSide * (size - 1);
            var index = 0;
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    Color textColor;
                    Color color;
                    if ((j + i) % 2 == 0)
                    {
                        textColor = Rgba32.Black;
                        color = Rgba32.FromHex("#EBECD0");
                    }
                    else
                    {
                        textColor = Rgba32.WhiteSmoke;
                        color = Rgba32.FromHex("#779556");
                    }

                    var rectangle = new PointF[]
                    {
                        new PointF(x, y),
                        new PointF(x + cellSide, y),
                        new PointF(x + cellSide, y + cellSide),
                        new PointF(x, y + cellSide)

                    };
                    var fonts = new FontCollection();
                    var Arial = fonts.Install("Fonts/arial.ttf");

                    //draw cell
                    image.Mutate(cl => cl.FillPolygon(GraphicsOptions.Default, Brushes.Solid(color), rectangle));

                    //draw posible moves
                    if (moves.Contains(index))
                    {
                        var mask = Rgba32.Blue;
                        mask.A = 150;
                        image.Mutate(cl => cl.FillPolygon(GraphicsOptions.Default, Brushes.Solid(mask), rectangle));
                    }
                    //draw text
                    image.Mutate(cl => cl.DrawText(index.ToString(), new Font(Arial, 8, FontStyle.Regular), textColor, new PointF(x, y)));
                    //draw figures
                    var figure = game.Board[index];
                    if (figure != 0)
                    {
                        var cropX = 0;
                        for (var q = 0; q < 32; q++)
                        {
                            if ((figure & (2 << q)) != 0)
                            {
                                cropX = (6 - q) * 45;
                                System.Console.WriteLine(((Figures) figure).ToString());
                                break;
                            }
                        }
                        var cropR = new Rectangle(new Point(cropX, (figure + 1) % 2 * 45), new Size(45, 45));
                        var sprite = Sprites.Clone();
                        sprite.Mutate(cl => cl.Crop(cropR));
                        sprite.Mutate(cl => cl.Resize(new ResizeOptions() { Size = new Size(cellSide, cellSide) }));
                        image.Mutate(cl => cl.DrawImage(sprite, new Point(x, y), 1));
                    }

                    x += cellSide;
                    index++;
                }
                x = 0;
                y -= cellSide;
            }
            image.SaveAsJpeg(File.OpenWrite("image.jpeg"));
            // System.Console.WriteLine(game.GetCellsUnderAttack(Figures.White));
            // System.Console.WriteLine(game.GetCellsUnderAttack(Figures.White));

            return;
            Console.ReadLine();
            var MaxThreadsCount = Environment.ProcessorCount * 4;
            ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
            ThreadPool.SetMinThreads(2, 2);
            new Server(22832);

            Console.ReadLine();
        }
    }
}