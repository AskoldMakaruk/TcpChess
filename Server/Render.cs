using System.Collections.Generic;
using System.IO;
using System.Linq;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace ChessServer
{
    public static class Render
    {
        private static Image<Rgba32> _sprites;
        public static Image<Rgba32> Sprites
        {
            get
            {
                if (_sprites == null)
                {
                    var bytes = typeof(Render).Assembly.GetManifestResourceStream(Resources.First(c => c.Contains(".png")));
                    _sprites = Image.Load(bytes) as Image<Rgba32>;
                }
                return _sprites;
            }
        }

        private static readonly string[] Resources = typeof(Render).Assembly.GetManifestResourceNames();
        private static readonly FontCollection Fonts = new FontCollection();
        private static readonly FontFamily Arial = Fonts.Install(typeof(Render).Assembly.GetManifestResourceStream(Resources.First(c => c.Contains(".ttf"))));

        public static Image<Rgba32> RenderBoard(TestGame game, ulong possiblemoves = 0)
        {
            var moves = new List<int>();
            for (var i = 0; i < 64; i++)
            {
                if ((possiblemoves & (ulong) 1 << i) != 0)
                {
                    moves.Add(i);
                }
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
                        using var sprite = Sprites.Clone();
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
            return image;
        }
    }
}