using System;
using Datatypes;

namespace Grid
{
    /*
     * Represents the Grid of blocked/unblocked tiles.
     */
    public class GridGraph
    {
        // Flattened 2D Array
        private bool[] tiles;

        public readonly int sizeX;
        public readonly int sizeY;
        public readonly int sizeXplusOne;

        private static readonly float SQRT_TWO = (float)Math.Sqrt(2);
        private static readonly double SQRT_TWO_DOUBLE = Math.Sqrt(2);
        private static readonly float SQRT_TWO_MINUS_ONE = (float)(Math.Sqrt(2) - 1);

        public GridGraph(int sizeX, int sizeY)
        {
            this.sizeX = sizeX;
            this.sizeY = sizeY;
            this.sizeXplusOne = sizeX + 1;

            tiles = new bool[sizeY * sizeX];
        }

        public void SetBlocked(int x, int y, bool value)
        {
            tiles[sizeX * y + x] = value;
        }

        public void TrySetBlocked(int x, int y, bool value)
        {
            if (IsValidBlock(x, y))
            {
                tiles[sizeX * y + x] = value;
            }
        }

        public bool IsBlocked(int x, int y)
        {
            if (x >= sizeX || y >= sizeY) return true;
            if (x < 0 || y < 0) return true;
            return tiles[sizeX * y + x];
        }

        public bool IsBlockedRaw(int x, int y)
        {
            return tiles[sizeX * y + x];
        }

        public bool IsValidCoordinate(int x, int y)
        {
            return (x <= sizeX && y <= sizeY && x >= 0 && y >= 0);
        }

        public bool IsValidBlock(int x, int y)
        {
            return (x < sizeX && y < sizeY && x >= 0 && y >= 0);
        }

        public int ToOneDimIndex(int x, int y)
        {
            return y * sizeXplusOne + x;
        }

        public int ToTwoDimX(int index)
        {
            return index % sizeXplusOne;
        }

        public int ToTwoDimY(int index)
        {
            return index / sizeXplusOne;
        }

        public bool IsUnblockedCoordinate(int x, int y)
        {
            return !TopRightOfBlockedTile(x, y) ||
                   !TopLeftOfBlockedTile(x, y) ||
                   !BottomRightOfBlockedTile(x, y) ||
                   !BottomLeftOfBlockedTile(x, y);
        }

        public bool TopRightOfBlockedTile(int x, int y)
        {
            return IsBlocked(x - 1, y - 1);
        }

        public bool TopLeftOfBlockedTile(int x, int y)
        {
            return IsBlocked(x, y - 1);
        }

        public bool BottomRightOfBlockedTile(int x, int y)
        {
            return IsBlocked(x - 1, y);
        }

        public bool BottomLeftOfBlockedTile(int x, int y)
        {
            return IsBlocked(x, y);
        }

        /*
         * x1,y1,x2,y2 refer to the top left corner of the tile.
         * @param x1 Condition: x1 between 0 and sizeX inclusive.
         * @param y1 Condition: y1 between 0 and sizeY inclusive.
         * @param x2 Condition: x2 between 0 and sizeX inclusive.
         * @param y2 Condition: y2 between 0 and sizeY inclusive.
         * @return distance.
         */
        public float Distance(int x1, int y1, int x2, int y2)
        {
            int xDiff = x2 - x1;
            int yDiff = y2 - y1;

            if (yDiff == 0)
            {
                return (float)Math.Abs(xDiff);
            }
            if (xDiff == 0)
            {
                return (float)Math.Abs(yDiff);
            }
            if (xDiff == yDiff || xDiff == -yDiff)
            {
                return SQRT_TWO * Math.Abs(xDiff);
            }

            int squareDistance = xDiff * xDiff + yDiff * yDiff;

            return (float)Math.Sqrt(squareDistance);
        }

        public double DistanceDouble(int x1, int y1, int x2, int y2)
        {
            int xDiff = x2 - x1;
            int yDiff = y2 - y1;

            if (xDiff == 0)
            {
                return Math.Abs(yDiff);
            }
            if (yDiff == 0)
            {
                return Math.Abs(xDiff);
            }
            if (xDiff == yDiff || xDiff == -yDiff)
            {
                return SQRT_TWO_DOUBLE * Math.Abs(xDiff);
            }

            int squareDistance = xDiff * xDiff + yDiff * yDiff;

            return Math.Sqrt(squareDistance);
        }

        /*
         * Octile distance:
         *   min(dx,dy)*sqrt(2) + (max(dx,dy)-min(dx,dy))
         * = min(dx,dy)*(sqrt(2)-1) + max(dx,dy)
         */
        public float OctileDistance(int x1, int y1, int x2, int y2)
        {
            int dx = x1 - x2;
            int dy = y1 - y2;
            if (dx < 0) dx = -dx;
            if (dy < 0) dy = -dy;

            int min = dx;
            int max = dy;
            if (dy < dx)
            {
                min = dy;
                max = dx;
            }

            return min * SQRT_TWO_MINUS_ONE + max;
        }

