using ChessLibrary.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

namespace ChessLibrary.Engine
{
    public class MovesValidator
    {
        private const int MAX_MOVES_AMOUNT = 218;
        private Game game;
        private Board board;


        private List<Move> sudoValidMoves;
        private List<Move> sudoValidKingMoves;
        private List<Move> sudoValidPawnsMoves;
        private List<Move> sudoValidRooksMoves;
        HashSet<int>? checkPossiblePos;
        private Dictionary<int,HashSet<int>> pinnedPieces; //piece from which pos is pinned / where it can move
        private bool[] enemyAttackZone;
        private bool isWhiteMove;
        private int enPassantPawnPinnedPos = -1;

        public MovesValidator(Game game, Board board)
        {
            this.game = game;
            this.board = board;
            Setup();
        }

        public bool[] GetEnemyAttackZone() { return enemyAttackZone;  }

        private void Setup()
        {
            sudoValidPawnsMoves = new();
            //isEnPassantPawnPinned = false;
            sudoValidMoves = new();
            sudoValidKingMoves = new();
            sudoValidRooksMoves = new();
            checkPossiblePos = null;
            pinnedPieces = new();
            isWhiteMove = game.GetCurrentlyPlayingSide() == ChessColors.WHITE ? true : false;
            enemyAttackZone = new bool[Board.BOARD_SIZE];
        }

        public Move[] GetValidMoves()
        {
            Setup();

            SetupSudoValidMoves();

            sudoValidKingMoves.RemoveAll(m => enemyAttackZone[m.ToPos] == true);

            if (checkPossiblePos != null)
            {
                sudoValidMoves.AddRange(sudoValidRooksMoves);
                sudoValidMoves.AddRange(sudoValidPawnsMoves);
                sudoValidMoves.RemoveAll(m => !checkPossiblePos.Contains(m.ToPos));
            }
            else
            {
                if(enPassantPawnPinnedPos != -1)
                {
                    sudoValidPawnsMoves.RemoveAll(m => m.GetEnPassantPosition() != -1 && m.FromPos == enPassantPawnPinnedPos);
                }
                HandleCastling();
                sudoValidMoves.AddRange(sudoValidRooksMoves);
                sudoValidMoves.AddRange(sudoValidPawnsMoves);
                sudoValidMoves.RemoveAll(m => pinnedPieces.TryGetValue(m.FromPos, out var valid) && !valid.Contains(m.ToPos)); //pin
            }

            sudoValidMoves.AddRange(sudoValidKingMoves);
            return sudoValidMoves.ToArray();
        }


