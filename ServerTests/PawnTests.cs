using ChessServer;
using NUnit.Framework;

namespace Tests
{
    public class PawnTests : ServerTests
    {
        #region Black Move
        [Test]
        public void Black_Pawn_Move_0()
        {
            TestGame[40] = (int) (Figures.Black | Figures.Pawn);
            var result = TestGame.GetMoves(40);
            Assert.True(MovesEqual(result, 32));
        }

        [Test]
        public void Black_Pawn_Move_1()
        {
            TestGame[48] = (int) (Figures.Black | Figures.Pawn);
            var result = TestGame.GetMoves(48);
            Assert.True(MovesEqual(result, 40, 32));
        }

        [Test]
        public void Black_Pawn_Move_2()
        {
            TestGame[48] = (int) (Figures.Black | Figures.Pawn);
            TestGame[32] = (int) (Figures.Black | Figures.Pawn);
            var result = TestGame.GetMoves(48);
            Assert.True(MovesEqual(result, 40));
        }

        [Test]
        public void Black_Pawn_Move_3()
        {
            TestGame[48] = (int) (Figures.Black | Figures.Pawn);
            TestGame[40] = (int) (Figures.Black | Figures.Pawn);
            var result = TestGame.GetMoves(48);
            Assert.True(MovesEqual(result));
        }
        #endregion
        #region White Move
        [Test]
        public void White_Pawn_Move_0()
        {
            TestGame[16] = (int) (Figures.White | Figures.Pawn);
            var result = TestGame.GetMoves(16);
            Assert.True(MovesEqual(result, 24));
        }

        [Test]
        public void White_Pawn_Move_1()
        {
            TestGame[8] = (int) (Figures.White | Figures.Pawn);
            var result = TestGame.GetMoves(8);
            Assert.True(MovesEqual(result, 16, 24));
        }

        [Test]
        public void White_Pawn_Move_2()
        {
            TestGame[8] = (int) (Figures.White | Figures.Pawn);
            TestGame[24] = (int) (Figures.White | Figures.Pawn);
            var result = TestGame.GetMoves(8);
            Assert.True(MovesEqual(result, new [] { 16 }));
        }

        [Test]
        public void White_Pawn_Move_3()
        {
            TestGame[8] = (int) (Figures.White | Figures.Pawn);
            TestGame[16] = (int) (Figures.White | Figures.Pawn);
            var result = TestGame.GetMoves(8);
            Assert.True(MovesEqual(result));
        }
        #endregion
        #region Black Attack
        [Test]
        public void Black_Pawn_Attack_Right()
        {
            TestGame[48] = (int) (Figures.Black | Figures.Pawn);

            TestGame[40] = (int) (Figures.White | Figures.Pawn);
            TestGame[41] = (int) (Figures.White | Figures.Pawn);
            TestGame[47] = (int) (Figures.White | Figures.Pawn);

            var result = TestGame.GetMoves(48);
            Assert.True(MovesEqual(result, 41));
        }

        [Test]
        public void Black_Pawn_Attack_Left()
        {
            TestGame[55] = (int) (Figures.Black | Figures.Pawn);

            TestGame[40] = (int) (Figures.White | Figures.Pawn);
            TestGame[46] = (int) (Figures.White | Figures.Pawn);
            TestGame[47] = (int) (Figures.White | Figures.Pawn);

            var result = TestGame.GetMoves(55);
            Assert.True(MovesEqual(result, 46));
        }

        [Test]
        public void Black_Pawn_En_Passant_Right()
        {
            TestGame[25] = (int) (Figures.Black | Figures.Pawn);
            TestGame[10] = (int) (Figures.White | Figures.Pawn);
            TestGame[17] = (int) (Figures.White | Figures.Pawn);
            TestGame.Move(10, 26);

            var result = TestGame.GetMoves(25);
            Assert.True(MovesEqual(result, 18));
        }

        [Test]
        public void Black_Pawn_En_Passant_Left()
        {
            TestGame[25] = (int) (Figures.Black | Figures.Pawn);
            TestGame[8] = (int) (Figures.White | Figures.Pawn);
            TestGame[17] = (int) (Figures.White | Figures.Pawn);
            TestGame.Move(8, 24);

            var result = TestGame.GetMoves(25);
            Assert.True(MovesEqual(result, 16));
        }

        #endregion
        #region White Attack
        [Test]
        public void White_Pawn_Attack_Right()
        {
            TestGame[8] = (int) (Figures.White | Figures.Pawn);
            TestGame[16] = (int) (Figures.Black | Figures.Pawn);

            TestGame[17] = (int) (Figures.Black | Figures.Pawn);
            TestGame[23] = (int) (Figures.Black | Figures.Pawn);

            var result = TestGame.GetMoves(8);
            Assert.True(MovesEqual(result, 17));
        }

        [Test]
        public void White_Pawn_Attack_Left()
        {
            TestGame[15] = (int) (Figures.White | Figures.Pawn);
            TestGame[16] = (int) (Figures.Black | Figures.Pawn);

            TestGame[22] = (int) (Figures.Black | Figures.Pawn);
            TestGame[23] = (int) (Figures.Black | Figures.Pawn);

            var result = TestGame.GetMoves(15);
            Assert.True(MovesEqual(result, 22));
        }

        [Test]
        public void White_Pawn_En_Passant_Right()
        {
            TestGame[50] = (int) (Figures.Black | Figures.Pawn);
            TestGame[41] = (int) (Figures.Black | Figures.Pawn);
            TestGame[33] = (int) (Figures.White | Figures.Pawn);
            TestGame.Move(50, 34);

            var result = TestGame.GetMoves(33);
            Assert.True(MovesEqual(result, 42));
        }

        [Test]
        public void White_Pawn_En_Passant_Left()
        {
            TestGame[48] = (int) (Figures.Black | Figures.Pawn);
            TestGame[41] = (int) (Figures.Black | Figures.Pawn);
            TestGame[33] = (int) (Figures.White | Figures.Pawn);
            TestGame.Move(48, 32);

            var result = TestGame.GetMoves(33);
            Assert.True(MovesEqual(result, 40));
        }
        #endregion
    }
}