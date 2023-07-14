using Chess.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Chess.Engine
{
    public class ChessBoard
    {
        public Action<ChessPiece>? OnPieceCaptured;
        public Action<ChessPiece>? OnPawnPromotion;
        
        public const int BOARD_SIZE = 8;


        public ChessColors CurrentSide { get; private set; }

        public List<ChessPiece> ChessPieces { get; private set; }
        public List<ChessPiece> ChessPiecesWhite { get; private set; }
        public List<ChessPiece> ChessPiecesBlack { get; private set; }

        public List<Move> PlayedMoves { get; private set; }

        private MovesValidator movesValidator;

        private BoardCell[,] boardCells;

        private Dictionary<ChessPiece, Move> movedPieces;


        #region CTOR

        public ChessBoard(string fen) : this()
        {
            (ChessColors currentSide, List<ChessPiece> chessPieces) = FenInterpreter.GetSetupFromFen(fen);
            CurrentSide = currentSide;
            PlacePiecesAfterInit(chessPieces);
            SetupPiecesLists();
        }

        public ChessBoard(ChessColors currentSide, params ChessPiece[] chessPieces) : this()
        {
            CurrentSide = currentSide;
            PlacePiecesAfterInit(new(chessPieces));
            SetupPiecesLists();
        }

        public ChessBoard(ChessColors currentSide, List<ChessPiece> chessPieces) : this()
        {
            CurrentSide = currentSide;
            PlacePiecesAfterInit(chessPieces);
            SetupPiecesLists();
        }

        public ChessBoard()
        {
            ChessPieces = new List<ChessPiece>();
            PlayedMoves = new();
            ChessPiecesBlack = new();
            ChessPiecesWhite = new();
            movesValidator = new(this);
            movedPieces = new();


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

        private void SetupPiecesLists()
        {
            foreach (var piece in ChessPieces)
            {
                if(piece.PieceColor == ChessColors.BLACK)
                {
                    ChessPiecesBlack.Add(piece);
                }
                else
                {
                    ChessPiecesWhite.Add(piece);
                }
            }
        }

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
            (var white, var black) = movesValidator.GetValidMoves();
            return (white, black);
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

            if (boardCells[x, y].HasPiece())
            {
                return;
            }
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
            int fromX, fromY, toX, toY;

            fromX = LetterToNum(moveString[0]);
            fromY = moveString[1] - 48 - 1;
            toX = LetterToNum(moveString[2]);
            toY = moveString[3] - 48 - 1;


            (var white, var black) = GetListOfValidMoves();

            var current = (CurrentSide == ChessColors.WHITE)? white : black;

            if (!current.Exists(m => m.FromPos.X == fromX && m.FromPos.Y == fromY && m.ToPos.X == toX && m.ToPos.Y == toY))
            {
                return false;
            }
            MakeMove(current.Find(m => m.FromPos.X == fromX && m.FromPos.Y == fromY && m.ToPos.X == toX && m.ToPos.Y == toY));
            return true;
        }

        public void MakeMove(Move move)
        {
            var fromCell = GetCell(move.FromPos);
            _ = fromCell.HasPiece(out ChessPiece pieceToMove);

            if (move.IsCastling) //CASTLING
            {
                bool isShort = Math.Abs(move.OtherPiece.Position.X - move.ToPos.X) == 1;

                Vec2 rookNewPos = (isShort) ? move.ToPos + Vec2.Left : move.ToPos + Vec2.Right;
                _ = RemovePieceAtPosition(move.OtherPiece.Position);
                PlacePieceAtPostion(move.OtherPiece, rookNewPos);
            }
            else if(move.IsPromotion) //Promotion
            {
                OnPawnPromotion?.Invoke(pieceToMove);

                //TMP
                pieceToMove.PieceClass = PieceClasses.QUEEN;
            }
            else
            {
                
                var pieceOnTarget = move.OtherPiece;

                if (pieceOnTarget != null)
                {
                    _ =RemovePieceAtPosition(pieceOnTarget.Position);
                    OnPieceCaptured?.Invoke(pieceOnTarget);
                }
            }


            _ = RemovePieceAtPosition(fromCell.Position);
            PlacePieceAtPostion(pieceToMove, move.ToPos);
            PlayedMoves.Add(move);
            _ = movedPieces.TryAdd(pieceToMove, move);
            ChangeCurrentlyPlayingSide();

            return;
        }

        

       
    }

}
