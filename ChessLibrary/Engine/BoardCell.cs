using ChessLibrary.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public class BoardCell
    {
        public readonly Vec2 Position;

        private BoardEntityFactory? piece;

        public BoardCell(Vec2 position, BoardEntityFactory? piece = null)
        {
            this.Position = position;
            this.piece = piece;
        }

        public BoardCell(int x, int y, BoardEntityFactory? piece = null)
        {
            this.Position = new Vec2(x, y);
            this.piece = piece;
        }

        public BoardEntityFactory? GetPiece()
        {
            return piece;
        }

        public bool HasPiece(out BoardEntityFactory piece)
        {
            piece = this.piece;
            return this.piece != null;
        }

        public bool HasPiece()
        {
            return piece != null;
        }

        public void SetPiece(BoardEntityFactory piece)
        {
            this.piece = piece;
        }

        public BoardEntityFactory? RemovePiece()
        {
            BoardEntityFactory? tmp = piece;
            piece = null;
            return tmp;
        }
    }

}
