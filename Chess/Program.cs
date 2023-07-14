using Chess.Engine;
using Chess.Gui;
using System.Reflection;

namespace Chess
{
    public class Program
    {
        public static void Main()
        {
            //RunGame();
            RunDebug();
        }

        private static void RunDebug()
        {
            ChessBoard board = new ChessBoard("r3k3/8/8/8/8/3q3p/7R/4K3 w");
            //ChessBoard board = new ChessBoard("8/8/8/8/8/8/8/4K3 w");
            ConsolePrinter printer = new ConsolePrinter();


            printer.DebugPrintValidMoves(board, PieceClasses.PAWN, PieceClasses.KING, PieceClasses.ROOK, PieceClasses.QUEEN);
            Console.ReadLine();

            board.ChangeCurrentlyPlayingSide();
            DebugDoMove(board, printer, "d3e3");

        }
        private static void DebugDoMove(ChessBoard board, ConsolePrinter printer, string move)
        {
            board.TryMakeMove(move);
            printer.DebugPrintValidMoves(board, PieceClasses.PAWN, PieceClasses.KING, PieceClasses.ROOK, PieceClasses.QUEEN);
            Console.ReadLine();
        }


        private static void RunGame()
        {
            ChessBoard board = new ChessBoard("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w");
            ConsolePrinter printer = new ConsolePrinter();

            bool wasSuccess = true;
            while (true)
            {
                var userInput = printer.PrintGameGui(board, !wasSuccess);
                wasSuccess = board.TryMakeMove(userInput);
                printer.ClearConsole();
            }
        }
    }
}