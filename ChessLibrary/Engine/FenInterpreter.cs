using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public static class FenInterpreter
    {
        public static bool GetSetupFromFen(string fen, out ChessColors? currentSide, out List<ChessPiece>? pieces)
        {
            pieces = null;
            currentSide = null;
            try
            {
                var toHandle = fen.Split(" ");
                pieces = new List<ChessPiece>();
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
                    pieces.Add(piece);
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

        private static ChessPiece CharToPiece(char c)
        {
            ChessPiece tmp = null;
            switch (c)
            {
                case 'r':
                    tmp = new ChessPiece(PieceClasses.ROOK, ChessColors.BLACK, -1, -1);
                    break;
                case 'R':
                    tmp = new ChessPiece(PieceClasses.ROOK, ChessColors.WHITE, -1, -1);
                    break;

                case 'n':
                    tmp = new ChessPiece(PieceClasses.KNIGHT, ChessColors.BLACK, -1, -1);
                    break;
                case 'N':
                    tmp = new ChessPiece(PieceClasses.KNIGHT, ChessColors.WHITE, -1, -1);
                    break;

                case 'b':
                    tmp = new ChessPiece(PieceClasses.BISHOP, ChessColors.BLACK, -1, -1);
                    break;
                case 'B':
                    tmp = new ChessPiece(PieceClasses.BISHOP, ChessColors.WHITE, -1, -1);
                    break;

                case 'q':
                    tmp = new ChessPiece(PieceClasses.QUEEN, ChessColors.BLACK, -1, -1);
                    break;
                case 'Q':
                    tmp = new ChessPiece(PieceClasses.QUEEN, ChessColors.WHITE, -1, -1);
                    break;

                case 'k':
                    tmp = new ChessPiece(PieceClasses.KING, ChessColors.BLACK, -1, -1);
                    break;
                case 'K':
                    tmp = new ChessPiece(PieceClasses.KING, ChessColors.WHITE, -1, -1);
                    break;

                case 'p':
                    tmp = new ChessPiece(PieceClasses.PAWN, ChessColors.BLACK, -1, -1);
                    break;
                case 'P':
                    tmp = new ChessPiece(PieceClasses.PAWN, ChessColors.WHITE, -1, -1);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return tmp;
        }
    }
}