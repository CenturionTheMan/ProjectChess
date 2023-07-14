using System.Collections;
using System.Collections.Generic;

namespace Chess.Engine
{
    public enum PieceClasses
    {
        KING,
        QUEEN,
        ROOK,
        BISHOP,
        PAWN,
        KNIGHT
    }

    public enum ChessColors
    {
        BLACK,
        WHITE,
    }

    public enum Promotions
    {
        TO_QUEEN,
        TO_KNIGHT,
        TO_ROOK,
        TO_BISHOP,
        NONE
    }
}
