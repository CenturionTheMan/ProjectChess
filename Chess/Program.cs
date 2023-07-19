using .Engine;
using Chess.Gui;
using System.Reflection;

namespace Chess
{
    public class Program
    {
        private static ConsolePrinter printer = new ConsolePrinter(); 

        public static void Main()
        {
            //RunGame();
            RunDebug1();

        }

        private static int MoveGeneration(ChessBoard board, int depth)
        {
            if (depth == 0)
            {
                return 1;
            }
            var currentMoves = board.GetListOfValidMovesForCurrentSide();
            int movesAmount = 0;

            foreach (var move in currentMoves)
            {
                board.MakeMove(move);
                printer.PrintBoard(board);
                Console.ReadKey();
                printer.ClearConsole();
                movesAmount += MoveGeneration(board, depth - 1);
                board.UnMakeLastMove();
            }

            return movesAmount;
        }

        private static void RunDebug1()
        {
            ChessBoard board = new ChessBoard("7r/8/8/8/8/8/8/R3K1PR w");

            board.OnMoveMade += DebugMakeMove;
            board.OnLastMoveUnMade += DebugUnMakeMove;



            printer.DebugPrintValidMoves(board, false, false, PieceClasses.KING);

            Console.ReadLine();

            board.TryMakeMove("e1a1");
            board.UnMakeLastMove();
            board.TryMakeMove("g1g2");
            board.TryMakeMove("h8b8");
            bool result = board.TryMakeMove("e1a1"); //true
            board.UnMakeLastMove();
            board.UnMakeLastMove();
            board.TryMakeMove("h8c8");
            result = board.TryMakeMove("e1a1"); //false
            board.UnMakeLastMove();
            result = board.TryMakeMove("h8e8"); //false


            Console.WriteLine(result);
        }

        private static void DebugUnMakeMove(ChessBoard board)
        {
            //printer.DebugPrintValidMoves(board, false, false, PieceClasses.PAWN, PieceClasses.KING, PieceClasses.ROOK, PieceClasses.QUEEN, PieceClasses.KNIGHT, PieceClasses.BISHOP);
            printer.DebugPrintValidMoves(board, false, false, PieceClasses.KING);
            Console.ReadLine();
        }

        private static void DebugMakeMove(ChessBoard board)
        {
            printer.DebugPrintValidMoves(board, false, false, PieceClasses.KING);
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