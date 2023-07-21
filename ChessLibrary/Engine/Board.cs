using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Engine
{
    /// <summary>
    /// Represents a chess board and provides methods for placing and removing pieces/entities on the board.
    /// </summary>
    public class Board
    {
        public const int BOARD_SINGLE_ROW_SIZE = 8;
        public const int BOARD_SIZE = 64;

        private uint[] boardCells;
        private List<int> piecePositions;


        /// <summary>
        /// Initializes a new instance of the Board class.
        /// </summary>
        public Board()
        {
            boardCells = new uint[BOARD_SIZE];
            piecePositions = new();
        }

        public List<int> GetPiecePositions()
        {
            return piecePositions;
        }

        /// <summary>
        /// Places a piece of the specified class and color on the board at the given position.
        /// </summary>
        /// <param name="pieceClass">The class of the chess piece.</param>
        /// <param name="pieceColor">The color of the chess piece.</param>
        /// <param name="piecePos">The position on the board where the piece will be placed.</param>
        public void PlacePiece(PieceClasses pieceClass, ChessColors pieceColor, int piecePos)
        {
            var piece = BoardEntityFactory.CreatePiece(pieceClass, pieceColor);
            boardCells[piecePos] = piece;
            piecePositions.Add(piecePos);
        }

        /// <summary>
        /// Checks if the cell at the specified position contains a chess piece.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <returns>True if the cell contains a piece, otherwise false.</returns>
        public bool CheckIfCellHavePiece(int pos)
        {
            if (pos >= Board.BOARD_SIZE || pos < 0)
            {
                return false;
            }

            return BoardEntityFactory.CheckIfPiece(boardCells[pos]);
        }

        /// <summary>
        /// Checks if the cell at the specified position contains a chess piece and retrieves its class and color.
        /// </summary>
        /// <param name="pos">The position to check.</param>
        /// <param name="pieceClass">The class of the chess piece, if it exists in the cell.</param>
        /// <param name="pieceColor">The color of the chess piece, if it exists in the cell.</param>
        /// <returns>True if the cell contains a piece, otherwise false.</returns>
        public bool CheckIfCellHavePiece(int pos, out PieceClasses pieceClass, out ChessColors chessColors)
        {
            if (pos >= Board.BOARD_SIZE || pos < 0)
            {
                pieceClass = PieceClasses.NONE;
                chessColors = ChessColors.NONE;
                return false;
            }
            return BoardEntityFactory.CheckIfPiece(boardCells[pos], out pieceClass, out chessColors);
        }

        /// <summary>
        /// Places the specified entity code on the board at the given position.
        /// </summary>
        /// <param name="code">The entity code to place on the board.</param>
        /// <param name="pos">The position on the board where the entity will be placed.</param>
        public void PlaceEntity(uint code, int pos)
        {
            boardCells[pos] = code;
            if(BoardEntityFactory.CheckIfPiece(code))
                piecePositions.Add(pos);
        }

        /// <summary>
        /// Removes the chess piece from the specified position on the board.
        /// </summary>
        /// <param name="piecePos">The position on the board to remove the piece from.</param>
        public void RemovePiece(int piecePos)
        {
            piecePositions.Remove(piecePos);
            boardCells[piecePos] = BoardEntityFactory.CreateEmpty();
        }

        /// <summary>
        /// Gets the entity code present in the cell at the specified position on the board.
        /// </summary>
        /// <param name="pos">The position on the board to retrieve the entity code from.</param>
        /// <returns>The entity code present in the specified cell.</returns>
        public uint GetCellCode(int pos)
        {
            return boardCells[pos];
        }

        public bool IsPositionInsideBoard(int position)
        {
            return position >= 0 && position < Board.BOARD_SIZE;
        }

        public bool CheckIfCellHavePieceWithGivenColor(int pos, bool isWhite)
        {
            ChessColors color = (isWhite) ? ChessColors.WHITE : ChessColors.BLACK;
            return BoardEntityFactory.CheckIfPieceWithGivenColor(GetCellCode(pos), color);
        }

        public bool CheckIfCellHavePieceWithGivenColor(int pos, ChessColors color)
        {
            return BoardEntityFactory.CheckIfPieceWithGivenColor(GetCellCode(pos), color);
        }

        public bool CheckIfCellHavePieceOfGivenClass(int pos, PieceClasses pClass)
        {
            return pClass == BoardEntityFactory.GetPieceClass(GetCellCode(pos));
        }

        /// <summary>
        /// Gets the entity code present in the cell at the specified (x, y) position on the board.
        /// </summary>
        /// <param name="x">The x-coordinate of the position.</param>
        /// <param name="y">The y-coordinate of the position.</param>
        /// <returns>The entity code present in the specified cell.</returns>
        public uint GetCellCode(int x, int y)
        {
            return boardCells[x + y * 8];
        }
    }
}