        private void HandleCastling()
        {
            if (game.GetCastling() == null) return;

            //add removing castling flag when rooks move

            //castling
            if (isWhiteMove)
            {
                if (game.GetCastling()[0]) //K
                {
                    int rookPos = 7;
                    int kingPos = 4;

                    sudoValidRooksMoves.ForEach(m => {
                        if (m.FromPos == rookPos) m = m.AddCastlingFlag(0);
                    });

                    bool canCastle = true;
                    for (int i = kingPos+1; i <= kingPos + 2; i++)
                    {
                        if(enemyAttackZone[i] || board.CheckIfCellHavePiece(i))
                        {
                            canCastle = false;
                        }
                    }
                    if(canCastle)
                    {
                        sudoValidKingMoves.Add(new Move(kingPos, kingPos+2).AddAffectedPiece(rookPos, 5, board.GetCellCode(rookPos)));
                    }
                }
                if (game.GetCastling()[1] && !enemyAttackZone[2] && !enemyAttackZone[3]) //Q
                {
                    int rookPos = 0;
                    int kingPos = 4;
                    bool canCastle = true;

                    sudoValidRooksMoves.ForEach(m => {
                        if (m.FromPos == rookPos) m = m.AddCastlingFlag(1);
                    });

                    for (int i = kingPos - 1; i >= kingPos - 2; i--)
                    {
                        if (enemyAttackZone[i] || board.CheckIfCellHavePiece(i))
                        {
                            canCastle = false;
                        }
                    }
                    if(board.CheckIfCellHavePiece(kingPos - 3))
                        canCastle = false;
                    if (canCastle)
                    {
                        sudoValidKingMoves.Add(new Move(kingPos, kingPos-2).AddAffectedPiece(rookPos, kingPos-1, board.GetCellCode(rookPos)));
                    }
                }
                sudoValidKingMoves.ForEach(m => m.AddCastlingFlag(0, 1));

            }
            else
            {

                if (game.GetCastling()[2] && !enemyAttackZone[61] && !enemyAttackZone[62]) //k
                {
                    int rookPos = 63;
                    int kingPos = 60;
                    bool canCastle = true;

                    sudoValidRooksMoves.ForEach(m => {
                        if (m.FromPos == rookPos) m = m.AddCastlingFlag(2);
                    });

                    for (int i = kingPos + 1; i <= kingPos + 2; i++)
                    {
                        if (enemyAttackZone[i] || board.CheckIfCellHavePiece(i))
                        {
                            canCastle = false;
                        }
                    }
                    if (canCastle)
                    {
                        sudoValidKingMoves.Add(new Move(kingPos, kingPos+2).AddAffectedPiece(rookPos, kingPos+1, board.GetCellCode(rookPos)));
                    }
                }
                if (game.GetCastling()[3] && !enemyAttackZone[58] && !enemyAttackZone[59]) //q
                {
                    int rookPos = 56;
                    int kingPos = 60;
                    bool canCastle = true;

                    sudoValidRooksMoves.ForEach(m => {
                        if (m.FromPos == rookPos) m = m.AddCastlingFlag(3);
                    });

                    for (int i = kingPos - 1; i >= kingPos - 2; i--)
                    {
                        if (enemyAttackZone[i] || board.CheckIfCellHavePiece(i))
                        {
                            canCastle = false;
                        }
                    }
                    if (board.CheckIfCellHavePiece(kingPos - 3))
                        canCastle = false;
                    if (canCastle)
                    {
                        sudoValidKingMoves.Add(new Move(kingPos, kingPos-2).AddAffectedPiece(rookPos, kingPos - 1, board.GetCellCode(rookPos)));
                    }
                }
                sudoValidKingMoves.ForEach(m => m.AddCastlingFlag(2, 3));
            }
        }


        private void SetupSudoValidMoves()
        {
            foreach (var piecePos in game.GetPiecePositions())
            {
                uint cellCode = board.GetCellCode(piecePos);
                var pieceClass = BoardEntityFactory.GetPieceClass(cellCode);
                bool isWhite = BoardEntityFactory.CheckIfPieceWithGivenColor(cellCode, ChessColors.WHITE);
                bool isEnemy = (isWhite == isWhiteMove)? false : true;

                switch (pieceClass)
                {
                    case PieceClasses.KING:
                        sudoValidKingMoves.AddRange(GetKingRawMoves(piecePos, isEnemy));
                        break;
                    case PieceClasses.QUEEN:
                        sudoValidMoves.AddRange(GetQueenRawMoves(piecePos, isEnemy));
                        break;
                    case PieceClasses.ROOK:
                        sudoValidRooksMoves.AddRange(GetRookRawMoves(piecePos, isEnemy));
                        break;
                    case PieceClasses.BISHOP:
                        sudoValidMoves.AddRange(GetBishopRawMoves(piecePos, isEnemy));
                        break;
                    case PieceClasses.PAWN:
                        sudoValidPawnsMoves.AddRange(GetPawnRawMoves(piecePos, isEnemy));
                        break;
                    case PieceClasses.KNIGHT:
                        sudoValidMoves.AddRange(GetKnightRawMoves(piecePos, isEnemy));
                        break;
                    case PieceClasses.NONE:
                        break;
                    default:
                        break;
                }
            }
        }


        #region RAW MOVES

        private List<Move> GetRookRawMoves(int piecePos, bool isEnemy)
        {
            List<Move> moves = new List<Move>();

            Func<int, Directions, int, List<Move>> action = (isEnemy)? CheckEnemySideLongMoves : CheckCurrentSideLongMoves;
            moves.AddRange(action(piecePos, Directions.UP, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.RIGHT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.DOWN, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.LEFT, Board.BOARD_SINGLE_ROW_SIZE));
            return moves;
        }

        private List<Move> GetBishopRawMoves(int piecePos, bool isEnemy)
        {
            List<Move> moves = new List<Move>();

            Func<int, Directions, int, List<Move>> action = (isEnemy) ? CheckEnemySideLongMoves : CheckCurrentSideLongMoves;
            moves.AddRange(action(piecePos, Directions.UP_RIGHT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.UP_LEFT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.DOWN_RIGHT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.DOWN_LEFT, Board.BOARD_SINGLE_ROW_SIZE));
            return moves;

        }

