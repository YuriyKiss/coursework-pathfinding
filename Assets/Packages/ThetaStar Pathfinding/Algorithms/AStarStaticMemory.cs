using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ThetaStar.Pathfinding.Datatypes;
using ThetaStar.Pathfinding.Grid;
using ThetaStar.Pathfinding.PriorityQueue;

namespace ThetaStar.Pathfinding.Algorithms
{
    public class AStarStaticMemory : PathFindingAlgorithm
    {
        protected bool postSmoothingOn = false;
        protected bool repeatedPostSmooth = false;
        protected float heuristicWeight = 1f;

        protected ReusableIndirectHeap pq;

        protected int finish;

        public AStarStaticMemory(GridGraph graph, int sx, int sy, int ex, int ey) : base(graph, graph.sizeX, graph.sizeY, sx, sy, ex, ey)
        {
        }

        public static AStarStaticMemory PostSmooth(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            AStarStaticMemory aStar = new AStarStaticMemory(graph, sx, sy, ex, ey);
            aStar.postSmoothingOn = true;
            aStar.repeatedPostSmooth = false;
            return aStar;
        }

        public static AStarStaticMemory RepeatedPostSmooth(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            AStarStaticMemory aStar = new AStarStaticMemory(graph, sx, sy, ex, ey);
            aStar.postSmoothingOn = true;
            aStar.repeatedPostSmooth = true;
            return aStar;
        }

        public static AStarStaticMemory Dijkstra(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            AStarStaticMemory aStar = new AStarStaticMemory(graph, sx, sy, ex, ey);
            aStar.heuristicWeight = 0;
            return aStar;
        }

        public override void ComputePath()
        {
            int totalSize = (graph.sizeX + 1) * (graph.sizeY + 1);

            int start = ToOneDimIndex(sx, sy);
            finish = ToOneDimIndex(ex, ey);

            pq = new ReusableIndirectHeap(totalSize);
            InitialiseMemory(totalSize, float.PositiveInfinity, -1, false);

            Initialise(start);

            // float lastDist = -1;
            while (!pq.IsEmpty())
            {
                // float dist = pq.GetMinValue();

                int current = pq.PopMinIndex();

                //if (Math.abs(dist - lastDist) > 0.01f) { maybeSaveSearchSnapshot(); lastDist = dist;}
                MaybeSaveSearchSnapshot();

                if (current == finish || Distance(current) == float.PositiveInfinity)
                {
                    MaybeSaveSearchSnapshot();
                    break;
                }
                SetVisited(current, true);

                int x = ToTwoDimX(current);
                int y = ToTwoDimY(current);

                TryRelaxNeighbour(current, x, y, x - 1, y - 1);
                TryRelaxNeighbour(current, x, y, x, y - 1);
                TryRelaxNeighbour(current, x, y, x + 1, y - 1);
                TryRelaxNeighbour(current, x, y, x - 1, y);
                TryRelaxNeighbour(current, x, y, x + 1, y);
                TryRelaxNeighbour(current, x, y, x - 1, y + 1);
                TryRelaxNeighbour(current, x, y, x, y + 1);
                TryRelaxNeighbour(current, x, y, x + 1, y + 1);

                //maybeSaveSearchSnapshot();
            }

            MaybePostSmooth();
        }

        protected virtual void TryRelaxNeighbour(int current, int currentX, int currentY, int x, int y)
        {
            if (!graph.IsValidCoordinate(x, y))
                return;

            int destination = ToOneDimIndex(x, y);
            if (Visited(destination))
                return;
            if (!graph.NeighbourLineOfSight(currentX, currentY, x, y))
                return;

            float weight = Weight(currentX, currentY, x, y);
            if (Relax(current, destination, weight))
            {
                // If relaxation is done.
                pq.DecreaseKey(destination, Distance(destination) + weight);
            }
        }

        protected virtual float Heuristic(int x, int y)
        {
            return heuristicWeight * graph.Distance(x, y, ex, ey);
        }

        protected float Weight(int x1, int y1, int x2, int y2) {
            float weight = -1f, secondWeight = -1f;
            bool isCorner = false;

            if (x1 == x2) {
                if (y1 > y2) {
                    weight = graph.GetWeight(x2, y2);
                    secondWeight = graph.GetWeight(x2 - 1, y2);
                    isCorner = true;
                } else {
                    weight = graph.GetWeight(x1, y1);
                    secondWeight = graph.GetWeight(x1 - 1, y1);
                    isCorner = true;
                }
            } else if (y1 == y2) {
                if (x1 > x2) {
                    weight = graph.GetWeight(x2, y2);
                    secondWeight = graph.GetWeight(x2, y2 - 1);
                    isCorner = true;
                } else {
                    weight = graph.GetWeight(x1, y1);
                    secondWeight = graph.GetWeight(x1, y1 - 1);
                    isCorner = true;
                }
            } else {
                weight = graph.GetWeight(Math.Min(x1, x2), Math.Min(y1, y2));
            }

            if (isCorner) {
                if (weight == -1) weight = secondWeight;
                else if (secondWeight != -1) weight = (weight + secondWeight) / 2;
            }

            return graph.Distance(x1, y1, x2, y2) * weight;
        }

