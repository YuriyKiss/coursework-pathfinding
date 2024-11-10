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

        public static (List<GridPartition>, bool) Calculate(int x1, int y1, int x2, int y2) {
            _partitions.Clear();

            if (y1 == y2) {
                return (VerticalPartitions(y1, x1, x2), true);
            }

            if (x1 == x2) {
                return (HorizontalPartitions(y1, y2, x1), true);
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

            return (_partitions, false);
        }

        private static List<GridPartition> VerticalPartitions(int x, int y1, int y2) {
            float percentage = 1f / Math.Abs(y1 - y2);

            int startY = Math.Min(y1, y2);
            int endY = Math.Max(y1, y2);

            for (int i = startY; i <= endY; ++i) {
                _partitions.Add(new GridPartition() {
                    X = i,
                    Y = x,
                    Percentage = percentage
                });
            }

            return _partitions;
        }

        private static List<GridPartition> HorizontalPartitions(int x1, int x2, int y) {
            float percentage = 1f / Math.Abs(x1 - x2);

            int startX = Math.Min(x1, x2);
            int endX = Math.Max(x1, x2);

            for (int i = startX; i <= endX; ++i) {
                _partitions.Add(new GridPartition() {
                    X = y,
                    Y = i,
                    Percentage = percentage
                });
            }

            return _partitions;
        }
    }
}