using System.IO;
using ChessServer;
using NUnit.Framework;
using SixLabors.ImageSharp;

namespace Tests
{
    public abstract class ServerTests
    {
        public const bool RenderAll = false;
        protected TestGame TestGame;

        [SetUp]
        public void Setup()
        {
            TestGame = new TestGame
            {
                Board = new int[64]
            };
        }
        public bool MovesEqual(ulong left, params int[] right)
        {
            ulong rightLong = 0;

#if DEBUG
            TestContext.WriteLine("LEFT: " + left.ToStringPositions());
            TestContext.WriteLine("RIGHT: " + string.Join(' ', right));
#endif
            for (int i = 0; i < right.Length; i++)
            {
                rightLong |= (ulong) 1 << right[i];
            }
            if (left != rightLong || RenderAll)
            {
                var image = Render.RenderBoard(TestGame, left);
                var dir = $"Images//{TestContext.CurrentContext.Test.ClassName}";
                Directory.CreateDirectory(dir);
                image.SaveAsPng(File.OpenWrite($"{dir}//{TestContext.CurrentContext.Test.Name}.png"));
            }
            return left == rightLong;
        }

        // [Test]
        public void MovesEqualTest()
        {
            ulong result = 1 << 24 | 1 << 17;
            Assert.True(MovesEqual(result, 17, 24));
        }

    }
}