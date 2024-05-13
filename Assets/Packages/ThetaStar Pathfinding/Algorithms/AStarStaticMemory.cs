using ThetaStar.Datatypes;
using ThetaStar.Grid;
using ThetaStar.PriorityQueue;

namespace ThetaStar.Algorithms
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

            if (Relax(current, destination, Weight(currentX, currentY, x, y)))
            {
                // If relaxation is done.
                pq.DecreaseKey(destination, Distance(destination) + Heuristic(x, y));
            }
        }

        protected virtual float Heuristic(int x, int y)
        {
            return heuristicWeight * graph.Distance(x, y, ex, ey);
        }

        protected float Weight(int x1, int y1, int x2, int y2)
        {
            return graph.Distance(x1, y1, x2, y2);
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
            return graph.Distance(x1, y1, x2, y2);
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
                    PostSmooth();
                }
            }
        }

        private bool PostSmooth()
        {
            bool didSomething = false;

            int current = finish;
            while (current != -1)
            {
                int next = Parent(current); // we can skip checking this one as it always has LoS to current.
                if (next != -1)
                {
                    next = Parent(next);
                    while (next != -1)
                    {
                        if (LineOfSight(current, next))
                        {
                            SetParent(current, next);
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

            int prevX = ToTwoDimX(current);
            int prevY = ToTwoDimY(current);
            current = Parent(current);

            while (current != -1)
            {
                int x = ToTwoDimX(current);
                int y = ToTwoDimY(current);

                pathLength += graph.Distance(x, y, prevX, prevY);

                current = Parent(current);
                prevX = x;
                prevY = y;
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
