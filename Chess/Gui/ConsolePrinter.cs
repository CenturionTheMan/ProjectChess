using Chess.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Gui
{
    public class ConsolePrinter
    {
        private const string LINE_HORIZONTAL = "=";
        private const string LINE_VERTICAL = "||";
        private const string EMPTY_CELL = "   ";

        private const string PIECE_PAWN = " P ";
        private const string PIECE_ROOK = " R ";
        private const string PIECE_KNIGHT = " N ";
        private const string PIECE_BISHOP = " B ";
        private const string PIECE_QUEEN = " Q ";
        private const string PIECE_KING = " K ";

        private const ConsoleColor BLACK_CELL = ConsoleColor.DarkGray;
        private const ConsoleColor WHITE_CELL = ConsoleColor.DarkYellow;

        private const ConsoleColor BLACK_PIECE = ConsoleColor.Black;
        private const ConsoleColor WHITE_PIECE = ConsoleColor.White;

        public string PrintGameGui(ChessBoard board, bool wasPrevMoveWrong)
        {
            PrintBoard(board);
            if(wasPrevMoveWrong)
            {
                Console.WriteLine(EMPTY_CELL + "CHOOSEN MOVE WAS INVALID!");
            }
            string userInput = PrintUserOptions(board);
            return userInput;
        }


        private void PrintHorizontalLine(int len, int offset)
        {
            Console.ResetColor();

            for (int i = 0; i < offset; i++)
            {
                Console.Write(" ");
            }

            for (int i = 0; i < len; i++)
            {
                Console.Write(LINE_HORIZONTAL);
            }
            Console.Write("\n");
        }

        public void DebugPrintValidMoves(ChessBoard board, params PieceClasses[] classesToShow)
        {
            ClearConsole();   
            (var whiteMoves, var blackMoves) = board.GetListOfValidMoves();

            var currentMoves = (board.CurrentSide == ChessColors.WHITE) ? whiteMoves : blackMoves;
            var sewed = currentMoves.FindAll(m => classesToShow.Contains(board.GetCell(m.FromPos).GetPiece().PieceClass));


            int size = ChessBoard.BOARD_SIZE;
            for (int y = size - 1; y >= 0; y--)
            {
                Console.ResetColor();
                Console.Write(EMPTY_CELL + $"{y + 1} ");

                for (int x = 0; x < size; x++)
                {
                    ConsoleColor color = ((x + y) % 2 == 1) ? WHITE_CELL : BLACK_CELL;

                    var result = sewed.Find(i => i.ToPos.X == x && i.ToPos.Y == y);
                    if(result != null)
                        color = ConsoleColor.Green;

                        Console.BackgroundColor = color;

                    if (board.GetCell(x, y).HasPiece(out ChessPiece? piece))
                    {
                        ConsoleColor pieceColor = (piece.PieceColor == ChessColors.WHITE) ? WHITE_PIECE : BLACK_PIECE;


                        Console.ForegroundColor = pieceColor;
                        Console.Write(ParsePieceClassToString(piece));
                    }
                    else
                    {
                        Console.Write(EMPTY_CELL);
                    }
                }

                if (y == 7)
                {
                    Console.ResetColor();
                    Console.Write(EMPTY_CELL + "CURENT SIDE: ");
                    var tmp = (board.CurrentSide == ChessColors.WHITE) ? "WHITE" : "BLACK";
                    Console.ForegroundColor = (board.CurrentSide == ChessColors.WHITE) ? WHITE_CELL : BLACK_CELL;
                    Console.Write(tmp);
                }

                if (y == 6)
                {
                    Console.ResetColor();
                    Console.Write(EMPTY_CELL + "LAST MOVE:   ");
                    Console.ForegroundColor = (board.CurrentSide != ChessColors.WHITE) ? WHITE_CELL : BLACK_CELL;
                    Move? lastMove = (board.PlayedMoves.Count() > 0) ? board.PlayedMoves.Last() : null;
                    string lastMoveStr = (lastMove == null) ? "NONE" : IntToLetter(lastMove.FromPos.X) + (lastMove.FromPos.Y + 1).ToString() + IntToLetter(lastMove.ToPos.X) + (lastMove.ToPos.Y + 1).ToString();
                    lastMoveStr = lastMoveStr.Replace(" ", "");
                    Console.Write(lastMoveStr);

                }

                Console.Write("\n");

            }

            Console.ResetColor();


            Console.Write("  " + EMPTY_CELL);
            for (int i = 0; i < size; i++)
            {
                Console.Write(IntToLetter(i));
            }

            Console.ResetColor();
            Console.Write("\n");
        }

        private void PrintBoard(ChessBoard board)
        {
            int width = ChessBoard.BOARD_SIZE;
            int height = ChessBoard.BOARD_SIZE;


            for (int y = height - 1; y >= 0; y--)
            {
                Console.ResetColor();
                Console.Write(EMPTY_CELL + $"{y + 1} ");

                for (int x = 0; x < width; x++)
                {
                    ConsoleColor color = ((x + y) % 2 == 1) ? WHITE_CELL : BLACK_CELL;
                    Console.BackgroundColor = color;
                    if (board.GetCell(x,y).HasPiece(out ChessPiece? piece))
                    {
                        ConsoleColor pieceColor = (piece.PieceColor == ChessColors.WHITE) ? WHITE_PIECE : BLACK_PIECE;
                        Console.ForegroundColor = pieceColor;
                        Console.Write(ParsePieceClassToString(piece));
                    }
                    else
                    {
                        Console.Write(EMPTY_CELL);
                    }
                }
                
                if(y == 7)
                {
                    Console.ResetColor();
                    Console.Write(EMPTY_CELL + "CURENT SIDE: ");
                    var tmp = (board.CurrentSide == ChessColors.WHITE) ? "WHITE" : "BLACK";
                    Console.ForegroundColor = (board.CurrentSide == ChessColors.WHITE) ? WHITE_CELL : BLACK_CELL;
                    Console.Write(tmp);
                }

                if (y == 6)
                {
                    Console.ResetColor();
                    Console.Write(EMPTY_CELL + "LAST MOVE:   ");
                    Console.ForegroundColor = (board.CurrentSide != ChessColors.WHITE) ? WHITE_CELL : BLACK_CELL;
                    Move? lastMove =(board.PlayedMoves.Count() > 0)? board.PlayedMoves.Last() : null;
                    string lastMoveStr = (lastMove == null)? "NONE" : IntToLetter(lastMove.FromPos.X) + (lastMove.FromPos.Y+ 1).ToString() + IntToLetter(lastMove.ToPos.X) + (lastMove.ToPos.Y + 1).ToString();
                    lastMoveStr = lastMoveStr.Replace(" ", "");
                    Console.Write(lastMoveStr);

                }


                Console.Write("\n");
            }
            Console.ResetColor();
            
           
            Console.Write("  " + EMPTY_CELL);
            for (int i = 0; i < width; i++)
            {
                Console.Write(IntToLetter(i));
            }

            Console.ResetColor();
            Console.Write("\n");
        }

        public string PrintUserOptions(ChessBoard board)
        {
            string? input ="";
            do
            {
                Console.Write("   CHOOSE MOVE: ");
                input = Console.ReadLine();

                if (input != null && input.Length == 4 )
                {
                    input = input.ToLower();

                    bool first = input[0] >= 97 && input[0] <= 104;
                    bool second = input[1] >= 49 && input[1] <= 56;
                    bool third = input[2] >= 97 && input[2] <= 104;
                    bool fourth = input[3] >= 49 && input[3] <= 56;

                    if(first && second && third && fourth)
                    {
                        return input;
                    }
                }

                ClearConsole();
                PrintBoard(board);

                Console.WriteLine("   WRONG MOVE FORMAT!. MUST BE: position from - position to. EXAMPLE: b8c6");
            } while (true);
        }

        private string IntToLetter(int i)
        {
            string res = i switch
            {
                0 => " a ",
                1 => " b ",
                2 => " c ",
                3 => " d ",
                4 => " e ",
                5 => " f ",
                6 => " g ",
                7 => " h ",
                _ => throw new NotImplementedException()
            };
            return res;
        }

        private string ParsePieceClassToString(ChessPiece piece)
        {
            string toRet = piece.PieceClass switch
            {
                PieceClasses.PAWN => PIECE_PAWN,
                PieceClasses.BISHOP => PIECE_BISHOP,
                PieceClasses.KNIGHT => PIECE_KNIGHT,
                PieceClasses.ROOK => PIECE_ROOK,
                PieceClasses.QUEEN => PIECE_QUEEN,
                PieceClasses.KING => PIECE_KING,
                _ => throw new NotImplementedException()
            };
            return toRet;
        }

        public void ClearConsole()
        {
            Console.Clear();
        }

    }
}
