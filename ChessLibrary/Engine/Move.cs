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

        private uint flags = 0b0000_0000_0000_0000_0000_0000_0000_0000;

        //public readonly uint ToMovePiece;


        public int? AffectedFromPos = null;
        public int? AffectedToPos = null;




        public Move(int from, int to)
        {
            FromPos = from;
            ToPos = to;
        }


        public Move AddAffectedPiece(int affectedFromPos, int affectedToPos)
        {
            this.AffectedToPos = affectedToPos;
            this.AffectedFromPos = affectedFromPos;
            return this;
        }
    }
}
