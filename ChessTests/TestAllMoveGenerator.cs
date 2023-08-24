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

            int movesAmount = Prefit(game, depth);
            return movesAmount;
        }

        private static int Prefit(Game game, int depth)
        {
            var currentMoves = game.GetValidMoves();
            int nMoves = currentMoves.Length;
            int totalNodes = 0;

            if (depth == 1)
            {
                return nMoves;
            }


            foreach (var move in currentMoves)
            {
                game.MakeMove(move);
                totalNodes += Prefit(game, depth - 1);
                game.UnMakeLastMove();
            }

            return totalNodes;
        }

    }
}
