using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public static class FenInterpreter
    {
        public static bool GetSetupFromFen(string fen, out ChessColors? currentSide, out List<BoardEntityFactory>? PieceClasses)
        {
            PieceClasses = null;
            currentSide = null;
            try
            {
                var toHandle = fen.Split(" ");
                PieceClasses = new List<BoardEntityFactory>();
                currentSide = toHandle[1] == "w" ? ChessColors.WHITE : ChessColors.BLACK;

                int currentY = 7;
                int currentX = 0;

                foreach (var c in toHandle[0])
                {
                    if (c == '/')
                    {
                        currentY--;
                        currentX = 0;
                        continue;
                    }
                    if (c >= 48 && c <= 57)
                    {
                        int offset = c - 48;
                        currentX += offset;
                        continue;
                    }

                    var piece = CharToPiece(c);
                    piece.SetPosition(currentX, currentY);
                    PieceClasses.Add(piece);
                    currentX++;
                }
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        private static BoardEntityFactory CharToPiece(char c)
        {
            BoardEntityFactory tmp = null;
            switch (c)
            {
                case 'r':
                    tmp = new BoardEntityFactory(PieceClasses.ROOK, ChessColors.BLACK, -1, -1);
                    break;
                case 'R':
                    tmp = new BoardEntityFactory(PieceClasses.ROOK, ChessColors.WHITE, -1, -1);
                    break;

                case 'n':
                    tmp = new BoardEntityFactory(PieceClasses.KNIGHT, ChessColors.BLACK, -1, -1);
                    break;
                case 'N':
                    tmp = new BoardEntityFactory(PieceClasses.KNIGHT, ChessColors.WHITE, -1, -1);
                    break;

                case 'b':
                    tmp = new BoardEntityFactory(PieceClasses.BISHOP, ChessColors.BLACK, -1, -1);
                    break;
                case 'B':
                    tmp = new BoardEntityFactory(PieceClasses.BISHOP, ChessColors.WHITE, -1, -1);
                    break;

                case 'q':
                    tmp = new BoardEntityFactory(PieceClasses.QUEEN, ChessColors.BLACK, -1, -1);
                    break;
                case 'Q':
                    tmp = new BoardEntityFactory(PieceClasses.QUEEN, ChessColors.WHITE, -1, -1);
                    break;

                case 'k':
                    tmp = new BoardEntityFactory(PieceClasses.KING, ChessColors.BLACK, -1, -1);
                    break;
                case 'K':
                    tmp = new BoardEntityFactory(PieceClasses.KING, ChessColors.WHITE, -1, -1);
                    break;

                case 'p':
                    tmp = new BoardEntityFactory(PieceClasses.PAWN, ChessColors.BLACK, -1, -1);
                    break;
                case 'P':
                    tmp = new BoardEntityFactory(PieceClasses.PAWN, ChessColors.WHITE, -1, -1);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return tmp;
        }
    }
}