using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Engine
{
    public static class EngineTester
    {
        public static int GetNodesAmount(string fen, int depth)
        {
            Game game = new Game(fen);
            return GetNodesAmount(game, depth);
        }

        public static int GetNodesAmount(Game game, int depth)
        {
            if (depth == 0) return 1;

            var moves = game.GetValidMoves();
            int numPositions = 0;

            foreach (var move in moves)
            {
                game.MakeMove(move);
                numPositions += GetNodesAmount(game, depth);
                game.UnMakeLastMove();
            }

            return numPositions;
        }
    }
}
