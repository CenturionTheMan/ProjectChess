using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary.Utilities
{
    public class Vec2
    {
        public readonly static Vec2 Right = new Vec2(1, 0);
        public readonly static Vec2 Left = new Vec2(-1, 0);
        public readonly static Vec2 Up = new Vec2(0, 1);
        public readonly static Vec2 Down = new Vec2(0, -1);
        public readonly static Vec2 Zero = new Vec2(0, 0);



        public int X
        {
            get { return x; }
            set { x = value; }
        }
        private int x;

        public int Y
        {
            get { return y; }
            set { y = value; }
        }
        private int y;



        public Vec2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vec2(Vec2 vec): this(vec.x, vec.y)
        {

        }


        public bool Equals(int x, int y)
        {
            return this.x == x && this.y == y;
        }


        public static Vec2 operator +(Vec2 a, Vec2 b)
        {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        public static bool operator ==(Vec2 a, Vec2 b)
        {
            if (a is null)
            {
                return b is null;
            }
            if (b is null)
            {
                return a is null;
            }

            return a.X == b.X && a.Y == b.Y;
        }

        public static bool operator !=(Vec2 a, Vec2 b)
        {
            if (a is null)
            {
                return b is not null;
            }
            if (b is null)
            {
                return a is not null;
            }


            return a.X != b.X || a.Y != b.Y;
        }

        public override bool Equals(object? obj)
        {
            if(obj is Vec2)
            {
                var tmp = (Vec2)obj;
                return this.X == tmp.X && this.Y == tmp.Y;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return 31 * x + 17 * y;
        }
    }
}
