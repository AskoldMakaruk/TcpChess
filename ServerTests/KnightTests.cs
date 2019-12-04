using ChessServer;
using NUnit.Framework;
namespace Tests
{
    public class KnightTests : ServerTests
    {
        [Test]
        public void Knight_Mask_1()
        {
            var resultPosition = 0;
            TestGame[resultPosition] = (int) Figures.Knight;
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 17, 10));
        }

        [Test]
        public void Knight_Mask_2()
        {
            var resultPosition = 7;
            TestGame[resultPosition] = (int) Figures.Knight;
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 13, 22));
        }

        [Test]
        public void Knight_Mask_3()
        {
            var resultPosition = 56;
            TestGame[resultPosition] = (int) Figures.Knight;
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 41, 50));
        }

        [Test]
        public void Knight_Mask_4()
        {
            var resultPosition = 63;
            TestGame[resultPosition] = (int) Figures.Knight;
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 53, 46));
        }

        [Test]
        public void Knight_Movement()
        {
            var resultPosition = 27;
            TestGame[resultPosition] = (int) Figures.Knight;
            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 10, 12, 17, 21, 33, 37, 42, 44));
        }
    }
}