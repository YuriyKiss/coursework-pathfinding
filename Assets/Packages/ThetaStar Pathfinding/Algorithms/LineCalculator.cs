using System;
using System.Collections.Generic;

namespace ThetaStar.Pathfinding.Algorithms {
    public class GridPartition {
        public int X;
        public int Y;
        public double Length;
        public double Percentage;
    }

    public class LineCalculator {
        public static (List<GridPartition>, bool) Calculate(int x1, int y1, int x2, int y2) {
            if (y1 == y2) {
                return (VerticalPartitions(y1, x1, x2), true);
            }

            if (x1 == x2) {
                return (HorizontalPartitions(y1, y2, x1), true);
            }

            var partitions = new List<GridPartition>();
            double dx = x2 - x1;
            double dy = y2 - y1;
            double length = Math.Sqrt(dx * dx + dy * dy);

            int stepX = dx > 0 ? 1 : -1;
            int stepY = dy > 0 ? 1 : -1;

            dx = Math.Abs(dx);
            dy = Math.Abs(dy);

            // Adjust tMaxX and tMaxY based on top-left corner positioning
            double tMaxX = (stepX > 0 ? 1 - (x1 % 1) : x1 % 1) * length / dx;
            double tMaxY = (stepY > 0 ? 1 - (y1 % 1) : y1 % 1) * length / dy;

            double tDeltaX = length / dx;
            double tDeltaY = length / dy;

            int currentX = x1;
            int currentY = y1;
            double totalLength = 0.0;

            int minX = Math.Min(x1, x2), minY = Math.Min(y1, y2);
            // Loop with bounds and maxSteps check
            while ((currentX != x2 || currentY != y2) &&
                   currentX >= minX && currentY >= minY) {

                double tMin = Math.Min(tMaxX, tMaxY);
                double segmentLength = tMin - totalLength;

                partitions.Add(new GridPartition {
                    X = currentX,
                    Y = currentY,
                    Length = segmentLength
                });
                totalLength = tMin;

                if (tMaxX < tMaxY) {
                    currentX += stepX;
                    tMaxX += tDeltaX;
                } else {
                    currentY += stepY;
                    tMaxY += tDeltaY;
                }
            }

            // Ensure the final cell is added if necessary
            if (currentX == x2 && currentY == y2) {
                partitions.Add(new GridPartition {
                    X = x2,
                    Y = y2,
                    Length = length - totalLength
                });
            } else {
                // Add the final cell to avoid missing the endpoint
                partitions.Add(new GridPartition {
                    X = x2,
                    Y = y2,
                    Length = Math.Max(0, length - totalLength)
                });
            }

            foreach (var partition in partitions) {
                partition.Percentage = partition.Length / length;
                //UnityEngine.Debug.Log($"[{partition.X}, {partition.Y}] with (len/perc) {partition.Length}/{partition.Percentage}");
            }

            return (partitions, false);
        }

        private static List<GridPartition> VerticalPartitions(int x, int y1, int y2) {
            var partitions = new List<GridPartition>();
            float percentage = 1f / MathF.Abs(y1 - y2);

            int startY = Math.Min(y1, y2);
            int endY = Math.Max(y1, y2);

            for (int i = startY; i <= endY; ++i) {
                partitions.Add(new GridPartition() {
                    X = i,
                    Y = x,
                    Percentage = percentage
                });
            }

            return partitions;
        }

        private static List<GridPartition> HorizontalPartitions(int x1, int x2, int y) {
            var partitions = new List<GridPartition>();
            float percentage = 1f / MathF.Abs(x1 - x2);

            int startX = Math.Min(x1, x2);
            int endX = Math.Max(x1, x2);

            for (int i = startX; i <= endX; ++i) {
                partitions.Add(new GridPartition() {
                    X = y,
                    Y = i,
                    Percentage = percentage
                });
            }

            return partitions;
        }
    }
}