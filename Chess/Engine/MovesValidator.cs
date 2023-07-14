using Chess.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Chess.Engine
{
    public class MovesValidator
    {
        private const int MAX_MOVES_AMOUNT = 200;

        private ChessBoard board;

        private List<Move> sudoValidMovesWhite, sudoValidMovesBlack;
        (Move, HashSet<Vec2>)? checkMoveWhite, checkMoveBlack;
        private Dictionary<ChessPiece, HashSet<Vec2>> pinnedPiecesWhite, pinnedPiecesBlack;
        private bool[,] attackZoneWhite;
        private bool[,] attackZoneBlack;


        public MovesValidator(ChessBoard board)
        {
            InitData();
            this.board = board;
        }

        private void InitData()
        {
            //sudoValidMovesWhite.Clear();
            //sudoValidMovesBlack.Clear();
            //checkMoveWhite = null;
            //checkMoveBlack = null;
            //pinMovesWhite.Clear();
            //pinMovesBlack.Clear();
            sudoValidMovesWhite = new(MAX_MOVES_AMOUNT);
            sudoValidMovesBlack = new(MAX_MOVES_AMOUNT);
            checkMoveWhite = null;
            checkMoveBlack = null;
            pinnedPiecesWhite = new();
            pinnedPiecesBlack = new();
            attackZoneWhite = new bool[ChessBoard.BOARD_SIZE, ChessBoard.BOARD_SIZE];
            attackZoneBlack = new bool[ChessBoard.BOARD_SIZE, ChessBoard.BOARD_SIZE];
        }

        public (List<Move> validMovesWhite, List<Move> validMovesBlack) GetValidMoves()
        {
            InitData();

            FindSudoValidMoves();

            (List<Move> validMovesWhite, List<Move> validMovesBlack) = FindValidMoves();
            return (validMovesWhite, validMovesBlack);
        }

        private void FindSudoValidMoves()
        {
            foreach (var piece in board.ChessPieces)
            {
                switch (piece.PieceClass)
                {
                    case PieceClasses.KING:
                        GetKingRawMoves(piece);
                        break;
                    case PieceClasses.QUEEN:
                        GetQueenRawMoves(piece);
                        break;
                    case PieceClasses.ROOK:
                        GetRookRawMoves(piece);
                        break;
                    case PieceClasses.BISHOP:
                        GetBishopRawMoves(piece);
                        break;
                    case PieceClasses.PAWN:
                        GetPawnRawMoves(piece);
                        break;
                    case PieceClasses.KNIGHT:
                        GetKnightRawMoves(piece);
                        break;
                    default:
                        break;
                }
            }
        }


        private (List<Move> validMovesWhite, List<Move> validMovesBlack) FindValidMoves()
        {
            var validWhiteMoves = HandleSide(sudoValidMovesWhite, checkMoveBlack, pinnedPiecesWhite, attackZoneBlack);
            var validBlackMoves = HandleSide(sudoValidMovesBlack, checkMoveWhite, pinnedPiecesBlack, attackZoneWhite);

            return (validWhiteMoves, validBlackMoves);



            List<Move> HandleSide(List<Move> sudoValidMoves, (Move, HashSet<Vec2>)? checkMove, Dictionary<ChessPiece, HashSet<Vec2>> pinnedPieces, bool[,] enemyAttackZone)
            {
                List<Move> validMoves = new();

                if(checkMove != null)//check
                {
                    (var cMove, var availablePos) = checkMove.Value;
                    foreach (var move in sudoValidMoves)
                    {
                        var piece = board.GetCell(move.FromPos).GetPiece();

                        if(piece == cMove.OtherPiece) //checked king
                        {
                            if (enemyAttackZone[move.ToPos.X, move.ToPos.Y] == true || move.IsCastling) continue;
                        }
                        else
                        {
                            if (!availablePos.Contains(move.ToPos)) continue;
                        }

                        validMoves.Add(move);
                    }
                }
                else
                {
                    foreach (var move in sudoValidMoves)
                    {
                        var piece = board.GetCell(move.FromPos).GetPiece();

                        if (pinnedPieces.TryGetValue(piece, out var validCords))//pin
                        {
                            if(validCords.Contains(move.ToPos))
                            {
                                validMoves.Add(move);
                                continue;
                            }
                        }
                        else if (move.IsCastling) //castling
                        {
                            int dir = (move.FromPos.X - move.ToPos.X < 0) ? 1 : -1;
                            int range = (dir == 1) ? 3 : 4;

                            for (int i = 1; i <= range; i++)
                            {
                                if (enemyAttackZone[move.FromPos.X + dir * i, move.FromPos.Y])
                                {
                                    continue;
                                }
                            }
                            validMoves.Add(move);
                        }
                        else
                        {
                            validMoves.Add(move);
                        }
                    }
                }

                return validMoves;
            }
        }



        #region RAW MOVES
        private void AddSudoValidMove(ChessColors color, Move move, bool isAttackZone = true)
        {
            if(color == ChessColors.WHITE)
            {
                sudoValidMovesWhite.Add(move);
                attackZoneWhite[move.ToPos.X, move.ToPos.Y] = isAttackZone;
            }
            else
            {
                sudoValidMovesBlack.Add(move);
                attackZoneBlack[move.ToPos.X, move.ToPos.Y] = isAttackZone;
            }
        }



        private void GetRookRawMoves(ChessPiece piece)
        {
            CheckLongMoves(piece, 0, 1);
            CheckLongMoves(piece, 1, 0);
            CheckLongMoves(piece, 0, -1);
            CheckLongMoves(piece, -1, 0);
        }

        private void GetKnightRawMoves(ChessPiece piece)
        {
            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    if (i == j || i == -j) continue;
                    if (i == 0 || j == 0) continue;

                    var checkPos = piece.Position + new Vec2(i, j);
                    var cell = board.GetCell(checkPos);
                    if (cell == null) continue;


                    if (!cell.HasPiece(out var occupying))
                    {
                        AddSudoValidMove(piece.PieceColor, new Move(piece.Position, checkPos));
                        continue;
                    }

                    if (piece.PieceColor != occupying.PieceColor)
                    {
                        AddSudoValidMove(piece.PieceColor, new Move(piece.Position, checkPos, occupying));
                    }
 
                }
            }
        }

        private void GetBishopRawMoves(ChessPiece piece)
        {
            CheckLongMoves(piece, 1, 1);
            CheckLongMoves(piece, 1, -1);
            CheckLongMoves(piece, -1, -1);
            CheckLongMoves(piece, -1, 1);
        }

        private void GetPawnRawMoves(ChessPiece piece)
        {
            Vec2 forward = (piece.PieceColor == ChessColors.WHITE) ? Vec2.Up : Vec2.Down;
            int promotionHight = (piece.PieceColor == ChessColors.WHITE) ? 7 : 0;

            //forward move
            var cell = board.GetCell(piece.Position + forward);
            if (cell != null && cell.HasPiece() == false)
            {
                var move = new Move(piece.Position, cell.Position);
                move.IsPromotion = promotionHight == cell.Position.Y;
                AddSudoValidMove(piece.PieceColor, move, false);
            }

            //double forward move
            if (!board.HasPieceMoved(piece))
            {
                cell = board.GetCell(piece.Position + forward + forward);
                if (cell != null && cell.HasPiece() == false)
                {
                    var move = new Move(piece.Position, cell.Position);
                    move.IsPawnTwoForward = true;
                    AddSudoValidMove(piece.PieceColor, move);
                }
            }

            //forward-right capture
            cell = board.GetCell(piece.Position + forward + Vec2.Right);
            if (cell != null && cell.HasPiece(out ChessPiece rightPiece) == true)
            {
                var move = new Move(piece.Position, cell.Position, rightPiece);
                move.IsPromotion = promotionHight == cell.Position.Y;
                AddSudoValidMove(piece.PieceColor, move);
            }

            //forward-left capture
            cell = board.GetCell(piece.Position + forward + Vec2.Left);
            if (cell != null && cell.HasPiece(out ChessPiece leftPiece) == true)
            {
                var move = new Move(piece.Position, cell.Position, leftPiece);
                move.IsPromotion = promotionHight == cell.Position.Y;
                AddSudoValidMove(piece.PieceColor, move);
            }

            //EnPassantCapture left
            cell = board.GetCell(piece.Position + Vec2.Left);
            if(cell != null && cell.HasPiece(out var enPassantLeft) && board.HasPieceMoved(enPassantLeft, out Move moveLeft) && moveLeft.IsPawnTwoForward)
            {
                var move = new Move(piece.Position, cell.Position + forward, enPassantLeft);
                move.IsEnPassantCapture = true;
                AddSudoValidMove(piece.PieceColor, move, false);
            }

            //EnPassantCapture right
            cell = board.GetCell(piece.Position + Vec2.Right);
            if (cell != null && cell.HasPiece(out var enPassantRight) && board.HasPieceMoved(enPassantRight, out Move moveRight) && moveRight.IsPawnTwoForward)
            {
                var move = new Move(piece.Position, cell.Position + forward, enPassantRight);
                move.IsEnPassantCapture = true;
                AddSudoValidMove(piece.PieceColor, move, false);
            }
        }

        private void GetKingRawMoves(ChessPiece piece)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    CheckLongMoves(piece, i, j, 1);
                }
            }

            if (board.HasPieceMoved(piece))
            {
                return;
            }

            //check castling

            var shortCastlingCell = (piece.PieceColor == ChessColors.WHITE) ? board.GetCell(7, 0) : board.GetCell(7, 7);
            var longCastlingCell = (piece.PieceColor == ChessColors.WHITE) ? board.GetCell(0, 0) : board.GetCell(0, 7);

            if (shortCastlingCell.HasPiece(out ChessPiece rookS) && rookS.PieceClass == PieceClasses.ROOK)
            {
                if (!board.HasPieceMoved(rookS)) //can do short (no check checked)
                {
                    var move1 = new Move(piece.Position, rookS.Position, rookS);
                    var move2 = new Move(piece.Position, rookS.Position + Vec2.Left, rookS);
                    move1.IsCastling = true;
                    move2.IsCastling = true;
                    AddSudoValidMove(piece.PieceColor, move1, false);
                    AddSudoValidMove(piece.PieceColor, move2, false);
                }
            }

            if (longCastlingCell.HasPiece(out ChessPiece rookL) && rookL.PieceClass == PieceClasses.ROOK)
            {
                if (!board.HasPieceMoved(rookL)) //can do long (no check checked)
                {
                    var move1 = new Move(piece.Position, rookL.Position, rookL);
                    var move2 = new Move(piece.Position, rookL.Position + Vec2.Right, rookL);
                    move1.IsCastling = true;
                    move2.IsCastling = true;
                    AddSudoValidMove(piece.PieceColor, move1, false);
                    AddSudoValidMove(piece.PieceColor, move2, false);
                }
            }
        }

        private void GetQueenRawMoves(ChessPiece piece)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    CheckLongMoves(piece, i, j);
                }
            }
        }




        private void CheckLongMoves(ChessPiece piece, int xDir, int yDir, int length = ChessBoard.BOARD_SIZE - 1)
        {
            HashSet<Vec2> posInRange = new();

            int enemiesAmount = 0;
            ChessPiece firstEnemyFound = null;

            for (int i = 1; i <= length; i++)
            {
                var checkPos = piece.Position + new Vec2(xDir * i, yDir * i);

                if (piece.Position == checkPos) continue;

                var cell = board.GetCell(checkPos);
                if (cell == null) break;

                if (!cell.HasPiece(out var occupying))
                {
                    posInRange.Add(checkPos);
                    if (enemiesAmount == 0)
                    {
                        Move move = new Move(piece.Position, checkPos);
                        AddSudoValidMove(piece.PieceColor, move);
                    }
                    
                }
                else if (piece.PieceColor != occupying.PieceColor)
                {
                    var move = new Move(piece.Position, checkPos, occupying);

                    if (occupying.PieceClass == PieceClasses.KING && enemiesAmount == 0) //check
                    {
                        if (piece.PieceColor == ChessColors.WHITE) checkMoveWhite = (move, posInRange);
                        else checkMoveBlack = (move, posInRange);

                        AddSudoValidMove(piece.PieceColor, move);

                        break;
                    }
                    else if(occupying.PieceClass == PieceClasses.KING && enemiesAmount == 1) //pin
                    {
                        if (piece.PieceColor == ChessColors.WHITE) pinnedPiecesBlack.Add(firstEnemyFound, posInRange);
                        else pinnedPiecesWhite.Add(firstEnemyFound, posInRange);

                        break;
                    }
                    else if (enemiesAmount == 0)
                    {
                        firstEnemyFound = occupying;
                        AddSudoValidMove(piece.PieceColor, move);
                    }

                    posInRange.Add(checkPos);

                    enemiesAmount++;
                }
                else
                {
                    break;
                }
            }
        }


        #endregion RAW MOVES   
    }
}

