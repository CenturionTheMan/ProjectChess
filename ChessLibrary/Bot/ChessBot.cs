using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChessLibrary.Engine;

namespace ChessLibrary.Bot
{
    public abstract class ChessBot
    {
        protected Game _game;

        public ChessBot(Game game)
        {
            _game = game;
        }

        public abstract Move? GetBotMove();

        public abstract int Evaluate();
    }
}
