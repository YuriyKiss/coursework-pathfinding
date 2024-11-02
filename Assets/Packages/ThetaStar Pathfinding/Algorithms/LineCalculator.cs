using System;
using System.Collections.Generic;
using System.Linq;

namespace ThetaStar.Pathfinding.Algorithms {
    public struct GridPartitions {
        public int X;
        public int Y;
        public double Length;
        public double Percentage;
    }

    public class LineCalculator {

        public static List<GridPartitions> Calculate(int x1, int y1, int x2, int y2) {
            if (x1 == x2) {
                return VerticalPartitions(x1, y1, y2);
            }
            
            var partitions = new List<GridPartitions>();
            var (slope, intercept) = CalculateLineEquation(x1, y1, x2, y2);

            int startX = x1 > x2 ? x2 : x1;
            int endX = x1 > x2 ? x1 - 1 : x2 - 1;
            int startY = y1 > y2 ? y2 : y1;
            int endY = y1 > y2 ? y1 : y2;
            for (int i = startX; i <= endX; ++i) { 
                for (int j = startY; j <= endY; ++j) {
                    partitions.Add(new GridPartitions()  {
                        X = i,
                        Y = j,
                        Length = CalculateIntersectionLength(slope, intercept, (i, j)),
                        Percentage = -1});
                }
            }

            double sum = partitions.Sum(x => x.Length);
            for (int i = 0; i < partitions.Count; ++i) {
                var partition = partitions[i];
                partition.Percentage = partition.Length / sum;
                partitions[i] = partition;
            }

            return partitions;
        }

        private static List<GridPartitions> VerticalPartitions(int x, int y1, int y2) {
            var partitions = new List<GridPartitions>();
            
            if (y1 > y2) {
                float percentage = 1f / (y1 - y2);

                for (int i = y1; i > y2; i--) {
                    partitions.Add(new GridPartitions() {
                        X = x, Y = i, Length = 1, Percentage = percentage
                    });
                }
            } else {
                float percentage = 1f / (y2 - y1);

                for (int i = y1 + 1; i <= y2; i++) {
                    partitions.Add(new GridPartitions() {
                        X = x, Y = i, Length = 1, Percentage = percentage
                    });
                }
            }

            return partitions;
        }

        static (double slope, double intercept) CalculateLineEquation(double x1, double y1, double x2, double y2) {
            if (x1 == x2) {
                return (0, 0);
            }

            // Calculate the slope
            double slope = (y2 - y1) / (x2 - x1);

            // Calculate the y-intercept
            double intercept = y1 - slope * x1;

            return (slope, intercept);
        }

        static double CalculateIntersectionLength(double slope, double intercept, (int cellX, int cellY) cell) {
            // Cell boundaries
            double left = cell.cellX;
            double right = cell.cellX + 1;
            double top = cell.cellY;
            double bottom = cell.cellY - 1;

            // List to store intersection points
            List<(double x, double y)> intersections = new List<(double x, double y)> {
                // Check intersection with left edge
                IntersectWithVerticalEdge(left, slope, intercept),
                // Check intersection with right edge
                IntersectWithVerticalEdge(right, slope, intercept),
                // Check intersection with bottom edge
                IntersectWithHorizontalEdge(bottom, slope, intercept),
                // Check intersection with top edge
                IntersectWithHorizontalEdge(top, slope, intercept)
            };

            // Filter valid intersections
            intersections = intersections.Where(p => p.x >= left && p.x <= right && p.y >= bottom && p.y <= top).ToList();

            // Calculate the length of the segment within the cell
            if (intersections.Count < 2) return 0; // No valid intersections
            return Distance(intersections[0], intersections[1]);
        }

        static (double x, double y) IntersectWithVerticalEdge(double xEdge, double slope, double intercept) {
            double y = slope * xEdge + intercept;
            return (xEdge, y);
        }

        static (double x, double y) IntersectWithHorizontalEdge(double yEdge, double slope, double intercept) {
            if (slope == 0) return (0, 0); // No intersection with horizontal edge if slope is 0
            double x = (yEdge - intercept) / slope;
            return (x, yEdge);
        }

        static double Distance((double x, double y) p1, (double x, double y) p2) {
            return Math.Sqrt(Math.Pow(p2.x - p1.x, 2) + Math.Pow(p2.y - p1.y, 2));
        }

    }
}