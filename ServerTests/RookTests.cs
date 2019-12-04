using ChessServer;
using NUnit.Framework;
namespace Tests
{
    public class RookTests : ServerTests
    {
        [Test]
        public void Rook_Moves_1()
        {
            var resultPosition = 0;
            TestGame[resultPosition] = (int) (Figures.Rook | Figures.White);
            TestGame[16] = (int) (Figures.Pawn | Figures.Black);
            TestGame[2] = (int) (Figures.Pawn | Figures.White);

            var result = TestGame.GetMoves(resultPosition);

            Assert.True(MovesEqual(result, 1, 8, 16));
        }

        [Test]
        public void Rook_Moves_2()
        {
            var testPos = 55;
            TestGame[testPos] = (int) (Figures.Rook | Figures.Black);
            TestGame[54] = (int) (Figures.Pawn | Figures.Black);
            TestGame[39] = (int) (Figures.Pawn | Figures.White);

            var result = TestGame.GetMoves(testPos);

            Assert.True(MovesEqual(result, 47, 39, 63));
        }
    }
}