using ChessLibrary.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public class Game
    {
        public Action<ChessColors> OnCurrentPlayerChanged;

        public Stack<Move> PlayedMoves { get; private set; }

        int halfmoves, fullmoves;
        
        private MovesValidator movesValidator;

        private Board board;


        private ChessColors currentSide;

        private int enPassantPosition = -1;

        private bool[] castling;
        private int castlingChanged = 0;

        private Move[] currentValidMoves = null;

        #region CTOR

        public Game(string fen) : this()
        {
            bool success = ChessStringsHandler.GetSetupFromFen(fen, out uint[] board, out ChessColors? currentSide, out castling, out enPassantPosition, out halfmoves, out fullmoves);
            if (success == false) throw new Exception();

            this.currentSide = currentSide.Value;

            for (int i = 0; i < board.Length; i++)
            {
                this.board.PlaceEntity(board[i], i);
            }

            if (castling[0] == false) castlingChanged++;
            if (castling[1] == false) castlingChanged++;
            if (castling[2] == false) castlingChanged++;
            if (castling[3] == false) castlingChanged++;
            
            OnCurrentPlayerChanged?.Invoke(currentSide.Value);
        }


        //castling[0] = toHandle[2].Contains("K");
        //castling[1] = toHandle[2].Contains("Q");
        //castling[2] = toHandle[2].Contains("k");
        //castling[3] = toHandle[2].Contains("q");
        public Game(uint[] board, ChessColors? currentSide, bool[] castling, int enPassantPosition, int halfmoves, int fullmoves) : this()
        {
            this.halfmoves = halfmoves;
            this.fullmoves = fullmoves;
            this.currentSide = currentSide.Value;
            this.enPassantPosition = enPassantPosition;
            this.castling = castling;
            for (int i = 0; i < board.Length; i++)
            {
                this.board.PlaceEntity(board[i], i);
            }

            if (castling[0] == false) castlingChanged++;
            if (castling[1] == false) castlingChanged++;
            if (castling[2] == false) castlingChanged++;
            if (castling[3] == false) castlingChanged++;

            OnCurrentPlayerChanged?.Invoke(currentSide.Value);
        }

        private Game()
        {
            board = new Board();
            PlayedMoves = new();
            movesValidator = new(this, board);
            BoardMovementRestrainer.ForceCTOR();
        }

        #endregion CTOR


        #region GETTERS/SETTERS

        public bool[] GetCastling()
        {
            if (castlingChanged >= 4) return null;
            return castling;
        }

        public bool[] GetEnemyAttackZone() { return movesValidator.GetEnemyAttackZone(); }

        public bool IsEnPassantPosition(out int position)
        {
            position = enPassantPosition;
            return position >= 0;
        }

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
            if(currentValidMoves == null) currentValidMoves = movesValidator.GetValidMoves();
            return currentValidMoves;
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
            OnCurrentPlayerChanged?.Invoke(currentSide);
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
            if(move.TryGetAffectedPiecePos(out int affectedFromPos, out int? affectedToPos))
            {
                if (affectedToPos != null) //castling
                {
                    uint rook = board.GetCellCode(affectedFromPos);
                    board.RemovePiece(affectedFromPos);
                    board.PlaceEntity(rook, affectedToPos.Value);
                }
                else //capture
                {
                    board.RemovePiece(affectedFromPos);
                }
            }

            foreach (var i in move.GetCastlingArrayIndex())
            {
                this.castling[i] = false;
                castlingChanged++;
            }
            //if (castlingChanged >= 4) castling = null;

            this.enPassantPosition = move.GetEnPassantPosition();

            uint piece = board.GetCellCode(move.FromPos);
            board.RemovePiece(move.FromPos);
            
            if(move.TryGetPromotion(out PieceClasses newClass))
            {
                board.PlacePiece(newClass, currentSide, move.ToPos);
            }
            else
                board.PlaceEntity(piece, move.ToPos);

            PlayedMoves.Push(move);

            ChangeCurrentSide();

            currentValidMoves = movesValidator.GetValidMoves();
        }

        //TEST!!!!
        public void UnMakeLastMove()
        {
            if (PlayedMoves.Count <= 0) return;

            Move move = PlayedMoves.Pop();

            uint piece = board.GetCellCode(move.ToPos);
            board.RemovePiece(move.ToPos);

            if (move.TryGetPromotion(out PieceClasses newClass))
            {
                var color = BoardEntityFactory.GetPieceColor(piece);
                board.PlacePiece(PieceClasses.PAWN, color, move.FromPos);
            }
            else
            {
                board.PlaceEntity(piece, move.FromPos);
            }

            this.enPassantPosition = move.GetEnPassantPosition();

            //if (PlayedMoves.Count == 1) this.enPassantPosition = -1;

            var movCastling = move.GetCastlingArrayIndex();
            foreach (var i in movCastling)
            {
                castling[i] = true;
                castlingChanged--;
            }


            if (move.TryGetAffectedPiecePos(out int affectedFromPos, out int? affectedToPos))
            {
                if (affectedToPos != null) //castling
                {
                    uint rook = board.GetCellCode(affectedToPos.Value);
                    board.RemovePiece(affectedToPos.Value);
                    board.PlaceEntity(rook, affectedFromPos);
                }
                else //capture
                {
                    board.PlaceEntity(move.GetAffectedPiece(), affectedFromPos);
                }
            }

            ChangeCurrentSide();

            currentValidMoves = movesValidator.GetValidMoves();
        }


    }

}
