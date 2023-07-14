using Chess.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess.Engine
{
    public class Move
    {
        //public readonly bool IsBlocked;
        
        public readonly Vec2 FromPos;
        public readonly Vec2 ToPos;
        
        public ChessPiece? OtherPiece = null;

        public Promotions promotion = Promotions.NONE;
        public bool IsPromotion = false;
        public bool IsCastling = false;
        public bool IsPawnTwoForward = false;
        public bool IsEnPassantCapture = false;


        //public Move(int fromX, int fromY, int toX, int toY, bool isBlocked, Piece? capturedPiece) : this(new Vec2(fromX, fromY), new Vec2(toX, toY), isBlocked)
        //{
        //    this.CapturedPiece = capturedPiece;
        //}

        public Move(Vec2 from, Vec2 to, ChessPiece? capturedPiece = null)
        {
            FromPos = new Vec2(from);
            ToPos = new Vec2(to);
            //IsBlocked = isBlocked;
            OtherPiece = capturedPiece;
        }
    }
}
