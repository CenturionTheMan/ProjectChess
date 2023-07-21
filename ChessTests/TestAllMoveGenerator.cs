using ChessLibrary.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTests
{
    public static class TestAllMoveGenerator
    {
        public static int TestPositionMoveAmount(string fen, int depth)
        {
            Game game = new Game(fen);

            int movesAmount = MoveGeneration(game, depth);
            return movesAmount;
        }

        private static int MoveGeneration(Game game, int depth)
        {
            if (depth == 0)
            {
                return 1;
            }
            var currentMoves = game.GetValidMoves();
            int movesAmount = 0;

            foreach (var move in currentMoves)
            {
                game.MakeMove(move);
                movesAmount += MoveGeneration(game, depth - 1);
                game.UnMakeLastMove();
            }

            return movesAmount;
        }
    }
}