        /*
         * Same as lineOfSight, but only works with a vertex and its 8 immediate neighbours.
         * Also (x1,y1) != (x2,y2)
         */
        public bool NeighbourLineOfSight(int x1, int y1, int x2, int y2)
        {
            if (x1 == x2)
            {
                if (y1 > y2)
                {
                    return !IsBlocked(x1, y2) || !IsBlocked(x1 - 1, y2);
                }
                else
                { 
                    // y1 < y2
                    return !IsBlocked(x1, y1) || !IsBlocked(x1 - 1, y1);
                }
            }
            else if (x1 < x2)
            {
                if (y1 == y2)
                {
                    return !IsBlocked(x1, y1) || !IsBlocked(x1, y1 - 1);
                }
                else if (y1 < y2)
                {
                    return !IsBlocked(x1, y1);
                }
                else
                { 
                    // y2 < y1
                    return !IsBlocked(x1, y2);
                }
            }
            else
            { 
                // x2 < x1
                if (y1 == y2)
                {
                    return !IsBlocked(x2, y1) || !IsBlocked(x2, y1 - 1);
                }
                else if (y1 < y2)
                {
                    return !IsBlocked(x2, y1);
                }
                else
                { 
                    // y2 < y1
                    return !IsBlocked(x2, y2);
                }
            }
        }


        /*
         * @return true iff there is line-of-sight from (x1,y1) to (x2,y2).
         */
        public bool LineOfSight(int x1, int y1, int x2, int y2)
        {
            int dy = y2 - y1;
            int dx = x2 - x1;

            int f = 0;

            int signY = 1;
            int signX = 1;
            int offsetX = 0;
            int offsetY = 0;

            if (dy < 0)
            {
                dy *= -1;
                signY = -1;
                offsetY = -1;
            }
            if (dx < 0)
            {
                dx *= -1;
                signX = -1;
                offsetX = -1;
            }

            if (dx >= dy)
            {
                while (x1 != x2)
                {
                    f += dy;
                    if (f >= dx)
                    {
                        if (IsBlocked(x1 + offsetX, y1 + offsetY))
                            return false;
                        y1 += signY;
                        f -= dx;
                    }
                    if (f != 0 && IsBlocked(x1 + offsetX, y1 + offsetY))
                        return false;
                    if (dy == 0 && IsBlocked(x1 + offsetX, y1) && IsBlocked(x1 + offsetX, y1 - 1))
                        return false;

                    x1 += signX;
                }
            }
            else
            {
                while (y1 != y2)
                {
                    f += dx;
                    if (f >= dy)
                    {
                        if (IsBlocked(x1 + offsetX, y1 + offsetY))
                            return false;
                        x1 += signX;
                        f -= dy;
                    }
                    if (f != 0 && IsBlocked(x1 + offsetX, y1 + offsetY))
                        return false;
                    if (dx == 0 && IsBlocked(x1, y1 + offsetY) && IsBlocked(x1 - 1, y1 + offsetY))
                        return false;

                    y1 += signY;
                }
            }
            return true;
        }

        public Point FindFirstBlockedTile(int x1, int y1, int dx, int dy)
        {

            int f = 0;

            int signY = 1;
            int signX = 1;
            int offsetX = 0;
            int offsetY = 0;

            if (dy < 0)
            {
                dy *= -1;
                signY = -1;
                offsetY = -1;
            }
            if (dx < 0)
            {
                dx *= -1;
                signX = -1;
                offsetX = -1;
            }

            if (dx >= dy)
            {
                while (true)
                {
                    f += dy;
                    if (f >= dx)
                    {
                        if (IsBlocked(x1 + offsetX, y1 + offsetY))
                            return new Point(x1 + offsetX, y1 + offsetY);
                        y1 += signY;
                        f -= dx;
                    }
                    if (f != 0 && IsBlocked(x1 + offsetX, y1 + offsetY))
                        return new Point(x1 + offsetX, y1 + offsetY);
                    if (dy == 0 && IsBlocked(x1 + offsetX, y1) && IsBlocked(x1 + offsetX, y1 - 1))
                        return new Point(x1 + offsetX, -1);

                    x1 += signX;
                }
            }
            else
            {
                while (true)
                {
                    f += dx;
                    if (f >= dy)
                    {
                        if (IsBlocked(x1 + offsetX, y1 + offsetY))
                            return new Point(x1 + offsetX, y1 + offsetY);
                        x1 += signX;
                        f -= dy;
                    }
                    if (f != 0 && IsBlocked(x1 + offsetX, y1 + offsetY))
                        return new Point(x1 + offsetX, y1 + offsetY);
                    if (dx == 0 && IsBlocked(x1, y1 + offsetY) && IsBlocked(x1 - 1, y1 + offsetY))
                        return new Point(-1, y1 + offsetY);

                    y1 += signY;
                }
            }
            //return null;
        }


