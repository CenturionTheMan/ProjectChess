using ChessLibrary.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;

namespace ChessLibrary.Engine
{
    public abstract class BoardEntityFactory
    {
        private const uint PIECE_MASK = 0b00000000_00000000_00000000_1111_1111;
        private const uint COLOR_MASK = 0b11000000_00000000_00000000_0000_0000;

        public BoardEntityFactory()
        {

        }

        /// <summary>
        /// Checks if the given piece code represents a piece on the chess board.
        /// </summary>
        /// <param name="pieceCode">The piece code to check.</param>
        /// <returns>True if the piece code represents a piece, otherwise false.</returns>
        public static bool CheckIfPiece(uint pieceCode)
        {
            if (pieceCode == 0) return false;
            else return true;
        }

        /// <summary>
        /// Checks if the given piece code represents a piece of the specified color on the chess board.
        /// </summary>
        /// <param name="pieceCode">The piece code to check.</param>
        /// <param name="color">The color to match.</param>
        /// <returns>True if the piece code represents a piece with the given color, otherwise false.</returns>
        public static bool CheckIfPieceWithGivenColor(uint pieceCode, ChessColors color)
        {
            if (pieceCode == 0) return false;

            var onlyColorCode = pieceCode & COLOR_MASK;

            return (onlyColorCode == GetColorCode(color));
        }

        /// <summary>
        /// Checks if the given piece code represents a piece on the chess board and retrieves its color.
        /// </summary>
        /// <param name="pieceCode">The piece code to check.</param>
        /// <param name="color">The color of the piece, if it represents a piece.</param>
        /// <returns>True if the piece code represents a piece, otherwise false.</returns>
        public static bool CheckIfPiece(uint pieceCode, out ChessColors color)
        {
            color = ChessColors.NONE;

            if (pieceCode == 0) return false;

            var onlyColorCode = pieceCode & COLOR_MASK;

            color = (ChessColors)onlyColorCode;
            return true;
        }


        /// <summary>
        /// Checks if the given piece code represents a piece on the chess board and retrieves its piece class and color.
        /// </summary>
        /// <param name="pieceCode">The piece code to check.</param>
        /// <param name="piece">The piece represented by the piece code, if it represents a piece.</param>
        /// <param name="color">The color of the piece, if it represents a piece.</param>
        /// <returns>True if the piece code represents a piece, otherwise false.</returns>
        public static bool CheckIfPiece(uint pieceCode, out PieceClasses piece, out ChessColors color)
        {
            piece = PieceClasses.NONE;
            color = ChessColors.NONE;

            if (pieceCode == 0) return false;

            var onlyPieceCode = pieceCode & PIECE_MASK;
            var onlyColorCode = pieceCode & COLOR_MASK;

            piece = (PieceClasses)onlyPieceCode;
            color = (ChessColors)onlyColorCode;
            return true;
        }

        /// <summary>
        /// Creates an empty piece code.
        /// </summary>
        /// <returns>An empty piece code.</returns>
        public static uint CreateEmpty() { return 0; }

        /// <summary>
        /// Gets the piece code for the specified chess piece class.
        /// </summary>
        /// <param name="piece">The chess piece class to get the code for.</param>
        /// <returns>The piece code corresponding to the specified chess piece.</returns>
        public static uint GetPieceCode(PieceClasses piece) { return (uint)piece; }

        /// <summary>
        /// Gets the color code for the specified chess color.
        /// </summary>
        /// <param name="color">The chess color to get the code for.</param>
        /// <returns>The color code corresponding to the specified chess color.</returns>
        public static uint GetColorCode(ChessColors color) { return (uint)color; }

        /// <summary>
        /// Creates a piece code for the given chess piece class and color.
        /// </summary>
        /// <param name="piece">The chess piece class to create the code for.</param>
        /// <param name="color">The chess color of the piece.</param>
        /// <returns>The piece code representing the specified chess piece and color.</returns>
        public static uint CreatePiece(PieceClasses piece, ChessColors color)
        {
            return ((uint)piece) | ((uint)color);
        }
    }

}
