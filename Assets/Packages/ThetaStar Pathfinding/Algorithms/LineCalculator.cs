using System;
using System.Collections.Generic;

namespace ThetaStar.Pathfinding.Algorithms {
    public class GridPartitions {
        public int X;
        public int Y;
        public double Length;
        public double Percentage;
    }

    public class LineCalculator {
        // Calculates intersection lengths of a grid
        public static (List<GridPartitions>, bool) Calculate(int x1, int y1, int x2, int y2) {
            if (y1 == y2) {
                //UnityEngine.Debug.Log("Vertical");
                return (VerticalPartitions(y1, x1, x2), true);
            }
            
            if (x1 == x2) {
                //UnityEngine.Debug.Log("Horizontal");
                return (HorizontalPartitions(y1, y2, x1), true);
            }

            var partitions = new List<GridPartitions>();
            var (slope, intercept) = CalculateLineEquation(x1, y1, x2, y2);

            int startX = Math.Min(x1, x2);
            int endX = Math.Max(x1, x2);
            int startY = Math.Min(y1, y2);
            int endY = Math.Max(y1, y2);

            double totalLength = 0.0;
            for (int i = startX; i <= endX - 1; ++i) {
                for (int j = startY; j <= endY - 1; ++j) {
                    double length = CalculateIntersectionLength(slope, intercept, i, i + 1, j + 1, j);
                    if (length > 0) {
                        partitions.Add(new GridPartitions {
                            X = i,
                            Y = j,
                            Length = length
                        });
                        totalLength += length;
                    }
                }
            }

            foreach (var partition in partitions) {
                partition.Percentage = partition.Length / totalLength;
            }

            return (partitions, false);
        }

        private static List<GridPartitions> VerticalPartitions(int x, int y1, int y2) {
            var partitions = new List<GridPartitions>();
            float percentage = 1f / MathF.Abs(y1 - y2);

            int startY = Math.Min(y1, y2);
            int endY = Math.Max(y1, y2);

            for (int i = startY; i <= endY; ++i) {
                partitions.Add(new GridPartitions() {
                    X = i,
                    Y = x,
                    Percentage = percentage
                });
            }

            return partitions;
        }

        private static List<GridPartitions> HorizontalPartitions(int x1, int x2, int y) {
            var partitions = new List<GridPartitions>();
            float percentage = 1f / MathF.Abs(x1 - x2);

            int startX = Math.Min(x1, x2);
            int endX = Math.Max(x1, x2);

            for (int i = startX; i <= endX; ++i) {
                partitions.Add(new GridPartitions() {
                    X = y,
                    Y = i,
                    Percentage = percentage
                });
            }

            return partitions;
        }

        static (double slope, double intercept) CalculateLineEquation(double x1, double y1, double x2, double y2) {
            // Calculate the slope
            double slope = (y2 - y1) / (x2 - x1);

            // Calculate the y-intercept
            double intercept = y1 - slope * x1;

            return (slope, intercept);
        }

        static double CalculateIntersectionLength(double slope, double intercept, double left, double right, double top, double bottom) {
            // Array to hold up to two valid intersections
            (double x, double y)?[] intersections = new (double, double)?[2];
            int count = 0;

            // Check intersection with left edge
            var leftIntersection = IntersectWithVerticalEdge(left, slope, intercept);
            if (leftIntersection.y >= bottom && leftIntersection.y <= top) {
                intersections[count++] = leftIntersection;
                if (count == 2) return Distance(intersections[0].Value, intersections[1].Value); // Early exit
            }

            // Check intersection with right edge
            var rightIntersection = IntersectWithVerticalEdge(right, slope, intercept);
            if (rightIntersection.y >= bottom && rightIntersection.y <= top) {
                intersections[count++] = rightIntersection;
                if (count == 2) return Distance(intersections[0].Value, intersections[1].Value);
            }

            // Check intersection with bottom edge
            var bottomIntersection = IntersectWithHorizontalEdge(bottom, slope, intercept);
            if (bottomIntersection.x >= left && bottomIntersection.x <= right) {
                intersections[count++] = bottomIntersection;
                if (count == 2) return Distance(intersections[0].Value, intersections[1].Value);
            }

            // Check intersection with top edge
            var topIntersection = IntersectWithHorizontalEdge(top, slope, intercept);
            if (topIntersection.x >= left && topIntersection.x <= right) {
                intersections[count++] = topIntersection;
                if (count == 2) return Distance(intersections[0].Value, intersections[1].Value);
            }

            return 0; // No valid intersection or fewer than two valid points
        }

        static (double x, double y) IntersectWithVerticalEdge(double xEdge, double slope, double intercept) {
            double y = slope * xEdge + intercept;
            return (xEdge, y);
        }

        static (double x, double y) IntersectWithHorizontalEdge(double yEdge, double slope, double intercept) {
            double x = (yEdge - intercept) / slope;
            return (x, yEdge);
        }

        static double Distance((double x, double y) p1, (double x, double y) p2) {
            double dx = p2.x - p1.x;
            double dy = p2.y - p1.y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

    }
}