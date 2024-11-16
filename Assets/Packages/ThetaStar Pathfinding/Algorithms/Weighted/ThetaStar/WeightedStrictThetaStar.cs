using ThetaStar.Pathfinding.Grid;
using ThetaStar.Pathfinding.PriorityQueue;

// Assuming you have the necessary classes like GridGraph, ReusableIndirectHeap, etc.
namespace ThetaStar.Pathfinding.Algorithms.Theta
{
    /// <summary>
    /// An modification of Theta* that I am experimenting with. -Oh
    /// </summary>
    public class WeightedStrictThetaStar : WeightedBasicThetaStar
    {
        private float BUFFER_VALUE = 0.42f;

        public WeightedStrictThetaStar(GridGraph graph, int sx, int sy, int ex, int ey) : base(graph, sx, sy, ex, ey)
        {
        }

        public static WeightedStrictThetaStar SetBuffer(GridGraph graph, int sx, int sy, int ex, int ey, float bufferValue)
        {
            WeightedStrictThetaStar algo = new WeightedStrictThetaStar(graph, sx, sy, ex, ey);
            algo.BUFFER_VALUE = bufferValue;
            return algo;
        }

        public new static WeightedStrictThetaStar NoHeuristic(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            WeightedStrictThetaStar algo = new WeightedStrictThetaStar(graph, sx, sy, ex, ey);
            algo.heuristicWeight = 0f;
            return algo;
        }

        public new static WeightedStrictThetaStar PostSmooth(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            WeightedStrictThetaStar algo = new WeightedStrictThetaStar(graph, sx, sy, ex, ey);
            algo.postSmoothingOn = true;
            return algo;
        }

        public override void ComputePath()
        {
            int totalSize = (graph.sizeX + 1) * (graph.sizeY + 1);

            int start = ToOneDimIndex(sx, sy);
            finish = ToOneDimIndex(ex, ey);

            pq = new ReusableIndirectHeap(totalSize);
            InitialiseMemory(totalSize, float.PositiveInfinity, -1, false);

            Initialise(start);

            while (!pq.IsEmpty())
            {
                int current = pq.PopMinIndex();
                TryFixBufferValue(current);

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

                MaybeSaveSearchSnapshot();
            }

            MaybePostSmooth();
        }

        protected override float Heuristic(int x, int y)
        {
            return heuristicWeight * graph.Distance(x, y, ex, ey);

            // MOD 2 :: Increased Goal Heuristic - Not needed when a Penalty value of 0.42 is used.
            /*if (x == Ex && y == Ey)
            {
                return 1.1f;
            }
            else
            {
                return HeuristicWeight * Graph.Distance(x, y, Ex, Ey);
            }*/
        }

        private void TryFixBufferValue(int current)
        {
            if (Parent(current) < 0 && Parent(current) != -1)
            {
                SetParent(current, Parent(current) - int.MinValue);
                SetDistance(current, Distance(Parent(current)) + PhysicalDistance(current, Parent(current)));
            }
        }

        protected override void TryRelaxNeighbour(int current, int currentX, int currentY, int x, int y)
        {
            if (!graph.IsValidCoordinate(x, y))
                return;

            int destination = ToOneDimIndex(x, y);
            if (Visited(destination))
                return;
            //if (Parent(current) != -1 && Parent(current) == Parent(destination)) // OPTIMISATION: [TI]
            //    return; // Idea: don't bother trying to relax if parents are equal. using triangle inequality.
            if (!graph.NeighbourLineOfSight(currentX, currentY, x, y))
                return;

            if (Relax(current, destination, Weight(currentX, currentY, x, y)))
            {
                // If relaxation is done.
                pq.DecreaseKey(destination, Distance(destination) + Heuristic(x, y));
            }
        }

        protected override bool Relax(int u, int v, float weightUV)
        {
            float path1Weight = Distance(u) + PhysicalDistance(u, v);

            // return true if relaxation is done.
            int par = Parent(u);
            if (LineOfSight(Parent(u), v))
            {
                float path2Weight = Distance(par) + PhysicalDistance(par, v);
                if (path1Weight > path2Weight && path2Weight < Distance(v)) {
                    if (!IsTaut(v, par)) {
                        path2Weight += BUFFER_VALUE;
                        par += int.MinValue;
                    }
                    SetDistance(v, path2Weight);
                    SetParent(v, par);
                    return true;
                }
            }
            
            if (path1Weight < Distance(v)) {
                if (!IsTaut(v, u)) {
                    path1Weight += BUFFER_VALUE;
                    u += int.MinValue;
                }
                SetDistance(v, path1Weight);
                SetParent(v, u);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks whether the path v, u, p=parent(u) is taut.
        /// </summary>
        private bool IsTaut(int v, int u)
        {
            int p = Parent(u); // assert u != -1
            if (p == -1) return true;
            int x1 = ToTwoDimX(v);
            int y1 = ToTwoDimY(v);
            int x2 = ToTwoDimX(u);
            int y2 = ToTwoDimY(u);
            int x3 = ToTwoDimX(p);
            int y3 = ToTwoDimY(p);
            return graph.IsTaut(x1, y1, x2, y2, x3, y3);
        }
    }
}