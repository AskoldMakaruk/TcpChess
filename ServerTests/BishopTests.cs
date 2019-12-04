using ChessServer;
using NUnit.Framework;
namespace Tests
{
    public class BishopTests : ServerTests
    {
        [Test]
        public void Bishop_Mask_1()
        {
            var resultPosition = 0;
            TestGame[resultPosition] = (int) (Figures.Bishop | Figures.White);
            TestGame[27] = (int) (Figures.Pawn | Figures.Black);
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 9, 18, 27));
        }

        [Test]
        public void Bishop_Mask_2()
        {
            var resultPosition = 14;
            TestGame[resultPosition] = (int) Figures.Bishop;
            TestGame[28] = (int) (Figures.Pawn | Figures.White);
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 5, 7, 21, 23, 28));
        }
    }
}