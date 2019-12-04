using System.Collections.Generic;
using System.Linq;

//todo рокировка 
//todo превращение пешки в фигуру на выбор
//todo проверка на мат
//todo очередность ходов
//todo проверка на пат
//todo проверка на три одинаковых хода
//todo юнит тестирование всех комбинаций
namespace ChessServer
{
    public class TestGame
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

        public TestGame()
        {
            Moves = new List < (int From, int To) > ();
            var row = new [] { 0, 0, 0, 0, 0, 0, 0, 0 };
            var pawnRow = new [] { 4, 4, 4, 4, 4, 4, 4, 4 };
            var figuresRow = new [] { 8, 16, 32, 64, 128, 32, 16, 8 };
            var figures = new []
            {
                figuresRow.Select(c => c + 1).ToArray(),
                pawnRow.Select(c => c + 1).ToArray(),
                row,
                row,
                row,
                row,
                pawnRow,
                figuresRow,
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

        public List < (int From, int To) > Moves { get; set; }

        public ulong GetMoves(int boardIndex, bool posible = false, int[] board = null)
        {
            board = board ?? Board;
            var position = (ulong) 1 << boardIndex;
            ulong result = 0;
            var figure = (Figure) board[boardIndex];

            if ((figure & Figures.Pawn) != 0)
            {
                var(lastFrom, lastTo) = Moves.Count == 0 ? (0, 0) : Moves.Last();
                //black 
                if ((figure & 1) == 0)
                {
                    bool emptyAhead_1 = boardIndex - 8 > 0 && Board[boardIndex - 8] == 0;
                    bool emptyAhead_2 = boardIndex - 16 > 0 && Board[boardIndex - 16] == 0 && emptyAhead_1;

                    bool whiteFigureRight = boardIndex - 7 > 0 && Board[boardIndex - 7] != 0 &&
                        (board[boardIndex - 7] & 1) != (1 & figure);
                    bool whiteFigureLeft = boardIndex - 9 > 0 && board[boardIndex - 9] != 0 &&
                        (board[boardIndex - 9] & 1) != (1 & figure);

                    bool enPassant = lastTo - lastFrom == 16 && (board[lastTo] & (int) Figures.Pawn) != 0;

                    bool enPassantRight = enPassant && lastTo - boardIndex == 1;
                    bool enPassantLeft = enPassant && lastTo - boardIndex == -1;

                    if (emptyAhead_1) result = position >> 8;
                    if ((position & Row_7) == 0 && emptyAhead_2) result |= position >> 16;

                    if (whiteFigureRight || enPassantRight) result |= position >> 7 & A_Line;
                    if (whiteFigureLeft || enPassantLeft) result |= position >> 9 & H_Line;
                }
                //white pawn
                else if ((figure & 1) == 1)
                {
                    bool emptyAhead_1 = boardIndex + 8 < 64 && Board[boardIndex + 8] == 0;
                    bool emptyAhead_2 = boardIndex + 16 < 64 && Board[boardIndex + 16] == 0 && emptyAhead_1;

                    bool blackFigureRight = boardIndex + 9 < 64 && board[boardIndex + 9] != 0 &&
                        (board[boardIndex + 9] & 1) != (1 & figure);
                    bool blackFigureLeft = boardIndex + 7 < 64 && Board[boardIndex + 7] != 0 &&
                        (board[boardIndex + 7] & 1) != (1 & figure);

                    bool enPassant = lastTo - lastFrom == -16 && (board[lastTo] & (int) Figures.Pawn) != 0;

                    bool enPassantRight = enPassant && lastTo - boardIndex == 1;
                    bool enPassantLeft = enPassant && lastTo - boardIndex == -1;

                    if (emptyAhead_1) result = position << 8;
                    if ((position & Row_2) == 0 && emptyAhead_2) result |= position << 16;

                    if (blackFigureRight || enPassantRight) result |= result |= position << 9 & A_Line;
                    if (blackFigureLeft || enPassantLeft) result |= position << 7 & H_Line;
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
                    if (board[i] != 0) break;
                }

                i = boardIndex;
                //right
                while (((one << i + 1) & A_Line) != 0)
                {
                    i += 1;
                    result |= one << i;
                    if (board[i] != 0) break;
                }

                i = boardIndex;
                //left
                while (((one << i - 1) & H_Line) != 0)
                {
                    i -= 1;
                    result |= one << i;
                    if (board[i] != 0) break;
                }

                i = boardIndex;
                //down
                while (((one << i - 8) & Row_8) != 0)
                {
                    i -= 8;
                    result |= one << i;
                    if (board[i] != 0) break;
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
                    if (board[i] != 0) break;
                }

                i = boardIndex;
                while (((one << i) & A_Line) != 0)
                {
                    i += 7;
                    if (((one << i) & Row_1) == 0) break;
                    result |= one << i;
                    if (board[i] != 0) break;
                }

                i = boardIndex;
                while (((one << i) & H_Line) != 0)
                {
                    i -= 7;
                    if (((one << i) & Row_8) == 0) break;
                    result |= one << i;
                    if (board[i] != 0) break;
                }

                i = boardIndex;
                while (((one << i) & A_Line) != 0)
                {
                    i -= 9;
                    if (((one << i) & Row_8) == 0) break;
                    result |= one << i;
                    if (board[i] != 0) break;
                }
            }

            if ((figure & (Figures.King)) != 0)
            {
                result =
                    (Row_1 & position << 8) |
                    (Row_1 & H_Line & position << 7) |
                    (Row_1 & A_Line & position << 9) |
                    (A_Line & position << 1) |
                    (H_Line & position >> 1) |
                    (Row_8 & position >> 8) |
                    (Row_8 & A_Line & position >> 7) |
                    (Row_8 & H_Line & position >> 9);
            }

            for (var i = 0; i < 64; i++)
            {
                //check if any posible move will land on ally
                if ((result & one << i) != 0 && board[i] != 0 && ((1 & board[i]) ^ (1 & figure)) == 0)
                    result ^= one << i;

            }
            if (!posible)
                //check if king underAttack               
                result = KingUnderAttackMask(figure, result, boardIndex);

            return result;
        }
        public static int GetKingPosition(int[] board, Figures figure)
        {
            int kingPosition = 0;
            for (var i = 0; i < 64; i++)
            {
                if ((((Figures) board[i] & Figures.King) != 0) && (board[i] & 1) == (1 & (int) figure))
                {
                    kingPosition = i;
                    break;
                }
            }
            return kingPosition;
        }
        public bool Move(int from, int to)
        {
            var moves = GetMoves(from);
            if ((moves & (one << to)) != 0)
            {
                Board = NoMove(from, to);
                Moves.Add((from, to));
                return true;
            }
            return false;
        }
        public int[] NoMove(int from, int to)
        {
            var buffer = Board.Clone() as int[];

            buffer[to] = buffer[from];
            buffer[from] = 0;
            return buffer;
        }
        public ulong KingUnderAttackMask(Figures kingColor, ulong possibleMoves, int from)
        {
            var debug = possibleMoves.ToPositions();
            for (var i = 0; i < 64; i++)
                if ((possibleMoves & one << i) != 0)
                {
                    var board = NoMove(from, i);
                    ulong result = 0;
                    for (int j = 0; j < 64; j++)
                    {

                        if (((int) kingColor & 1) != (1 & board[j]))
                            result |= GetMoves(j, true, board);
                    }
                    debug = result.ToPositions();
                    if ((result & (one << GetKingPosition(board, kingColor))) != 0)
                    {
                        possibleMoves ^= one << i;
                    }
                    debug = possibleMoves.ToPositions();
                }

            return possibleMoves;
        }

        public int turn = 0;
    }
}