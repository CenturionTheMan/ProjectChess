using ChessLibrary.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public class MovesValidator
    {
        private const int MAX_MOVES_AMOUNT = 218;
        private Game game;
        private Board board;

        public MovesValidator(Game game, Board board)
        {
            this.game = game;
            this.board = board;
        }

        public Move[] GetValidMoves()
        {
            throw new NotImplementedException();
        }

    }
}

