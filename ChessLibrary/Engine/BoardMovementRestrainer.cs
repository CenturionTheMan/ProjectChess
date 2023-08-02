using ChessLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Engine
{
    internal static class BoardMovementRestrainer
    {
        private static int[,] slidingEndCells;
        private static int[][] knightsValidCells;
        private static (int low, int high) whitePawnsOriginCells = (7, 16);
        private static (int low, int high) blackPawnsOriginCells = (47, 56);


        //private static readonly Vec2[] knightOffsets = { new Vec2(1,2), new Vec2(2,1), new Vec2(2,-1), new Vec2(1,-2), new Vec2(-1,-2), new Vec2(-2,-1), new Vec2(-2,1), new Vec2(-1,2) };


        static BoardMovementRestrainer()
        {
            var directionsOffsets = Enum.GetValues(typeof(Directions)).Cast<int>();
            //values = values.Select(v => v = v + 9);

            slidingEndCells = new int[Board.BOARD_SIZE, 19];

            for (int position = 0; position < Board.BOARD_SIZE; position++)
            {
                foreach (var dir in directionsOffsets)
                {
                    Vec2 pos = SingleDimPosToVec2(position);
                    Vec2 offset = DirectionToVec2((Directions)dir);

                    while (pos.X + offset.X >= 0 && pos.X + offset.X < Board.BOARD_SINGLE_ROW_SIZE && pos.Y + offset.Y >= 0 && pos.Y + offset.Y < Board.BOARD_SINGLE_ROW_SIZE)
                    {
                        pos = pos + offset;
                    }

                    slidingEndCells[position, dir + 9] = pos.ToBoardPosition();
                }
            }


            //KINGHTS
            knightsValidCells = new int[Board.BOARD_SIZE][];
            for (int position = 0; position < Board.BOARD_SIZE; position++)
            {
                if (position == 18)
                {
                    Console.WriteLine("???");
                }

                List<int> tmp = new();
                for (int i = -2; i <= 2; i++)
                {
                    for (int j = -2; j <= 2; j++)
                    {
                        if (i == j || i == -j) continue;
                        if (i == 0 || j == 0) continue;
                        Vec2 pos = SingleDimPosToVec2(position);
                        var offset = new Vec2(i, j);

                        if (pos.X + offset.X >= 0 && pos.X + offset.X < Board.BOARD_SINGLE_ROW_SIZE && pos.Y + offset.Y >= 0 && pos.Y + offset.Y < Board.BOARD_SINGLE_ROW_SIZE)
                        {
                            var toAddVec = pos + offset;
                            int singleDimPos = toAddVec.ToBoardPosition();
                            tmp.Add(singleDimPos);
                        }
                    }
                }
                knightsValidCells[position] = tmp.ToArray();
            }
        }

        private static Vec2 DirectionToVec2(Directions direction)
        {
            switch (direction)
            {
                case Directions.UP:
                    return new Vec2(0,1);
                case Directions.RIGHT:
                    return new Vec2(1, 0);
                case Directions.DOWN:
                    return new Vec2(0, -1);
                case Directions.LEFT:
                    return new Vec2(-1, 0);
                case Directions.UP_RIGHT:
                    return new Vec2(1, 1);
                case Directions.UP_LEFT:
                    return new Vec2(-1, 1);
                case Directions.DOWN_RIGHT:
                    return new Vec2(1, -1);
                case Directions.DOWN_LEFT:
                    return new Vec2(-1, -1);
                default:
                    throw new NotImplementedException();
            }
        }

        public static void ForceCTOR() { GetSlidingEndCellPosition(0, Directions.UP); GetKnightsValidMoveCells(0); }

        private static Vec2 SingleDimPosToVec2(int pos)
        {
            int posY = (int)((uint)pos >> 3);
            //int posY = pos/8;
            int posX = pos - posY * 8;
            return new Vec2(posX, posY);
        }

        public static int GetSlidingEndCellPosition(int position, Directions direction)
        {
            return slidingEndCells[position, (int)direction+9];
        }

        public static int[] GetKnightsValidMoveCells(int position)
        {
            return knightsValidCells[position];
        }

        public static bool IsPositionBeginWhitePawnPosition(int position)
        {
            return position > whitePawnsOriginCells.low && position < whitePawnsOriginCells.high;
        }

        public static bool IsPositionBeginBlackPawnPosition(int position)
        {
            return position > blackPawnsOriginCells.low && position < blackPawnsOriginCells.high;
        }
    }
}
