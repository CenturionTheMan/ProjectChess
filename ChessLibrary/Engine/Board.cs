using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Engine
{
    public class Board
    {
        public const int BOARD_SINGLE_ROW_SIZE = 8;
        public const int BOARD_SIZE = 64;

        private uint[] boardCells;

        public Board()
        {
            boardCells = new uint[BOARD_SIZE];
        }

        public void PlacePiece(PieceClasses pieceClass, ChessColors pieceColor, int piecePos)
        {
            var piece = BoardEntityFactory.CreatePiece(pieceClass, pieceColor);
            boardCells[piecePos] = piece;
        }

        public void RemovePiece(int piecePos)
        {
            boardCells[piecePos] = BoardEntityFactory.CreateEmpty();
        }

        public uint GetCellCode(int pos)
        {
            return boardCells[pos];
        }

        public uint GetCellCode(int x, int y)
        {
            return boardCells[x + y*8];
        }
    }
}