        /*
         * Used by Accelerated A* and MazeAnalysis.
         * leftRange is the number of blocks you can move left before hitting a blocked tile.
         * downRange is the number of blocks you can move down before hitting a blocked tile.
         * For blocked tiles, leftRange, downRange are both -1.
         * 
         * How to use the maxRange property:
         * 
         *  x,y is the starting point.
         *  k is the number of tiles diagonally up-right of the starting point.
         *  int i = x-y+sizeY;
         *  int j = Math.min(x, y);
         *  return maxRange[i][j + k] - k;
         */
        public int[][] ComputeMaxDownLeftRanges()
        {
            int[][] downRange = new int[sizeY + 1][];
            int[][] leftRange = new int[sizeY + 1][];

            for (int y = 0; y < sizeY; ++y)
            {
                if (IsBlocked(0, y))
                    leftRange[y][0] = -1;
                else
                    leftRange[y][0] = 0;

                for (int x = 1; x < sizeX; ++x)
                {
                    if (IsBlocked(x, y))
                    {
                        leftRange[y][x] = -1;
                    }
                    else
                    {
                        leftRange[y][x] = leftRange[y][x - 1] + 1;
                    }
                }
            }

            for (int x = 0; x < sizeX; ++x)
            {
                if (IsBlocked(x, 0))
                    downRange[0][x] = -1;
                else
                    downRange[0][x] = 0;

                for (int y = 1; y < sizeY; ++y)
                {
                    if (IsBlocked(x, y))
                    {
                        downRange[y][x] = -1;
                    }
                    else
                    {
                        downRange[y][x] = downRange[y - 1][x] + 1;
                    }
                }
            }

            for (int x = 0; x < sizeX + 1; ++x)
            {
                downRange[sizeY][x] = -1;
                leftRange[sizeY][x] = -1;
            }

            for (int y = 0; y < sizeY; ++y)
            {
                downRange[y][sizeX] = -1;
                leftRange[y][sizeX] = -1;
            }

            int[][] maxRanges = new int[sizeX + sizeY + 1][];
            int maxSize = Math.Min(sizeX, sizeY) + 1;
            for (int i = 0; i < maxRanges.Length; ++i)
            {
                int currSize = Math.Min(i + 1, maxRanges.Length - i);
                if (maxSize < currSize)
                    currSize = maxSize;
                maxRanges[i] = new int[currSize];

                int currX = i - sizeY;
                if (currX < 0) currX = 0;
                int currY = currX - i + sizeY;
                for (int j = 0; j < maxRanges[i].Length; ++j)
                {
                    maxRanges[i][j] = Math.Min(downRange[currY][currX], leftRange[currY][currX]);
                    currY++;
                    currX++;
                }

            }
            return maxRanges;
        }

        /*
         * @return the percentage of blocked tiles as compared to the total grid size.
         */
        public float GetPercentageBlocked()
        {
            return (float)GetNumBlocked() / (sizeX * sizeY);
        }

        /*
         * @return the number of blocked tiles in the grid.
         */
        public int GetNumBlocked()
        {
            int nBlocked = 0;
            for (int y = 0; y < sizeY; y++)
            {
                for (int x = 0; x < sizeX; x++)
                {
                    if (IsBlocked(x, y))
                    {
                        nBlocked++;
                    }
                }
            }
            return nBlocked;
        }

        public bool IsOuterCorner(int x, int y)
        {
            bool a = IsBlocked(x - 1, y - 1);
            bool b = IsBlocked(x, y - 1);
            bool c = IsBlocked(x, y);
            bool d = IsBlocked(x - 1, y);

            return ((!a && !c) || (!d && !b)) && (a || b || c || d);

            /* NOTE
             *   ___ ___
             *  |   |||||
             *  |...X'''| <-- this is considered a corner in the above definition
             *  |||||___|
             *  
             *  The definition below excludes the above case.
             */

            /*int results = 0;
            if(a)results++;
            if(b)results++;
            if(c)results++;
            if(d)results++;
            return (results == 1);*/
        }

