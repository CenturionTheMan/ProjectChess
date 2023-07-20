using ChessLibrary.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public class Game
    {
        public List<Move> PlayedMoves { get; private set; }


        private int movesCount = 0;

        private MovesValidator movesValidator;

        private Board board;


        private ChessColors currentSide;

        //private int fiftyMovesRule = 0;

        #region CTOR

        public Game(string fen) : this()
        {
            throw new NotImplementedException();
            bool success = ChessStringsHandler.GetSetupFromFen(fen, out uint[] board, out ChessColors? currentSide, out bool[] castling, out int enPassantPosition, out int halfmoves, out int fullmove);
        }

        public Game(uint[] board, ChessColors? currentSide, bool[] castling, int enPassantPosition, int halfmoves, int fullmove) : this()
        {
            this.currentSide = currentSide.Value;


            for (int i = 0; i < board.Length; i++)
            {
                this.board.PlaceEntity(board[i], i);
            }
        }

        public Game()
        {
            board = new Board();
            PlayedMoves = new();
            movesValidator = new(this, board);
            BoardMovementRestrainer.ForceCTOR();
        }

        #endregion CTOR


        #region GETTERS/SETTERS

        public List<int> GetPiecePositions()
        {
            return board.GetPiecePositions();
        }

        public bool TryGetPieceAtPosition(int pos, out PieceClasses pieceClass, out ChessColors color)
        {
            return board.CheckIfCellHavePiece(pos, out pieceClass, out color);
        }

        public uint GetCellCode(int cellPos)
        {
            return board.GetCellCode(cellPos);
        }


        public Move[] GetValidMoves()
        {
            return movesValidator.GetValidMoves();
        }

        public ChessColors GetCurrentlyPlayingSide()
        {
            return currentSide;
        }

        #endregion GETTERS/SETTERS

        #region WORKERS

        private void ChangeCurrentSide()
        {
            currentSide = (currentSide == ChessColors.WHITE) ? ChessColors.BLACK : ChessColors.WHITE;
        }




        #endregion WORKERS



        public bool TryMakeMove(string moveString)
        {
            if (moveString == null || moveString.Length != 4)
            {
                return false;
            }
            moveString = moveString.ToLower();
            bool first = moveString[0] >= 97 && moveString[0] <= 104;
            bool second = moveString[1] >= 49 && moveString[1] <= 56;
            bool third = moveString[2] >= 97 && moveString[2] <= 104;
            bool fourth = moveString[3] >= 49 && moveString[3] <= 56;
            if (!first || !second || !third || !fourth)
            {
                return false;
            }

            int from, to, fromX, fromY, toX, toY;

            fromX = ChessStringsHandler.BoardLetterToNum(moveString[0]);
            fromY = moveString[1] - 48 - 1;
            toX = ChessStringsHandler.BoardLetterToNum(moveString[2]);
            toY = moveString[3] - 48 - 1;
            from = fromX + 8 * fromY;
            to = toX + 8 * toY;

            var validMoves = GetValidMoves();

            var res = Array.Find(validMoves,m => m.FromPos == from && m.ToPos == to);

            if (res == null)
            {
                return false;
            }
            else
            {
                MakeMove(res);
                return true;
            }
        }

        public void MakeMove(Move move)
        {
            if(move.AffectedToPos != null) //castling
            {

            }
            else if(move.AffectedFromPos != null) //capture
            {
                board.RemovePiece(move.AffectedFromPos.Value);
            }


            uint piece = board.GetCellCode(move.FromPos);
            board.RemovePiece(move.FromPos);
            board.PlaceEntity(piece, move.ToPos);

            ChangeCurrentSide();
        }

        //TEST!!!!
        public void UnMakeLastMove()
        {

            throw new NotImplementedException();

        }

         
    }

}
