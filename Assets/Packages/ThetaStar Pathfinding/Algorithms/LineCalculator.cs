using System;
using System.Collections.Generic;

namespace ThetaStar.Pathfinding.Algorithms {
    public struct GridPartition {
        public int X;
        public int Y;
        public float Percentage;
    }

    public class LineCalculator {
        private static List<GridPartition> _partitions = new List<GridPartition>(9);

        public static List<GridPartition> Calculate(int x1, int y1, int x2, int y2) {
            _partitions.Clear();

            float dx = Math.Abs(x2 - x1);
            float dy = Math.Abs(y2 - y1);
            float length = dx * dx + dy * dy;
            float invertLength = 1 / length;

            int stepX = x2 > x1 ? 1 : -1;
            int stepY = y2 > y1 ? 1 : -1;

            float tDeltaX = length / dx;
            float tDeltaY = length / dy;

            float tMaxX = (stepX > 0 ? 1 - (x1 % 1) : x1 % 1) * tDeltaX;
            float tMaxY = (stepY > 0 ? 1 - (y1 % 1) : y1 % 1) * tDeltaY;

            float totalLength = 0f;
            int minX = Math.Min(x1, x2), minY = Math.Min(y1, y2);
            while ((x1 != x2 || y1 != y2) &&
                   x1 >= minX && y1 >= minY) {
                bool xIsMin = tMaxX < tMaxY;
                float tMin = xIsMin ? tMaxX : tMaxY;

                if (tMin != totalLength) {
                    _partitions.Add(new GridPartition {
                        X = x1,
                        Y = y1,
                        Percentage = (tMin - totalLength) * invertLength
                    });
                }
                totalLength = tMin;

                if (xIsMin) {
                    x1 += stepX;
                    tMaxX += tDeltaX;
                } else {
                    y1 += stepY;
                    tMaxY += tDeltaY;
                }
            }

            if (length != totalLength) {
                _partitions.Add(new GridPartition {
                    X = x2,
                    Y = y2,
                    Percentage = (length - totalLength) * invertLength
                });
            }

            return _partitions;
        }
    }
}