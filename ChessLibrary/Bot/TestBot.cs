using ChessLibrary.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Bot
{
    public class TestBot : ChessBot
    {
        const int PAWN_VALUE = 100;
        const int KNIGHT_VALUE = 300;
        const int BISHOP_VALUE = 300;
        const int ROOK_VALUE = 500;
        const int QUEEN_VALUE = 900;

        const int NEGATIVE_INFINITY = int.MinValue;
        const int POSITIVE_INFINITY = int.MaxValue;

        int depness = 4;

        public TestBot(Game game) : base(game)
        {
        }

        public override Move GetBotMove()
        {
            var moves = _game.GetValidMoves();

            Move? bestMove = null;
            int bestMoveValue = 0;

            foreach (var move in moves)
            {
                _game.MakeMove(move);
                int value = Search(depness - 1, NEGATIVE_INFINITY, POSITIVE_INFINITY);
                if(value > bestMoveValue)
                {
                    bestMove = move;
                    bestMoveValue = value;
                }
                _game.UnMakeLastMove();
            }

            return bestMove;
        }

        private int Search(int depth, int alpha, int beta)
        {
            if (depth == 0)
                return Evaluate();

            var moves = _game.GetValidMoves();

            if(moves.Count() == 0)
            {
                if(_game.IsCheck())
                {
                    return NEGATIVE_INFINITY;
                }
                return 0;
            }


            foreach (var move in moves)
            {
                _game.MakeMove(move);
                int evaluation = -Search(depth - 1, -beta, - alpha);
                _game.UnMakeLastMove();

                if(evaluation >= beta)
                {
                    //Oponent won t choose this move -> it is to good
                    return beta;
                }
                alpha = Math.Max(alpha, evaluation);
            }

            return alpha;
        }


        public override int Evaluate()
        {
            var points = GetPointsByPieces();

            int evaluation = points.white + points.black;

            int side = _game.GetCurrentlyPlayingSide() == ChessColors.WHITE ? 1 : -1;

            return evaluation * side;
        }

        private (int white, int black) GetPointsByPieces()
        {
            int white = 0;
            int black = 0;

            var piecesPos = _game.GetPiecePositions();

            foreach (var piece in piecesPos)
            {
                uint pieceCode = _game.GetCellCode(piece);
                int points = CountPiecePoints(pieceCode);
                if (points > 0)
                    white += points;
                else
                    black += points;
            }

            return (white, black);
        }


        private int CountPiecePoints(uint pieceCode)
        {
            var pieceClass = BoardEntityFactory.GetPieceClass(pieceCode);
            int points = 0;
            switch (pieceClass)
            {
                case PieceClasses.QUEEN:
                    points = QUEEN_VALUE;
                    break;
                case PieceClasses.ROOK:
                    points = ROOK_VALUE;
                    break;
                case PieceClasses.BISHOP:
                    points = BISHOP_VALUE;
                    break;
                case PieceClasses.PAWN:
                    points = PAWN_VALUE;
                    break;
                case PieceClasses.KNIGHT:
                    points = KNIGHT_VALUE;
                    break;
                default:
                    break;
            }

            return BoardEntityFactory.GetPieceColor(pieceCode) == ChessColors.WHITE ? points : -points;
        }

        
    }
}