        /**
         * Checks whether the path (x1,y1),(x2,y2),(x3,y3) is taut.
         */
        public bool IsTaut(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (x1 < x2)
            {
                if (y1 < y2)
                {
                    return IsTautFromBottomLeft(x1, y1, x2, y2, x3, y3);
                }
                else if (y2 < y1)
                {
                    return IsTautFromTopLeft(x1, y1, x2, y2, x3, y3);
                }
                else
                { 
                    // y1 == y2
                    return IsTautFromLeft(x1, y1, x2, y2, x3, y3);
                }
            }
            else if (x2 < x1)
            {
                if (y1 < y2)
                {
                    return IsTautFromBottomRight(x1, y1, x2, y2, x3, y3);
                }
                else if (y2 < y1)
                {
                    return IsTautFromTopRight(x1, y1, x2, y2, x3, y3);
                }
                else
                { 
                    // y1 == y2
                    return IsTautFromRight(x1, y1, x2, y2, x3, y3);
                }
            }
            else
            { 
                // x2 == x1
                if (y1 < y2)
                {
                    return IsTautFromBottom(x1, y1, x2, y2, x3, y3);
                }
                else if (y2 < y1)
                {
                    return IsTautFromTop(x1, y1, x2, y2, x3, y3);
                }
                else
                { 
                    // y1 == y2
                    throw new NotSupportedException("v == u?");
                }
            }
        }


        private bool IsTautFromBottomLeft(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (x3 < x2 || y3 < y2) return false;

            int compareGradients = (y2 - y1) * (x3 - x2) - (y3 - y2) * (x2 - x1); // m1 - m2
            if (compareGradients < 0)
            { 
                // m1 < m2
                return BottomRightOfBlockedTile(x2, y2);
            }
            else if (compareGradients > 0)
            { 
                // m1 > m2
                return TopLeftOfBlockedTile(x2, y2);
            }
            else
            { 
                // m1 == m2
                return true;
            }
        }


        private bool IsTautFromTopLeft(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (x3 < x2 || y3 > y2) return false;

            // m1 - m2
            int compareGradients = (y2 - y1) * (x3 - x2) - (y3 - y2) * (x2 - x1);
            if (compareGradients < 0)
            { 
                // m1 < m2
                return BottomLeftOfBlockedTile(x2, y2);
            }
            else if (compareGradients > 0)
            { 
                // m1 > m2
                return TopRightOfBlockedTile(x2, y2);
            }
            else
            { 
                // m1 == m2
                return true;
            }
        }

        private bool IsTautFromBottomRight(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (x3 > x2 || y3 < y2) return false;

            // m1 - m2
            int compareGradients = (y2 - y1) * (x3 - x2) - (y3 - y2) * (x2 - x1); 
            if (compareGradients < 0)
            { 
                // m1 < m2
                return TopRightOfBlockedTile(x2, y2);
            }
            else if (compareGradients > 0)
            { 
                // m1 > m2
                return BottomLeftOfBlockedTile(x2, y2);
            }
            else
            { 
                // m1 == m2
                return true;
            }
        }


        private bool IsTautFromTopRight(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (x3 > x2 || y3 > y2) return false;

            // m1 - m2
            int compareGradients = (y2 - y1) * (x3 - x2) - (y3 - y2) * (x2 - x1); 
            if (compareGradients < 0)
            { 
                // m1 < m2
                return TopLeftOfBlockedTile(x2, y2);
            }
            else if (compareGradients > 0)
            { 
                // m1 > m2
                return BottomRightOfBlockedTile(x2, y2);
            }
            else
            { 
                // m1 == m2
                return true;
            }
        }


        private bool IsTautFromLeft(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (x3 < x2) return false;

            int dy = y3 - y2;
            if (dy < 0)
            { 
                // y3 < y2
                return TopRightOfBlockedTile(x2, y2);
            }
            else if (dy > 0)
            { 
                // y3 > y2
                return BottomRightOfBlockedTile(x2, y2);
            }
            else
            { 
                // y3 == y2
                return true;
            }
        }

        private bool IsTautFromRight(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (x3 > x2) return false;

            int dy = y3 - y2;
            if (dy < 0)
            { // y3 < y2
                return TopLeftOfBlockedTile(x2, y2);
            }
            else if (dy > 0)
            { // y3 > y2
                return BottomLeftOfBlockedTile(x2, y2);
            }
            else
            { // y3 == y2
                return true;
            }
        }

        private bool IsTautFromBottom(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (y3 < y2) return false;

            int dx = x3 - x2;
            if (dx < 0)
            { 
                // x3 < x2
                return TopRightOfBlockedTile(x2, y2);
            }
            else if (dx > 0)
            { 
                // x3 > x2
                return TopLeftOfBlockedTile(x2, y2);
            }
            else
            { 
                // x3 == x2
                return true;
            }
        }

        private bool IsTautFromTop(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            if (y3 > y2) return false;

            int dx = x3 - x2;
            if (dx < 0)
            { 
                // x3 < x2
                return BottomRightOfBlockedTile(x2, y2);
            }
            else if (dx > 0)
            { 
                // x3 > x2
                return BottomLeftOfBlockedTile(x2, y2);
            }
            else
            { 
                // x3 == x2
                return true;
            }
        }
    }
}
