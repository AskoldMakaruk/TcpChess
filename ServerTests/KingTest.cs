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

        [Test]
        public void King_Defense()
        {
            TestGame[11] = (int) (Figures.King | Figures.Black);
            TestGame[20] = (int) (Figures.Queen | Figures.Black);

            TestGame[13] = (int) (Figures.Rook | Figures.White);

            Assert.True(MovesEqual(TestGame.GetMoves(20), 12, 13));
            Assert.True(MovesEqual(TestGame.GetMoves(11), 2, 3, 4, 18, 19));
        }

    }
}