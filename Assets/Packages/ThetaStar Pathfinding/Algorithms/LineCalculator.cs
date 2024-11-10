using System;
using System.Collections.Generic;

namespace ThetaStar.Pathfinding.Algorithms {
    public enum PartitionType {
        Diagonal,
        Vertical,
        Horizontal
    }

    public struct GridPartition {
        public int X;
        public int Y;
        public float Percentage;
    }

    public class LineCalculator {
        private static List<GridPartition> _partitions = new List<GridPartition>(9);

        public static (List<GridPartition>, PartitionType) Calculate(int x1, int y1, int x2, int y2) {
            _partitions.Clear();

            if (y1 == y2) {
                return (null, PartitionType.Vertical);
            }

            if (x1 == x2) {
                return (null, PartitionType.Horizontal);
            }

            float dx = x2 - x1;
            float dy = y2 - y1;
            float length = dx * dx + dy * dy;
            float invertLength = 1 / length;

            int stepX = dx > 0 ? 1 : -1;
            int stepY = dy > 0 ? 1 : -1;

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            float tDeltaX = length / dx;
            float tDeltaY = length / dy;

            float tMaxX = (stepX > 0 ? 1 - (x1 % 1) : x1 % 1) * tDeltaX;
            float tMaxY = (stepY > 0 ? 1 - (y1 % 1) : y1 % 1) * tDeltaY;

            int currentX = x1;
            int currentY = y1;
            float totalLength = 0f;

            int minX = Math.Min(x1, x2), minY = Math.Min(y1, y2);
            while ((currentX != x2 || currentY != y2) &&
                   currentX >= minX && currentY >= minY) {
                bool xIsMin = tMaxX < tMaxY;
                float tMin = xIsMin ? tMaxX : tMaxY;

                _partitions.Add(new GridPartition {
                    X = currentX,
                    Y = currentY,
                    Percentage = (tMin - totalLength) * invertLength
                });
                totalLength = tMin;

                if (xIsMin) {
                    currentX += stepX;
                    tMaxX += tDeltaX;
                } else {
                    currentY += stepY;
                    tMaxY += tDeltaY;
                }
            }

            _partitions.Add(new GridPartition {
                X = x2,
                Y = y2,
                Percentage = (length - totalLength) * invertLength
            });

            return (_partitions, PartitionType.Diagonal);
        }
    }
}