        private List<Move> GetQueenRawMoves(int piecePos, bool isEnemy)
        {
            List<Move> moves = new List<Move>();

            Func<int, Directions, int, List<Move>> action = (isEnemy) ? CheckEnemySideLongMoves : CheckCurrentSideLongMoves;
            moves.AddRange(action(piecePos, Directions.UP_RIGHT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.UP_LEFT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.DOWN_RIGHT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.DOWN_LEFT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.UP, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.RIGHT, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.DOWN, Board.BOARD_SINGLE_ROW_SIZE));
            moves.AddRange(action(piecePos, Directions.LEFT, Board.BOARD_SINGLE_ROW_SIZE));
            return moves;

        }

        private List<Move> GetKnightRawMoves(int piecePos, bool isEnemy)
        {
            List<Move> moves = new();
            Action action = (isEnemy) ? CheckEnemySideKnightMoves : CheckCurrentSideKnightMoves;
            action();
            return moves;

            void CheckCurrentSideKnightMoves()
            {
                var toCheckPosition = BoardMovementRestrainer.GetKnightsValidMoveCells(piecePos);

                foreach (var checkPos in toCheckPosition)
                {
                    if (!board.CheckIfCellHavePiece(checkPos)) //no piece -> can move
                    {
                        moves.Add(new Move(piecePos, checkPos));
                    }
                    else if (board.CheckIfCellHavePieceWithGivenColor(checkPos, isWhiteMove)) //friendly piece
                    {
                        continue;
                    }
                    else //enemy piece
                    {
                        moves.Add(new Move(piecePos, checkPos).AddAffectedPiece(checkPos, null, board.GetCellCode(checkPos)));
                    }
                }
            }

            void CheckEnemySideKnightMoves()
            {
                var toCheckPosition = BoardMovementRestrainer.GetKnightsValidMoveCells(piecePos);

                foreach (var checkPos in toCheckPosition)
                {
                    HashSet<int> posInRadius = new();

                    if (!board.CheckIfCellHavePiece(checkPos)) //no piece -> can move
                    {
                        enemyAttackZone[checkPos] = true;
                    }
                    else if (board.CheckIfCellHavePieceWithGivenColor(checkPos, !isWhiteMove)) //friendly piece
                    {
                        return;
                    }
                    else //enemy piece
                    {
                        enemyAttackZone[checkPos] = true;

                        if (board.CheckIfCellHavePieceOfGivenClass(checkPos, PieceClasses.KING))
                        {
                            posInRadius.Add(piecePos);
                            checkPossiblePos = posInRadius;
                        }
                    }
                }
            }
        }

        private List<Move> GetKingRawMoves(int piecePos, bool isEnemy)
        {
            List<Move> res = new();
            Func<int, Directions, int, List<Move>> action = (isEnemy) ? CheckEnemySideLongMoves : CheckCurrentSideLongMoves;
            res.AddRange(action(piecePos, Directions.UP_RIGHT, 1));
            res.AddRange(action(piecePos, Directions.UP_LEFT, 1));
            res.AddRange(action(piecePos, Directions.DOWN_RIGHT, 1));
            res.AddRange(action(piecePos, Directions.DOWN_LEFT, 1));
            res.AddRange(action(piecePos, Directions.UP, 1));
            res.AddRange(action(piecePos, Directions.RIGHT, 1));
            res.AddRange(action(piecePos, Directions.DOWN, 1));
            res.AddRange(action(piecePos, Directions.LEFT, 1));
            return res;
        }

