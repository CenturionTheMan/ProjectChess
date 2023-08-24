using ChessLibrary.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    /// <summary>
    /// Provides methods to handle FEN (Forsyth-Edwards Notation) strings for setting up a chess board.
    /// </summary>
    public static class ChessStringsHandler
    {
        /// <summary>
        /// Retrieves the chess board setup and game state from a given FEN string.
        /// </summary>
        /// <param name="fen">The FEN string representing the chess board setup and game state.</param>
        /// <param name="board">The chess board with piece codes.</param>
        /// <param name="currentSide">The side to move (current turn).</param>
        /// <param name="castling">An array indicating castling availability for both sides.</param>
        /// <param name="enPassantPosition">The position where en passant capture is possible, if any.</param>
        /// <param name="halfmoves">The number of half-moves since the last capture or pawn advance.</param>
        /// <param name="fullmove">The number of full moves made in the game so far.</param>
        /// <returns>True if the FEN string is valid and successfully parsed, otherwise false.</returns>
        public static bool GetSetupFromFen(string fen, out uint[] board, out ChessColors? currentSide,
                                           out bool[] castling, out int enPassantPosition, out int halfmoves, out int fullmove)
        {
            board = null;
            currentSide = null;
            castling = null;
            enPassantPosition = -1;
            halfmoves = -1;
            fullmove = -1;

            try
            {
                var toHandle = fen.Split(" ");
                board = new uint[Board.BOARD_SIZE];
                castling = new bool[4];

                int offsetPos = 8 * 7;
                for (int i = 0; i < toHandle[0].Length; i++)
                {
                    char c = toHandle[0][i];
                    if (c == '/')
                    {
                        offsetPos -= 8 * 2;
                        continue;
                    }
                    if (c >= 48 && c <= 57)
                    {
                        int offset = c - 48;
                        offsetPos += offset;
                        continue;
                    }
                    var piece = CharToPiece(c);
                    board[offsetPos] = piece;
                    offsetPos++;
                }

                currentSide = toHandle[1] == "w" ? ChessColors.WHITE : ChessColors.BLACK;

                castling[0] = toHandle[2].Contains("K");
                castling[1] = toHandle[2].Contains("Q");
                castling[2] = toHandle[2].Contains("k");
                castling[3] = toHandle[2].Contains("q");

                if (!toHandle[3].Contains("-") && !string.IsNullOrEmpty(toHandle[3]))
                {
                    int x = BoardLetterToNum(toHandle[3][0]);
                    int y = toHandle[3][1] - 48;
                    enPassantPosition = x + y * 8;
                }

                if (toHandle.Length < 5 || toHandle[4] == String.Empty)
                {
                    halfmoves = 0;
                    fullmove = 0;
                }
                else
                {
                    if(!int.TryParse(toHandle[4], out halfmoves))
                    {
                        halfmoves = 0;
                    }
                    if (!int.TryParse(toHandle[5], out fullmove))
                    {
                        fullmove = 0;
                    }

                }

                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        /// <summary>
        /// Converts a board letter (e.g., 'a', 'b', 'c') to its corresponding column number (0-7).
        /// </summary>
        /// <param name="c">The board letter character.</param>
        /// <returns>The column number corresponding to the board letter.</returns>
        public static int BoardLetterToNum(char c)
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

        public static string BoardPositionToString(int pos)
        {
            Vec2 grid = new Vec2(pos);
            string horizontal = grid.X switch
            {
                0 => "a",
                1 => "b",
                2 => "c",
                3 => "d",
                4 => "e",
                5 => "f",
                6 => "g",
                7 => "h",
                _ => throw new NotImplementedException()
            };
            string vertical = grid.Y.ToString();
            return horizontal + vertical;
        }

        /// <summary>
        /// Converts a character representing a chess piece to its corresponding piece code.
        /// </summary>
        /// <param name="c">The character representing a chess piece.</param>
        /// <returns>The piece code corresponding to the chess piece character.</returns>
        private static uint CharToPiece(char c)
        {
            uint tmp = 0;
            switch (c)
            {
                case 'r':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.ROOK, ChessColors.BLACK);
                    break;
                case 'R':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.ROOK, ChessColors.WHITE);
                    break;

                case 'n':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.KNIGHT, ChessColors.BLACK);
                    break;
                case 'N':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.KNIGHT, ChessColors.WHITE);
                    break;

                case 'b':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.BISHOP, ChessColors.BLACK);
                    break;
                case 'B':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.BISHOP, ChessColors.WHITE);
                    break;

                case 'q':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.QUEEN, ChessColors.BLACK);
                    break;
                case 'Q':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.QUEEN, ChessColors.WHITE);
                    break;

                case 'k':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.KING, ChessColors.BLACK);
                    break;
                case 'K':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.KING, ChessColors.WHITE);
                    break;

                case 'p':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.PAWN, ChessColors.BLACK);
                    break;
                case 'P':
                    tmp = BoardEntityFactory.CreatePiece(PieceClasses.PAWN, ChessColors.WHITE);
                    break;

                default:
                    throw new NotImplementedException();
            }

            return tmp;
        }
    }
}
