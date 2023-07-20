using ChessLibrary.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Engine
{
    public static class BoardMovementRestrainer
    {
        private static int[,] slidingEndCells;
        private static int[][] knightsValidCells;

        private static readonly Vec2[] knightOffsets = { new Vec2(1,2), new Vec2(1, -2), new Vec2(-1, 2), new Vec2(-1, -2), new Vec2(2, 1), new Vec2(2, -1), new Vec2(-2, 1), new Vec2(-2, -1) };


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


            knightsValidCells = new int[Board.BOARD_SIZE][];
            for (int position = 0; position < Board.BOARD_SIZE; position++)
            {
                List<int> tmp = new();
                foreach (var knightOffset in knightOffsets)
                {
                    Vec2 origin = SingleDimPosToVec2(position);
                    Vec2 offset = knightOffset;
                    Vec2 pos = origin + offset;
                    if (pos.X < 0 || pos.X >= Board.BOARD_SINGLE_ROW_SIZE || pos.Y < 0 || pos.Y >= Board.BOARD_SINGLE_ROW_SIZE) continue;

                    tmp.Add(pos.ToBoardPosition());
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

        public static void ForceCTOR() { }

        private static Vec2 SingleDimPosToVec2(int pos)
        {
            //int posY = (int)((uint)pos >> 3);
            int posY = pos/8;
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
    }
}
