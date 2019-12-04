using ChessServer;
using NUnit.Framework;
namespace Tests
{
    public class KingTests : ServerTests
    {
        [Test]
        public void King_Moves_0()
        {
            var resultPosition = 11;
            TestGame[resultPosition] = (int) (Figures.King | Figures.Black);
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 2, 3, 4, 10, 12, 18, 19, 20));
        }

        [Test]
        public void King_Moves()
        {
            var resultPosition = 11;
            TestGame[resultPosition] = (int) (Figures.King | Figures.Black);
            TestGame[3] = (int) (Figures.Rook | Figures.White);
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 3, 10, 12, 18, 20));
        }

    }
}