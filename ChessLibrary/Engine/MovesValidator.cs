using ChessLibrary.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public class MovesValidator
    {
        private const int MAX_MOVES_AMOUNT = 200;

        private GameManager board;

        private List<Move> sudoValidMovesWhite, sudoValidMovesBlack;
        (Move, HashSet<Vec2>)? checkMoveWhite, checkMoveBlack;
        private Dictionary<BoardEntityFactory, HashSet<Vec2>> pinnedPieceClassesWhite, pinnedPieceClassesBlack;
        private bool[,] attackZoneWhite;
        private bool[,] attackZoneBlack;


        public MovesValidator(GameManager board)
        {
            InitData();
            this.board = board;
        }

        private void InitData()
        {
            sudoValidMovesWhite = new(MAX_MOVES_AMOUNT);
            sudoValidMovesBlack = new(MAX_MOVES_AMOUNT);
            checkMoveWhite = null;
            checkMoveBlack = null;
            pinnedPieceClassesWhite = new();
            pinnedPieceClassesBlack = new();
            attackZoneWhite = new bool[GameManager.BOARD_SINGLE_ROW_SIZE, GameManager.BOARD_SINGLE_ROW_SIZE];
            attackZoneBlack = new bool[GameManager.BOARD_SINGLE_ROW_SIZE, GameManager.BOARD_SINGLE_ROW_SIZE];
        }

        public (List<Move> validMovesWhite, List<Move> validMovesBlack) GetValidMoves()
        {
            InitData();

            FindSudoValidMoves();

            (List<Move> validMovesWhite, List<Move> validMovesBlack) = FindValidMoves();
            return (validMovesWhite, validMovesBlack);
        }

        public bool IsChecked(ChessColors color)
        {
            if (color == ChessColors.WHITE && checkMoveBlack != null) return true;
            if (color == ChessColors.BLACK && checkMoveWhite != null) return true;
            return false;
        }

        private void FindSudoValidMoves()
        {
            foreach (var piece in board.ChessPieceClasses)
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
            var validWhiteMoves = HandleSide(sudoValidMovesWhite, checkMoveBlack, pinnedPieceClassesWhite, attackZoneBlack);
            var validBlackMoves = HandleSide(sudoValidMovesBlack, checkMoveWhite, pinnedPieceClassesBlack, attackZoneWhite);

            return (validWhiteMoves, validBlackMoves);



            List<Move> HandleSide(List<Move> sudoValidMoves, (Move, HashSet<Vec2>)? checkMove, Dictionary<BoardEntityFactory, HashSet<Vec2>> pinnedPieceClasses, bool[,] enemyAttackZone)
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

                        if (pinnedPieceClasses.TryGetValue(piece, out var validCords))//pin
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
                            int range = (dir == 1) ? 2 : 3;
                            bool isSkip = false;
                            for (int i = 1; i <= range; i++)
                            {
                                int toCheckX = move.FromPos.X + dir * i;
                                bool isAttacked =(dir == -1 && i == range)? false : enemyAttackZone[toCheckX, move.FromPos.Y];
                                bool isOccupied = board.GetCell(toCheckX, move.FromPos.Y).HasPiece();
                                if (isAttacked || isOccupied)
                                {
                                    isSkip = true;
                                    break;
                                }
                            }
                            if (isSkip)
                            {
                                continue;
                            }

                            validMoves.Add(move);
                        }
                        else if(piece.PieceClass == PieceClasses.KING && (enemyAttackZone[move.ToPos.X, move.ToPos.Y] == true))//king
                        {
                            continue;
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

        private void AddAttackZone(ChessColors color, Vec2 pos)
        {
            if (color == ChessColors.WHITE)
            {
                attackZoneWhite[pos.X, pos.Y] = true;
            }
            else
            {
                attackZoneBlack[pos.X, pos.Y] = true;
            }
        }



        private void GetRookRawMoves(BoardEntityFactory piece)
        {
            CheckLongMoves(piece, 0, 1);
            CheckLongMoves(piece, 1, 0);
            CheckLongMoves(piece, 0, -1);
            CheckLongMoves(piece, -1, 0);
        }

        private void GetKnightRawMoves(BoardEntityFactory piece)
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
                        AddSudoValidMove(piece.PieceColor, new Move(piece.Position, checkPos, piece, null));
                        continue;
                    }

                    if (piece.PieceColor != occupying.PieceColor)
                    {
                        var move = new Move(piece.Position, checkPos, piece, occupying);
                        if (occupying.PieceClass == PieceClasses.KING) //check
                        {
                            HashSet<Vec2> posInRange = new HashSet<Vec2>();
                            posInRange.Add(new Vec2(piece.Position));

                            if (piece.PieceColor == ChessColors.WHITE) checkMoveWhite = (move, posInRange);
                            else checkMoveBlack = (move, posInRange);
                        }

                        AddSudoValidMove(piece.PieceColor, move);
                    }
                    else
                    {
                        AddAttackZone(piece.PieceColor, occupying.Position);
                    }
 
                }
            }
        }

        private void GetBishopRawMoves(BoardEntityFactory piece)
        {
            CheckLongMoves(piece, 1, 1);
            CheckLongMoves(piece, 1, -1);
            CheckLongMoves(piece, -1, -1);
            CheckLongMoves(piece, -1, 1);
        }

        private void GetPawnRawMoves(BoardEntityFactory piece)
        {
            Vec2 forward = (piece.PieceColor == ChessColors.WHITE) ? Vec2.Up : Vec2.Down;
            int promotionHight = (piece.PieceColor == ChessColors.WHITE) ? 7 : 0;

            //forward move
            var cell = board.GetCell(piece.Position + forward);
            if (cell != null && cell.HasPiece() == false)
            {
                var move = new Move(piece.Position, cell.Position, piece, null);
                move.IsPromotion = promotionHight == cell.Position.Y;
                AddSudoValidMove(piece.PieceColor, move, false);
            }

            //double forward move
            if (!board.HasPieceMoved(piece))
            {
                cell = board.GetCell(piece.Position + forward + forward);
                var cellSingle = board.GetCell(piece.Position + forward);
                if (cellSingle.HasPiece() == false && cell != null && cell.HasPiece() == false)
                {
                    var move = new Move(piece.Position, cell.Position, piece, null);
                    move.IsPawnTwoForward = true;
                    AddSudoValidMove(piece.PieceColor, move);
                }
            }

            //forward-right capture
            cell = board.GetCell(piece.Position + forward + Vec2.Right);
            if (cell != null && cell.HasPiece(out BoardEntityFactory rightPiece) == true && rightPiece.PieceColor != piece.PieceColor)
            {
                var move = new Move(piece.Position, cell.Position, piece, rightPiece);
                move.IsPromotion = promotionHight == cell.Position.Y;

                if (rightPiece.PieceClass == PieceClasses.KING) //check
                {
                    HashSet<Vec2> posInRange = new HashSet<Vec2>();
                    posInRange.Add(new Vec2(piece.Position));

                    if (piece.PieceColor == ChessColors.WHITE) checkMoveWhite = (move, posInRange);
                    else checkMoveBlack = (move, posInRange);
                }

                AddSudoValidMove(piece.PieceColor, move);
            }
            else if(cell != null)
            {
                AddAttackZone(piece.PieceColor, cell.Position);
            }

            //forward-left capture
            cell = board.GetCell(piece.Position + forward + Vec2.Left);
            if (cell != null && cell.HasPiece(out BoardEntityFactory leftPiece) == true && leftPiece.PieceColor != piece.PieceColor)
            {
                var move = new Move(piece.Position, cell.Position, piece, leftPiece);
                move.IsPromotion = promotionHight == cell.Position.Y;

                if (leftPiece.PieceClass == PieceClasses.KING) //check
                {
                    HashSet<Vec2> posInRange = new HashSet<Vec2>();
                    posInRange.Add(new Vec2(piece.Position));

                    if (piece.PieceColor == ChessColors.WHITE) checkMoveWhite = (move, posInRange);
                    else checkMoveBlack = (move, posInRange);
                }

                AddSudoValidMove(piece.PieceColor, move);
            }
            else if (cell != null)
            {
                AddAttackZone(piece.PieceColor, cell.Position);
            }

            //EnPassantCapture left
            cell = board.GetCell(piece.Position + Vec2.Left);
            if(cell != null && cell.HasPiece(out var enPassantLeft) 
                && board.HasPieceMoved(enPassantLeft, out Move moveLeft)
                && enPassantLeft.PieceColor != piece.PieceColor
                && moveLeft.IsPawnTwoForward && 1 == board.PlayedMoves.Count - moveLeft.OrderInPlayedMoves)
            {
                var move = new Move(piece.Position, cell.Position + forward, piece, enPassantLeft);
                move.IsEnPassantCapture = true;
                AddSudoValidMove(piece.PieceColor, move, false);
            }

            //EnPassantCapture right
            cell = board.GetCell(piece.Position + Vec2.Right);
            if (cell != null && cell.HasPiece(out var enPassantRight) 
                && board.HasPieceMoved(enPassantRight, out Move moveRight)
                && enPassantRight.PieceColor != piece.PieceColor
                && moveRight.IsPawnTwoForward 
                && 1 == board.PlayedMoves.Count - moveRight.OrderInPlayedMoves)
            {
                var move = new Move(piece.Position, cell.Position + forward, piece, enPassantRight);
                move.IsEnPassantCapture = true;
                AddSudoValidMove(piece.PieceColor, move, false);
            }
        }

        private void GetKingRawMoves(BoardEntityFactory piece)
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

            if (shortCastlingCell.HasPiece(out BoardEntityFactory rookS) && rookS.PieceClass == PieceClasses.ROOK)
            {
                if (!board.HasPieceMoved(rookS)) //can do short (no check checked)
                {
                    var move1 = new Move(piece.Position, piece.Position + Vec2.Right + Vec2.Right, piece, rookS);
                    //var move2 = new Move(piece.Position, piece.Position + Vec2.Right + Vec2.Right, piece, rookS);
                    move1.SetCastling(rookS.Position);
                    //move2.SetCastling(rookS.Position + Vec2.Left);
                    AddSudoValidMove(piece.PieceColor, move1, false);
                    //AddSudoValidMove(piece.PieceColor, move2, false);
                }
            }

            if (longCastlingCell.HasPiece(out BoardEntityFactory rookL) && rookL.PieceClass == PieceClasses.ROOK)
            {
                if (!board.HasPieceMoved(rookL)) //can do long (no check checked)
                {
                    var move1 = new Move(piece.Position, piece.Position + Vec2.Left + Vec2.Left, piece, rookL);
                    //var move2 = new Move(piece.Position, piece.Position + Vec2.Left + Vec2.Left, piece, rookL);
                    move1.SetCastling(rookL.Position);
                    //move2.SetCastling(rookL.Position + Vec2.Right);
                    AddSudoValidMove(piece.PieceColor, move1, false);
                    //AddSudoValidMove(piece.PieceColor, move2, false);
                }
            }
        }

        private void GetQueenRawMoves(BoardEntityFactory piece)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    CheckLongMoves(piece, i, j);
                }
            }
        }




        private void CheckLongMoves(BoardEntityFactory piece, int xDir, int yDir, int length = GameManager.BOARD_SINGLE_ROW_SIZE - 1)
        {
            HashSet<Vec2> posInRange = new();

            int enemiesAmount = 0;
            BoardEntityFactory firstEnemyFound = null;

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
                        Move move = new Move(piece.Position, checkPos, piece, null);
                        AddSudoValidMove(piece.PieceColor, move);
                    }
                    
                }
                else if (piece.PieceColor != occupying.PieceColor)
                {
                    var move = new Move(piece.Position, checkPos, piece, occupying);

                    if (occupying.PieceClass == PieceClasses.KING && enemiesAmount == 0) //check
                    {
                        posInRange.Add(piece.Position);

                        if (piece.PieceColor == ChessColors.WHITE) checkMoveWhite = (move, posInRange);
                        else checkMoveBlack = (move, posInRange);

                        AddSudoValidMove(piece.PieceColor, move);

                        break;
                    }
                    else if(occupying.PieceClass == PieceClasses.KING && enemiesAmount == 1) //pin
                    {
                        posInRange.Add(new Vec2(piece.Position));
                        if (piece.PieceColor == ChessColors.WHITE) pinnedPieceClassesBlack.Add(firstEnemyFound, posInRange);
                        else pinnedPieceClassesWhite.Add(firstEnemyFound, posInRange);

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
                    AddAttackZone(piece.PieceColor, occupying.Position);
                    break;
                }
            }
        }


        #endregion RAW MOVES   
    }
}