        private List<Move> GetPawnRawMoves(int piecePos, bool isEnemy)
        {
            List<Move> moves = new List<Move>();
            int dirMuti;
            bool isBeginPos;

            int leftEdge, rightEdge;

            if(isWhiteMove)
            {
                dirMuti = 1;
                leftEdge = 0;
                rightEdge = 7;
                isBeginPos = BoardMovementRestrainer.IsPositionBeginWhitePawnPosition(piecePos);
            }
            else
            {
                dirMuti = -1;
                leftEdge = 7;
                rightEdge = 0;
                isBeginPos = BoardMovementRestrainer.IsPositionBeginBlackPawnPosition(piecePos);
            }

            Action action = (isEnemy) ? CheckEnemySidePawnMoves : CheckCurrentSidePawnMoves;
            action();
            return moves;


            void CheckEnemySidePawnMoves()
            {
                dirMuti *= -1;
                //right capture
                if (piecePos % 8 != leftEdge) //right edge
                {
                    int rightForCapture = 9 * dirMuti + piecePos;
                    if (board.IsPositionInsideBoard(rightForCapture))
                    {
                        enemyAttackZone[rightForCapture] = true;
                        if (board.CheckIfCellHavePieceWithGivenColor(rightForCapture, isWhiteMove) && IsKingOnPos(rightForCapture))
                            checkPossiblePos = new HashSet<int> { piecePos };
                    }
                }

                //left capture
                if (piecePos % 8 != rightEdge)
                {
                    int leftForCapture = 7 * dirMuti + piecePos;
                    if (board.IsPositionInsideBoard(leftForCapture))
                    {
                        enemyAttackZone[leftForCapture] = true;
                        if (board.CheckIfCellHavePieceWithGivenColor(leftForCapture, isWhiteMove) && IsKingOnPos(leftForCapture))
                            checkPossiblePos = new HashSet<int> { piecePos };
                    }
                }
            }

            void CheckCurrentSidePawnMoves()
            {
                //single forward
                int singleForPos = 8 * dirMuti + piecePos;
                if (board.IsPositionInsideBoard(singleForPos) && !board.CheckIfCellHavePiece(singleForPos))
                {
                    moves.Add(new Move(piecePos, singleForPos));
                    
                    if (isBeginPos) //double forward
                    {
                        int pos = 16 * dirMuti + piecePos;
                        if (board.IsPositionInsideBoard(pos) && !board.CheckIfCellHavePiece(pos))
                        {
                            moves.Add(new Move(piecePos, pos).AddEnPassantPosition(singleForPos));
                        }
                    }
                }

                //right capture + enPassant
                if (piecePos % 8 != rightEdge) //right edge
                {
                    int rightForCapture = 9 * dirMuti + piecePos;
                    if (board.IsPositionInsideBoard(rightForCapture))
                    {
                        if (board.CheckIfCellHavePieceWithGivenColor(rightForCapture, !isWhiteMove))
                            moves.Add(new Move(piecePos, rightForCapture).AddAffectedPiece(rightForCapture, null, board.GetCellCode(rightForCapture)));
                        else if (game.IsEnPassantPosition(out int enPassant) && enPassant == rightForCapture)
                        {
                            int affectedPos = rightForCapture + 8 * dirMuti * -1;
                            if(board.CheckIfCellHavePieceWithGivenColor(affectedPos, !isWhiteMove))
                                moves.Add(new Move(piecePos, rightForCapture).AddAffectedPiece(affectedPos, null, board.GetCellCode(rightForCapture + 8 * dirMuti * -1))
                                    .AddEnPassantPosition(rightForCapture));
                        }

                    }
                }

                //left capture + enPassant
                if (piecePos % 8 != leftEdge)
                {
                    int leftForCapture = 7 * dirMuti + piecePos;
                    if (board.IsPositionInsideBoard(leftForCapture))
                    {
                        if (board.CheckIfCellHavePieceWithGivenColor(leftForCapture, !isWhiteMove))
                            moves.Add(new Move(piecePos, leftForCapture).AddAffectedPiece(leftForCapture, null, board.GetCellCode(leftForCapture)));
                        else if (game.IsEnPassantPosition(out int enPassant) && enPassant == leftForCapture)
                        {
                            int affectedPos = leftForCapture + 8 * dirMuti * -1;
                            if (board.CheckIfCellHavePieceWithGivenColor(affectedPos, !isWhiteMove))
                                moves.Add(new Move(piecePos, leftForCapture).AddAffectedPiece(affectedPos, null, board.GetCellCode(leftForCapture + 8 * dirMuti * -1))
                                    .AddEnPassantPosition(leftForCapture));
                        }
                    }
                }

            }
        }

        #endregion RAW MOVES


