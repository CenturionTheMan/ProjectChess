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
        public readonly int FromPos;
        public readonly int ToPos;

        //private uint flags = 0b0000_0000_0000_0000_0000_0000_0000_0000;


        private int? affectedFromPos = null;
        private int? affectedToPos = null;
        private uint affectedPiece = BoardEntityFactory.CreateEmpty();

        private int enPassantPosition = -1;

        private int[] castlingArrayIndex = new int[0];

        private PieceClasses promoteToPiece = PieceClasses.NONE;

        internal Move(int from, int to)
        {
            FromPos = from;
            ToPos = to;
        }


        internal Move AddAffectedPiece(int affectedFromPos, int? affectedToPos, uint affectedPiece)
        {
            this.affectedToPos = affectedToPos;
            this.affectedFromPos = affectedFromPos;
            this.affectedPiece = affectedPiece;
            return this;
        }

        internal Move AddEnPassantPosition(int enPassantPosition)
        {
            this.enPassantPosition = enPassantPosition;
            return this;
        }

        internal Move AddCastlingFlag(params int[] castlingArrayIndex)
        {
            this.castlingArrayIndex = castlingArrayIndex;
            return this;
        }

        internal Move AddPromotionFlag(PieceClasses pieceNewClass)
        {
            this.promoteToPiece = pieceNewClass;
            return this;
        }

        public bool TryGetPromotion(out PieceClasses newPieceCode)
        {
            newPieceCode = promoteToPiece;
            return promoteToPiece != PieceClasses.NONE;
        }

        public int[] GetCastlingArrayIndex()
        {
            return this.castlingArrayIndex;
        }

        public int GetEnPassantPosition()
        {
            return this.enPassantPosition;
        }

        public bool TryGetAffectedPiecePos(out int affectedFromPos,out int? affectedToPos)
        {
            affectedFromPos = this.affectedFromPos.GetValueOrDefault();
            affectedToPos = this.affectedToPos;
            return this.affectedFromPos != null;
        }

        public uint GetAffectedPiece()
        {
            return affectedPiece;
        }
    }
}
