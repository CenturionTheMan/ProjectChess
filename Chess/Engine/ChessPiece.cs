using Chess.Utilities;
using System.Collections;
using System.Collections.Generic;

namespace Chess.Engine
{
    public class ChessPiece
    {
        public readonly ChessColors PieceColor;

        public PieceClasses PieceClass;

        public Vec2 Position { get { return position; } set { position = value; } }

        private Vec2 position;


        public ChessPiece(PieceClasses pieceClass, ChessColors pieceColor, Vec2 position)
        {
            this.PieceClass = pieceClass;
            PieceColor = pieceColor;
            this.position = position;
        }

        public ChessPiece(PieceClasses pieceClass, ChessColors pieceColor, int xPos, int yPos) : this(pieceClass, pieceColor, new Vec2(xPos, yPos))
        {

        }

        public void SetPosition(int x, int y)
        {
            position.X = x;
            position.Y = y;
        }



    }

}
