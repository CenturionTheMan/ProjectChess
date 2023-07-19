using ChessLibrary.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public class ChessBoard
    {
        public Action<ChessPiece>? OnPieceCaptured;
        public Action<ChessPiece>? OnPawnPromotion;
        public Action<ChessPiece>? OnPawnPromotionReversed;
        public Action? OnCheck;
        public Action? OnCheckMate;
        public Action? OnEnPassantCapture;
        public Action? OnCastle;
        public Action<ChessBoard>? OnMoveMade;
        public Action<ChessBoard>? OnLastMoveUnMade;

        public const int BOARD_SIZE = 8;


        public ChessColors CurrentSide { get; private set; }

        public List<ChessPiece> ChessPieces { get; private set; }

        public List<Move> PlayedMoves { get; private set; }

        public GameResult CurrentGameResult { get; private set; }

        private MovesValidator movesValidator;

        private BoardCell[,] boardCells;

        private Dictionary<ChessPiece, Move> movedPieces;

        private List<Move>? validMovesWhite = null, validMovesBlack = null;

        private int fiftyMovesRule = 0;

        #region CTOR

        public ChessBoard(string fen) : this()
        {
            bool success = FenInterpreter.GetSetupFromFen(fen, out ChessColors? currentSide, out List<ChessPiece> chessPieces);
            CurrentSide = currentSide.Value;
            PlacePiecesAfterInit(chessPieces);
        }

        public ChessBoard(ChessColors currentSide, params ChessPiece[] chessPieces) : this()
        {
            CurrentSide = currentSide;
            PlacePiecesAfterInit(new(chessPieces));
        }

        public ChessBoard(ChessColors currentSide, List<ChessPiece> chessPieces) : this()
        {
            CurrentSide = currentSide;
            PlacePiecesAfterInit(chessPieces);
        }

        public ChessBoard()
        {
            ChessPieces = new List<ChessPiece>();
            PlayedMoves = new();
            movesValidator = new(this);
            movedPieces = new();
            CurrentGameResult = GameResult.NONE;

            boardCells = new BoardCell[BOARD_SIZE, BOARD_SIZE];
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    boardCells[i, j] = new BoardCell(i, j);
                }
            }
        }

        #endregion CTOR

        #region INIT

        private void PlacePiecesAfterInit(List<ChessPiece> pieces)
        {
            foreach (var piece in pieces)
            {
                PlacePieceAtPostion(piece, piece.Position);
            }
        }

        #endregion INIT

        #region GETTERS/SETTERS

        public bool HasPieceMoved(ChessPiece piece)
        {
            return movedPieces.ContainsKey(piece);
        }

        public bool HasPieceMoved(ChessPiece piece, out Move move)
        {
            bool res = movedPieces.TryGetValue(piece, out move);
            return res;
        }

        public BoardCell? GetCell(int x, int y)
        {
            if (CheckIfCordsAreValid(x, y))
                return boardCells[x, y];
            else
                return null;
        }

        public BoardCell? GetCell(Vec2 pos)
        {
            return GetCell(pos.X, pos.Y);
        }


        public (List<Move> validMovesWhite, List<Move> validMovesBlack) GetListOfValidMoves()
        {
            if (validMovesWhite == null || validMovesBlack == null)
            {
                (validMovesWhite, validMovesBlack) = movesValidator.GetValidMoves();
            }
            return (validMovesWhite, validMovesBlack);
        }

        public List<Move> GetListOfValidMovesForCurrentSide()
        {
            (validMovesWhite, validMovesBlack) = GetListOfValidMoves();
            return (CurrentSide == ChessColors.WHITE) ? validMovesWhite : validMovesBlack;
        }

        #endregion GETTERS/SETTERS

        #region WORKERS
        private bool CheckIfCordsAreValid(int x, int y)
        {
            return x < 0 || y < 0 || x >= 8 || y >= 8 ? false : true;
        }

        private bool CheckIfCordsAreValid(Vec2 pos)
        {
            return CheckIfCordsAreValid(pos.X, pos.Y);
        }

        public void ChangeCurrentlyPlayingSide()
        {
            CurrentSide = CurrentSide == ChessColors.WHITE ? ChessColors.BLACK : ChessColors.WHITE;
        }

        private void PlacePieceAtPostion(ChessPiece piece, Vec2 position)
        {
            PlacePieceAtPostion(piece, position.X, position.Y);
        }

        private void PlacePieceAtPostion(ChessPiece piece, int x, int y)
        {
            if (!CheckIfCordsAreValid(x, y)) return;

            //if (boardCells[x, y].HasPiece())
            //{
            //    return;
            //}
            boardCells[x, y].SetPiece(piece);
            piece.SetPosition(x, y);
            ChessPieces.Add(piece);
            return;
        }

        private bool RemovePieceAtPosition(Vec2 pos)
        {
            if (!CheckIfCordsAreValid(pos)) return false;

            var removedPiece = boardCells[pos.X, pos.Y].RemovePiece();
            if (removedPiece == null) return false;

            //removedPiece.SetPosition(-1, -1);
            _ = ChessPieces.Remove(removedPiece);

            return true;
        }


        private int LetterToNum(char c)
        {
            int toRet = c switch
            {
                'a' => 0,
                'b' => 1,
                'c' => 2,
                'd' => 3,
                'e' => 4,
                'f' => 5,
                'g' => 6,
                'h' => 7,
                _ => throw new NotImplementedException()
            };
            return toRet;
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

            int fromX, fromY, toX, toY;

            fromX = LetterToNum(moveString[0]);
            fromY = moveString[1] - 48 - 1;
            toX = LetterToNum(moveString[2]);
            toY = moveString[3] - 48 - 1;


            (var white, var black) = GetListOfValidMoves();

            var current = (CurrentSide == ChessColors.WHITE)? white : black;

            var res = current.Find(m => m.FromPos.Equals(fromX, fromY) && m.GetTriggerPos().Equals(toX, toY));

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

        public GameResult MakeMove(Move move)
        {
            if(CurrentGameResult != GameResult.NONE)
            {
                return CurrentGameResult;
            }

            move.FiftyMoveRuleToThisMove = fiftyMovesRule;
            fiftyMovesRule++;

            var fromCell = GetCell(move.FromPos);
            _ = fromCell.HasPiece(out ChessPiece pieceToMove);

            if (pieceToMove.PieceClass == PieceClasses.PAWN)
            {
                fiftyMovesRule = 0;
            }

            if(move.IsEnPassantCapture)
            {
                OnEnPassantCapture?.Invoke();
            }

            if (move.IsCastling) //CASTLING
            {
                OnCastle?.Invoke();

                bool isShort = Math.Abs(move.OtherPiece.Position.X - move.ToPos.X) == 1;

                Vec2 rookNewPos = (isShort) ? move.ToPos + Vec2.Left : move.ToPos + Vec2.Right;
                _ = RemovePieceAtPosition(move.OtherPiece.Position);
                PlacePieceAtPostion(move.OtherPiece, rookNewPos);
            }
            else //CHECK IF CAPTURE
            {
                var pieceOnTarget = move.OtherPiece;

                if (pieceOnTarget != null)
                {
                    OnPieceCaptured?.Invoke(pieceOnTarget);


                    fiftyMovesRule = 0;
                    _ = RemovePieceAtPosition(pieceOnTarget.Position);
                    
                    CheckDrawInsufficientMaterial();
                }

            }

            if (move.IsPromotion) //PROMOTION
            {
                //TMP
                pieceToMove.PieceClass = PieceClasses.QUEEN;

                OnPawnPromotion?.Invoke(pieceToMove);
            }

            _ = RemovePieceAtPosition(fromCell.Position);
            PlacePieceAtPostion(pieceToMove, move.ToPos);
            move.OrderInPlayedMoves = PlayedMoves.Count;
            PlayedMoves.Add(move);
            _ = movedPieces.TryAdd(pieceToMove, move);


            (this.validMovesWhite, this.validMovesBlack) = movesValidator.GetValidMoves();



            CheckFiftyMoveRule();
            CheckCheckMateAndStalemate();


            ChangeCurrentlyPlayingSide();

            OnMoveMade?.Invoke(this);

            return CurrentGameResult;
        }

        //TEST!!!!
        public GameResult UnMakeLastMove()
        {

            if (PlayedMoves.Count == 0)
            {
                return CurrentGameResult;
            }
            CurrentGameResult = GameResult.NONE;

            var lastMove = PlayedMoves.Last();

            fiftyMovesRule = lastMove.FiftyMoveRuleToThisMove;


            _ = RemovePieceAtPosition(lastMove.ToPos);
            PlacePieceAtPostion(lastMove.ToMovePiece, lastMove.FromPos);


            if (lastMove.IsCastling) //WAS CASTLING
            {
                bool isShort =  lastMove.ToPos.X - lastMove.FromPos.X > 0;
                int yCord = lastMove.OtherPiece.PieceColor == ChessColors.WHITE ? 0 : 7;

                Vec2 rookOldPos = (isShort) ? new Vec2(7, yCord) : new Vec2(0, yCord);
                _ = RemovePieceAtPosition(lastMove.OtherPiece.Position);
                PlacePieceAtPostion(lastMove.OtherPiece, rookOldPos);
            }
            else if(lastMove.OtherPiece != null) //CHECK IF WAS CAPTURE
            {
                if (lastMove.IsEnPassantCapture)
                {
                    int dir = (lastMove.ToMovePiece.PieceColor == ChessColors.WHITE) ? -1 : 1;
                    PlacePieceAtPostion(lastMove.OtherPiece, lastMove.ToPos.X, lastMove.ToPos.Y + dir);
                }
                else
                    PlacePieceAtPostion(lastMove.OtherPiece, lastMove.ToPos);
            }

            if (lastMove.IsPromotion) //WAS PROMOTION
            {
                lastMove.ToMovePiece.PieceClass = PieceClasses.PAWN;
                OnPawnPromotionReversed?.Invoke(lastMove.ToMovePiece);
            }

            PlayedMoves.Remove(lastMove);
            if(movedPieces.TryGetValue(lastMove.ToMovePiece,out Move move))
            {
                if(move == lastMove)
                {
                    movedPieces.Remove(lastMove.ToMovePiece);
                }
            }


            (this.validMovesWhite, this.validMovesBlack) = movesValidator.GetValidMoves();


            ChangeCurrentlyPlayingSide();

            OnLastMoveUnMade?.Invoke(this);

            return CurrentGameResult;
        }

        private void CheckDrawInsufficientMaterial()
        {
            if (ChessPieces.Count <= 4) //Insufficient material
            {
                if (ChessPieces.Count == 2)
                {
                    CurrentGameResult = GameResult.DRAW;//Two kings
                }
                else if (ChessPieces.Count == 3) // Two kings and bishop or knight
                {
                    ChessPiece? other = ChessPieces.Find(p => p.PieceClass != PieceClasses.KING);
                    if (other == null) throw new Exception();
                    if (other.PieceClass == PieceClasses.BISHOP || other.PieceClass == PieceClasses.KNIGHT) CurrentGameResult = GameResult.DRAW;
                }
                else //Two king and two bishops
                {
                    var pieces = ChessPieces.FindAll(p => p.PieceClass == PieceClasses.BISHOP);
                    if (pieces.Count == 2 && pieces[0].PieceColor != pieces[1].PieceColor)
                    {
                        foreach (var bishop in pieces)
                        {
                            if ((bishop.Position.X + bishop.Position.Y) % 2 == 1 && bishop.PieceColor == ChessColors.WHITE)
                            {
                                CurrentGameResult = GameResult.DRAW;
                            }
                            if ((bishop.Position.X + bishop.Position.Y) % 2 == 0 && bishop.PieceColor == ChessColors.BLACK)
                            {
                                CurrentGameResult = GameResult.DRAW;
                            }
                        }
                    }
                }
            }
        }

 
        private void CheckCheckMateAndStalemate()
        {
            var nextMoves = (CurrentSide == ChessColors.WHITE) ? validMovesBlack : validMovesWhite;
            bool isChecked = (CurrentSide == ChessColors.WHITE) ? movesValidator.IsChecked(ChessColors.BLACK) : movesValidator.IsChecked(ChessColors.WHITE);

            if(isChecked)OnCheck?.Invoke();

            if (nextMoves.Count == 0)
            {
                if (isChecked)
                {
                    OnCheckMate?.Invoke();
                    CurrentGameResult = (CurrentSide == ChessColors.WHITE) ? GameResult.WHITE_WON : GameResult.WHITE_WON; //CheckMate
                }
                else CurrentGameResult = GameResult.DRAW; //Stalemate
            }
        }

        private void CheckFiftyMoveRule()
        {
            if (fiftyMovesRule == 50)
            {
                CurrentGameResult = GameResult.DRAW; //50-Move Rule
            }
        }       
    }

}
