using Protocol;

namespace Generator.Debugger
{
    [MessageObject]
    public struct Vector2Int
    {
        private int _x;
        private int _y;

        public int X { get => _x; set => _x = value; }
        public int Y { get => _y; set => _y = value; }

        public static Vector2Int Zero => new Vector2Int(0, 0);
        public static Vector2Int UnitX => new Vector2Int(1, 0);
        public static Vector2Int UnitY => new Vector2Int(0, 1);
        public bool IsEmpty() => _x == 0 && _y == 0;

        public Vector2Int(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public Vector2Int Rotate(float angleInRadians)
        {
            float cos = (float)Math.Cos(angleInRadians);
            float sin = (float)Math.Sin(angleInRadians);
            return new Vector2Int((int)(X * cos - Y * sin), (int)(X * sin + Y * cos));
        }

        /// <summary>
        /// Returns the angle in radians between two vectors.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float AngleBetween(Vector2Int a, Vector2Int b)
        {
            float dot = Dot(a, b);
            float det = a.X * b.Y - a.Y * b.X;
            return (float)Math.Atan2(det, dot);
        }

        private static float Dot(Vector2Int a, Vector2Int b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        public static bool operator ==(Vector2Int a, Vector2Int b)
        {
            return a.X == b.X && a.Y == b.Y;
        }
        public static bool operator !=(Vector2Int a, Vector2Int b)
        {
            return a.X != b.X || a.Y != b.Y;
        }

        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X + b.X, a.Y + b.Y);
        }
        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.X - b.X, a.Y - b.Y);
        }

        public static Vector2Int operator *(Vector2Int a, int scalar)
        {
            return new Vector2Int(a.X * scalar, a.Y * scalar);
        }

        public override int GetHashCode()
        {
            return X ^ (Y << 16);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static float Distance(Vector2Int position, Vector2Int targetPoint)
        {
            int dx = position.X - targetPoint.X;
            int dy = position.Y - targetPoint.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        public static float DistanceSquared(Vector2Int position, Vector2Int targetPoint)
        {
            int dx = position.X - targetPoint.X;
            int dy = position.Y - targetPoint.Y;
            return dx * dx + dy * dy;
        }

        public float Length()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }


        public Vector2Int Normalize(float length)
        {
            if (length == 0)
            {
                return Vector2Int.Zero;
            }
            return new Vector2Int((int)(X / length), (int)(Y / length));
        }

        public Vector2Int Normalize()
        {
            return Normalize(Length());
        }
    }
}
