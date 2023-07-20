using System.Collections;
using System.Collections.Generic;

namespace ChessLibrary.Engine
{
    [Flags]
    public enum PieceClasses : uint
    {
        KING =      0b00000000_00000000_00000000_0000_0001,
        QUEEN =     0b00000000_00000000_00000000_0000_0010,
        ROOK =      0b00000000_00000000_00000000_0000_0100,
        BISHOP =    0b00000000_00000000_00000000_0000_1000,
        PAWN =      0b00000000_00000000_00000000_0001_0000,
        KNIGHT =    0b00000000_00000000_00000000_0010_0000,
        NONE =      0,
    }

    [Flags]
    public enum ChessColors : uint
    {
        BLACK = 0b0100_0000_00000000_00000000_00000000,
        WHITE = 0b1000_0000_00000000_00000000_00000000,
        NONE = 0,
    }

    //public enum Promotions
    //{
    //    TO_QUEEN,
    //    TO_KNIGHT,
    //    TO_ROOK,
    //    TO_BISHOP,
    //    NONE
    //}

    public enum Directions
    {
        UP = 8,
        RIGHT = 1,
        DOWN = -8,
        LEFT = -1,
        UP_RIGHT = 9,
        UP_LEFT = 7,
        DOWN_RIGHT = -7,
        DOWN_LEFT = -9,
    }

    public enum GameResult
    {
        NONE,
        WHITE_WON,
        BLACK_WON,
        DRAW
    }
}
