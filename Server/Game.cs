using System.Linq;

namespace ChessServer
{
    public class Game
    {
        public int this [int i] { get => Board[i]; set => Board[i] = value; }
        public Player BlackPlayer { get; set; }

        public Player WhitePlayer { get; set; }

        //00000000 - empty
        //0:   black 
        //1:   white       
        //4:   isPawn        
        //8:   isRook
        //16:  isKnight
        //32:  isBishop
        //64:  isQueen
        //128: isKing
        //256: isPawn`s trail
        public int[] Board { get; set; }

        public Game()
        {
            var row = new [] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var pawnRow = new [] { 4, 4, 4, 4, 4, 4, 4, 4 };
            var figuresRow = new [] { 8, 16, 32, 64, 128, 32, 16, 8 };
            var figures = new []
            {
                figuresRow,
                pawnRow,
                row,
                row,
                row,
                row,
                pawnRow.Select(c => c + 1).ToArray(),
                figuresRow.Select(c => c + 1).ToArray()
            };
            Board = new int[64];
            for (var i = 0; i < 8; i++)
                for (var j = 0; j < 8; j++)
                    Board[i * 8 + j] = figures[i][j];
        }

        public static readonly ulong one = 1;

        private const ulong Row_1 = 0xFFFFFFFFFFFFFF00;
        private const ulong Row_2 = 0xFFFFFFFFFFFF00FF;
        private const ulong Row_7 = 0xFF00FFFFFFFFFFFF;
        private const ulong Row_8 = 0x00FFFFFFFFFFFFFF;
        private const ulong A_Line = 0xFEFEFEFEFEFEFEFE;
        private const ulong B_Line = 0xFDFDFDFDFDFDFDFD;
        private const ulong H_Line = 0x7F7F7F7F7F7F7F7F;
        private const ulong G_Line = 0xBFBFBFBFBFBFBFBF;

        public ulong GetMoves(int boardIndex, bool posible = false)
        {
            var position = (ulong) 1 << boardIndex;
            ulong result = 0;
            var figure = (Figure) Board[boardIndex];

            if ((figure & Figures.Pawn) != 0)
            {
                //todo normal check for pawn`s trail
                //todo check for figure ahead
                //black 
                if ((figure & 1) == 0)
                {
                    result = position >> 8;
                    if ((figure & 2) == 2) result |= position >> 16;
                    if (boardIndex - 7 < 64 && Board[boardIndex - 7] != 0 && (Board[boardIndex - 7] & 1 & figure) == 0)
                        result |= (position >> 7 & A_Line); //left
                    if (boardIndex - 9 < 64 && Board[boardIndex - 9] != 0 && (Board[boardIndex - 9] & 1 & figure) == 0)
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

            if ((figure & (Figures.Rook | Figures.Queen)) != 0)
            {
                var i = boardIndex;
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

            if ((figure & Figures.Knight) != 0)
            {
                result =
                    (G_Line & H_Line & Row_1 & position << 6) |
                    (G_Line & H_Line & Row_8 & position >> 10) |
                    (H_Line & Row_1 & Row_2 & position << 15) |
                    (H_Line & Row_7 & Row_8 & position >> 17) |
                    (A_Line & Row_1 & Row_2 & position << 17) |
                    (A_Line & Row_7 & Row_8 & position >> 15) |
                    (A_Line & B_Line & Row_1 & position << 10) |
                    (A_Line & B_Line & Row_8 & position >> 6);
            }

            if ((figure & (Figures.Bishop | Figures.Queen)) != 0)
            {
                var i = boardIndex;
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

            for (var i = 0; i < 64; i++)
                if ((result & one << i) != 0 && Board[i] != 0 && ((1 & Board[i]) ^ (1 & figure)) == 0)
                    //check if any posible move will land on ally
                    result ^= one << i;
            return result;
        }

        public bool IsCellUnderAttack(int cellIndex, Figures colorOfAttacker, int[] board = null)
        {
            board = board == null?Board : board;
            var color = (ulong) colorOfAttacker;
            for (var i = 0; i < 64; i++)
            {
                if (Board[i] != 0)
                {
                    var moves = GetMoves(i);
                    if ((moves >> cellIndex & color) != 0)
                        return true;
                }
            }

            return false;
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
}