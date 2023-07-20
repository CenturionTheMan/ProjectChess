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
        HashSet<int>? checkPossiblePos;
        private Dictionary<int,HashSet<int>> pinnedPieces; //piece from which pos is pinned / where it can move
        private bool[] enemyAttackZone;
        private bool isWhiteMove;

        //private readonly int[] knightOffsets = { 17, 10, -6, -15, -10, -17, 6, 15};

        public MovesValidator(Game game, Board board)
        {
            this.game = game;
            this.board = board;
            Setup();
        }

        private void Setup()
        {
            sudoValidMoves = new();
            checkPossiblePos = null;
            pinnedPieces = new();
            isWhiteMove = game.GetCurrentlyPlayingSide() == ChessColors.WHITE ? true : false;
            enemyAttackZone = new bool[Board.BOARD_SIZE];
        }

        public Move[] GetValidMoves()
        {
            Setup();

            SetupSudoValidMoves();

            return sudoValidMoves.ToArray();
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
                        break;
                    case PieceClasses.QUEEN:
                        GetQueenRawMoves(piecePos, isEnemy);
                        break;
                    case PieceClasses.ROOK:
                        GetRookRawMoves(piecePos, isEnemy);
                        break;
                    case PieceClasses.BISHOP:
                        GetBishopRawMoves(piecePos, isEnemy);
                        break;
                    case PieceClasses.PAWN:
                        break;
                    case PieceClasses.KNIGHT:
                        GetKnightRawMoves(piecePos, isEnemy);
                        break;
                    case PieceClasses.NONE:
                        break;
                    default:
                        break;
                }
            }
        }


        #region RAW MOVES

        private void GetRookRawMoves(int piecePos, bool isEnemy)
        {
            Action<int, Directions, int> action = (isEnemy)? CheckEnemySideLongMoves : CheckCurrentSideLongMoves;
            action(piecePos, Directions.UP, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.RIGHT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.DOWN, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.LEFT, Board.BOARD_SINGLE_ROW_SIZE);
        }

        private void GetBishopRawMoves(int piecePos, bool isEnemy)
        {
            Action<int, Directions, int> action = (isEnemy) ? CheckEnemySideLongMoves : CheckCurrentSideLongMoves;
            action(piecePos, Directions.UP_RIGHT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.UP_LEFT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.DOWN_RIGHT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.DOWN_LEFT, Board.BOARD_SINGLE_ROW_SIZE);
        }

        private void GetQueenRawMoves(int piecePos, bool isEnemy)
        {
            Action<int, Directions, int> action = (isEnemy) ? CheckEnemySideLongMoves : CheckCurrentSideLongMoves;
            action(piecePos, Directions.UP_RIGHT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.UP_LEFT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.DOWN_RIGHT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.DOWN_LEFT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.UP, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.RIGHT, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.DOWN, Board.BOARD_SINGLE_ROW_SIZE);
            action(piecePos, Directions.LEFT, Board.BOARD_SINGLE_ROW_SIZE);
        }

        private void GetKnightRawMoves(int piecePos, bool isEnemy)
        {
            Action action = (isEnemy) ? CheckEnemySideKnightMoves : CheckCurrentSideKnightMoves;
            action();


            void CheckCurrentSideKnightMoves()
            {
                var toCheckPosition = BoardMovementRestrainer.GetKnightsValidMoveCells(piecePos);

                foreach (var checkPos in toCheckPosition)
                {
                    if (!board.CheckIfCellHavePiece(checkPos)) //no piece -> can move
                    {
                        sudoValidMoves.Add(new Move(piecePos, checkPos));
                    }
                    else if (board.CheckIfCellHavePieceWithGivenColor(checkPos, isWhiteMove)) //friendly piece
                    {
                        return;
                    }
                    else //enemy piece
                    {
                        sudoValidMoves.Add(new Move(piecePos, checkPos).AddAffectedPiece(checkPos, null));
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

        #endregion RAW MOVES

        private void CheckCurrentSideLongMoves(int piecePos, Directions direction, int length = Board.BOARD_SINGLE_ROW_SIZE - 1)
        {
            int dir = (int)direction;

            int endPos = BoardMovementRestrainer.GetSlidingEndCellPosition(piecePos, direction);
            if (piecePos == endPos) return;

            for (int i = 1; i <= length; i++)
            {
                int checkPos = piecePos + i * dir;
                if (piecePos == checkPos) continue;

                if(!board.CheckIfCellHavePiece(checkPos)) //no piece -> can move
                {
                    sudoValidMoves.Add(new Move(piecePos, checkPos));

                    //continue;
                }
                else if (board.CheckIfCellHavePieceWithGivenColor(checkPos, isWhiteMove)) //friendly piece
                {
                    break;
                }
                else //enemy piece
                {
                    sudoValidMoves.Add(new Move(piecePos, checkPos).AddAffectedPiece(checkPos, null));
                    break;
                }
                if (checkPos == endPos) break;
            }
        }

        private void CheckEnemySideLongMoves(int piecePos, Directions direction, int length = Board.BOARD_SINGLE_ROW_SIZE - 1)
        {
            int dir = (int)direction;

            int endPos = BoardMovementRestrainer.GetSlidingEndCellPosition(piecePos, direction);
            if (piecePos == endPos) return;

            HashSet<int> posInRadius = new();
            int pinnedPos = -1;

            for (int i = 1; i <= length; i++)
            {
                int checkPos = piecePos + i * dir;

                if (piecePos == checkPos) continue;

                if (!board.CheckIfCellHavePiece(checkPos)) //no piece -> can move
                {
                    posInRadius.Add(checkPos);
                    enemyAttackZone[checkPos] = true;
                }
                else if (board.CheckIfCellHavePieceWithGivenColor(checkPos, !isWhiteMove)) //friendly piece
                {
                    return;
                }
                else //enemy piece
                {
                    enemyAttackZone[checkPos] = true;
                    if (board.CheckIfCellHavePieceOfGivenClass(checkPos, PieceClasses.KING))//possible check
                    {
                        posInRadius.Add(piecePos);
                        if (pinnedPos == -1) // there is no pin -> check
                        {
                            checkPossiblePos = posInRadius;
                            return;
                        }
                        else //pin
                        {
                            pinnedPieces.Add(pinnedPos, posInRadius);
                        }
                    }
                    else //possible pin
                    {
                        if (pinnedPos != -1) return;
                        pinnedPos = checkPos;
                    }
                }

                if (checkPos == endPos) break;
            }




        }
    }
}

