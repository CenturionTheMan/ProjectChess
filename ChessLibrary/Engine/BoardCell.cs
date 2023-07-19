using ChessLibrary.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    public class BoardCell
    {
        public readonly Vec2 Position;

        private ChessPiece? piece;

        public BoardCell(Vec2 position, ChessPiece? piece = null)
        {
            this.Position = position;
            this.piece = piece;
        }

        public BoardCell(int x, int y, ChessPiece? piece = null)
        {
            this.Position = new Vec2(x, y);
            this.piece = piece;
        }

        public ChessPiece? GetPiece()
        {
            return piece;
        }

        public bool HasPiece(out ChessPiece piece)
        {
            piece = this.piece;
            return this.piece != null;
        }

        public bool HasPiece()
        {
            return piece != null;
        }

        public void SetPiece(ChessPiece piece)
        {
            this.piece = piece;
        }

        public ChessPiece? RemovePiece()
        {
            ChessPiece? tmp = piece;
            piece = null;
            return tmp;
        }
    }

}
