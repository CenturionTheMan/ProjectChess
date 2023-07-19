using ChessLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Engine
{
    public class Move
    {
        //public readonly bool IsBlocked;
        
        public readonly Vec2 FromPos;
        public readonly Vec2 ToPos;
        
        public ChessPiece ToMovePiece;
        public ChessPiece? OtherPiece = null;

        public Promotions promotion = Promotions.NONE;
        public bool IsPromotion = false;
        public bool IsCastling { get; private set; }

        public bool IsPawnTwoForward = false;
        public bool IsEnPassantCapture = false;

        public int FiftyMoveRuleToThisMove = -1;

        public int OrderInPlayedMoves = -1;

        private Vec2 triggerPos = null;



        public Move(Vec2 from, Vec2 to, ChessPiece toMovePiece, ChessPiece? capturedPiece)
        {
            FromPos = new Vec2(from);
            ToPos = new Vec2(to);
            ToMovePiece = toMovePiece;
            OtherPiece = capturedPiece;

            IsCastling = false;
        }

        public void SetCastling(Vec2 triggerPos)
        {
            IsCastling = true;
            this.triggerPos = triggerPos;
        }

        public Vec2 GetTriggerPos()
        {
            return (IsCastling)? triggerPos : ToPos;
        }
    }
}