        protected virtual bool Relax(int u, int v, float weightUV)
        {
            // return true if relaxation is done.

            float newWeight = Distance(u) + weightUV;
            if (newWeight < Distance(v))
            {
                SetDistance(v, newWeight);
                SetParent(v, u);
                //maybeSaveSearchSnapshot();
                return true;
            }
            return false;
        }

        protected void Initialise(int s)
        {
            pq.DecreaseKey(s, 0f);
            Memory.SetDistance(s, 0f);
        }

        private int PathLength()
        {
            int length = 0;
            int current = finish;
            while (current != -1)
            {
                current = Parent(current);
                length++;
            }
            return length;
        }

        protected bool LineOfSight(int node1, int node2)
        {
            int x1 = ToTwoDimX(node1);
            int y1 = ToTwoDimY(node1);
            int x2 = ToTwoDimX(node2);
            int y2 = ToTwoDimY(node2);
            return graph.LineOfSight(x1, y1, x2, y2);
        }

        protected float PhysicalDistance(int node1, int node2)
        {
            int x1 = ToTwoDimX(node1);
            int y1 = ToTwoDimY(node1);
            int x2 = ToTwoDimX(node2);
            int y2 = ToTwoDimY(node2);

            return ApplyPhysicalWeight(x1, y1, x2, y2);
        }

        private float ApplyPhysicalWeight(int x1, int y1, int x2, int y2) {
            float pathLength = 0;

            (var partitions, var partitionType) = LineCalculator.Calculate(y1, x1, y2, x2);
            if (partitionType == PartitionType.Vertical) {
                int startY = Math.Min(y1, y2);
                int endY = Math.Max(y1, y2);

                for (int i = startY; i <= endY - 1; ++i) {
                    pathLength += Weight(x1, i, x2, i + 1);
                }
            } else if (partitionType == PartitionType.Horizontal) {
                int startX = Math.Min(x1, x2);
                int endX = Math.Max(x1, x2);

                for (int i = startX; i <= endX - 1; ++i) {
                    pathLength += Weight(i, y1, i + 1, y2);
                }
            } else {
                float distance = graph.Distance(x1, y1, x2, y2);

                for (int i = 0; i < partitions.Count; ++i) {
                    pathLength += distance * partitions[i].Percentage * graph.GetWeight(partitions[i].Y, partitions[i].X);
                }
            }

            return pathLength;
        }

        protected float PhysicalDistance(int x1, int y1, int node2)
        {
            int x2 = ToTwoDimX(node2);
            int y2 = ToTwoDimY(node2);
            return graph.Distance(x1, y1, x2, y2);
        }

        protected void MaybePostSmooth()
        {
            if (postSmoothingOn)
            {
                if (repeatedPostSmooth)
                {
                    while (PostSmooth()) ;
                }
                else
                {
                    // Optimal post-smoothing passes
                    for (int i = 0; i < 50; ++i) {
                        PostSmooth();
                    }
                }
            }
        }

        private bool PostSmooth()
        {
            bool didSomething = false;

            int current = finish;
            while (current != -1)
            {
                int next = Parent(current);
                float distanceJagged = PhysicalDistance(current, next);
                if (next != -1 && LineOfSight(current, next))
                { 
                    int prev = next;
                    next = Parent(next);
                    while (next != -1)
                    {
                        distanceJagged += PhysicalDistance(prev, next);
                        bool isShorter = distanceJagged <= PhysicalDistance(next, current);
                        if (LineOfSight(current, next) && isShorter)
                        {
                            SetParent(current, next);
                            prev = next;
                            next = Parent(next);
                            didSomething = true;
                            MaybeSaveSearchSnapshot();
                        }
                        else
                        {
                            next = -1;
                        }
                    }
                }

                current = Parent(current);
            }

            return didSomething;
        }

        public override int[][] GetPath()
        {
            int length = PathLength();
            int[][] path = new int[length][];
            int current = finish;

            int index = length - 1;
            while (current != -1)
            {
                int x = ToTwoDimX(current);
                int y = ToTwoDimY(current);

                path[index] = new int[2];
                path[index][0] = x;
                path[index][1] = y;

                index--;
                current = Parent(current);
            }

            return path;
        }

        public override float GetPathLength()
        {
            int current = finish;
            if (current == -1) return -1;

            float pathLength = 0;

            int x1 = ToTwoDimX(current);
            int y1 = ToTwoDimY(current);
            current = Parent(current);

            while (current != -1)
            {
                int x2 = ToTwoDimX(current);
                int y2 = ToTwoDimY(current);

                pathLength += ApplyPhysicalWeight(x1, y1, x2, y2);

                current = Parent(current);
                x1 = x2;
                y1 = y2;
            }
            return pathLength;
        }

        protected override bool Selected(int index)
        {
            return Visited(index);
        }

        protected int Parent(int index)
        {
            return Memory.Parent(index);
        }

        protected void SetParent(int index, int value)
        {
            Memory.SetParent(index, value);
        }

        protected float Distance(int index)
        {
            return Memory.Distance(index);
        }

        protected void SetDistance(int index, float value)
        {
            Memory.SetDistance(index, value);
        }

        protected bool Visited(int index)
        {
            return Memory.Visited(index);
        }

        protected void SetVisited(int index, bool value)
        {
            Memory.SetVisited(index, value);
        }
    }
}