        private List<Move> CheckCurrentSideLongMoves(int piecePos, Directions direction, int length = Board.BOARD_SINGLE_ROW_SIZE - 1)
        {
            List<Move> moves = new();
            int dir = (int)direction;


            int endPos = BoardMovementRestrainer.GetSlidingEndCellPosition(piecePos, direction);
            if (piecePos == endPos) return moves;

            for (int i = 1; i <= length; i++)
            {
                int checkPos = piecePos + i * dir;
                if (piecePos == checkPos) continue;

                if(!board.CheckIfCellHavePiece(checkPos)) //no piece -> can move
                {
                    moves.Add(new Move(piecePos, checkPos));
                    //continue;
                }
                else if (board.CheckIfCellHavePieceWithGivenColor(checkPos, isWhiteMove)) //friendly piece
                {
                    break;
                }
                else //enemy piece
                {
                    moves.Add(new Move(piecePos, checkPos).AddAffectedPiece(checkPos, null, board.GetCellCode(checkPos)));
                    break;
                }
                if (checkPos == endPos) break;
            }

            return moves;
        }

        private bool IsKingOnPos(int position)
        {
            return board.CheckIfCellHavePieceOfGivenClass(position, PieceClasses.KING);
        }

        ///!!!! After enPassantcapture there might be situation when king is left open (Position 3)
        private List<Move> CheckEnemySideLongMoves(int piecePos, Directions direction, int length = Board.BOARD_SINGLE_ROW_SIZE - 1)
        {
            List<Move> dummy = new();

            int endPos = BoardMovementRestrainer.GetSlidingEndCellPosition(piecePos, direction);
            if (piecePos == endPos) return dummy;

            int dir = (int)direction;

            HashSet<int> posInRadius = new();
            posInRadius.Add(piecePos);

            int friendlyPiecePos = -1;
            int enemyPiecePos = -1;
            int enemyKingPos = -1;

            //if (piecePos == 26 && direction == Directions.RIGHT)
            //{
            //    Console.WriteLine("s");
            //}
            for (int i = 1; i <= length; i++)
            {
                int checkPos = piecePos + i * dir;
                if (piecePos == checkPos) continue; //?


                if(!board.CheckIfCellHavePiece(checkPos) && friendlyPiecePos == -1 && enemyPiecePos == -1) //no piece and no friendly and enemy piece met
                {
                    enemyAttackZone[checkPos] = true;
                    posInRadius.Add(checkPos);
                }
                else if (board.CheckIfCellHavePieceWithGivenColor(checkPos, !isWhiteMove)) //friendly piece
                {
                    if (friendlyPiecePos != -1) return dummy; //two friendly pieces in row -> skip
                    
                    if (enemyPiecePos == -1)
                    {
                        enemyAttackZone[checkPos] = true;
                    }

                    friendlyPiecePos = checkPos;
                }
                else if(board.CheckIfCellHavePieceWithGivenColor(checkPos, isWhiteMove))
                {
                    if (IsKingOnPos(checkPos)) //is possible check
                    {
                        enemyKingPos = checkPos;
                    }
                    else //normal enemy piece
                    {
                        if (enemyPiecePos != -1) return dummy; //two enemy pieces in row -> skip

                        if (friendlyPiecePos == -1)
                        {
                            enemyAttackZone[checkPos] = true;
                        }

                        enemyPiecePos = checkPos;
                    }
                }
               
                if (checkPos == endPos || enemyKingPos != -1) break;
                posInRadius.Add(checkPos);
            }

            

            if(enemyKingPos != -1 && enemyPiecePos == -1 && friendlyPiecePos == -1)//check
            {
                checkPossiblePos = posInRadius;
            }
            else if(enemyKingPos != -1 && enemyPiecePos != -1 && friendlyPiecePos == -1)//pin
            {
                pinnedPieces.Add(enemyPiecePos, posInRadius);
            }
            else if(enemyKingPos != -1 && enemyPiecePos != -1 && friendlyPiecePos != -1)//check enPassant invalid take
            {
                if(!game.IsEnPassantPosition(out int gameEnPassantPos)) return dummy;

                int localEn = (isWhiteMove) ? friendlyPiecePos + 8 : friendlyPiecePos - 8;
                if(localEn == gameEnPassantPos && board.CheckIfCellHavePieceOfGivenClass(enemyPiecePos, PieceClasses.PAWN))
                {
                    enPassantPawnPinnedPos = enemyPiecePos;
                }
            }

            return dummy;
        }
    }
}

