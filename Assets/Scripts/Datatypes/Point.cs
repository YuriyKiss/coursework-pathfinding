namespace algorithms.datatypes
{
    public sealed class Point
    {
        public readonly int x;
        public readonly int y;

        public Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime * result + x;
            result = prime * result + y;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (obj is Point other)
            {
                return x == other.x && y == other.y;
            }
            return false;
        }

        public override string ToString()
        {
            return $"({x},{y})";
        }
    }
}
