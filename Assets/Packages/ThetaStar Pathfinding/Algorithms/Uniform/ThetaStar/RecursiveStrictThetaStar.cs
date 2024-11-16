using ThetaStar.Pathfinding.Grid;
using ThetaStar.Pathfinding.PriorityQueue;

namespace ThetaStar.Pathfinding.Algorithms.Theta
{
    public class RecursiveStrictThetaStar : BasicThetaStar
    {
        private int DEPTH_LIMIT = -1;
        private float BUFFER_VALUE = 0.42f;

        public RecursiveStrictThetaStar(GridGraph graph, int sx, int sy, int ex, int ey) : base(graph, sx, sy, ex, ey)
        {
        }

        public static RecursiveStrictThetaStar SetBuffer(GridGraph graph, int sx, int sy, int ex, int ey, float bufferValue)
        {
            RecursiveStrictThetaStar algo = new RecursiveStrictThetaStar(graph, sx, sy, ex, ey);
            algo.BUFFER_VALUE = bufferValue;
            return algo;
        }

        public new static RecursiveStrictThetaStar NoHeuristic(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            RecursiveStrictThetaStar algo = new RecursiveStrictThetaStar(graph, sx, sy, ex, ey);
            algo.heuristicWeight = 0f;
            return algo;
        }

        public static RecursiveStrictThetaStar DepthLimit(GridGraph graph, int sx, int sy, int ex, int ey, int depthLimit)
        {
            RecursiveStrictThetaStar algo = new RecursiveStrictThetaStar(graph, sx, sy, ex, ey);
            algo.DEPTH_LIMIT = depthLimit;
            return algo;
        }

        public new static RecursiveStrictThetaStar PostSmooth(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            RecursiveStrictThetaStar algo = new RecursiveStrictThetaStar(graph, sx, sy, ex, ey);
            algo.postSmoothingOn = true;
            return algo;
        }

        public override void ComputePath()
        {
            int totalSize = (graph.sizeX + 1) * (graph.sizeY + 1);

            int start = ToOneDimIndex(sx, sy);
            finish = ToOneDimIndex(ex, ey);

            pq = new ReusableIndirectHeap(totalSize);
            this.InitialiseMemory(totalSize, float.PositiveInfinity, -1, false);

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
            if (Parent(current) != -1 && Parent(current) == Parent(destination)) // OPTIMISATION: [TI]
                return; // Idea: don't bother trying to relax if parents are equal. using triangle inequality.
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
            // return true iff relaxation is done.
            return TautRelax(u, v, DEPTH_LIMIT);
        }

        // This gives very good paths... but the recursion level is too deep.
        private bool TautRelax(int u, int v, int depth)
        {
            if (IsTaut(v, u))
            {
                return TryRelaxVertex(u, v, false);
            }
            else
            {
                int par = Parent(u);
                if (LineOfSight(par, v))
                {
                    if (depth == 0)
                    {
                        return TryRelaxVertex(par, v, !IsTaut(v, par));
                    }
                    return TautRelax(par, v, depth - 1);
                }
                else
                {
                    return TryRelaxVertex(u, v, true);
                }
            }
        }

        private bool TryRelaxVertex(int u, int v, bool addBuffer)
        {
            int newParent = u;
            float newWeight = Distance(u) + PhysicalDistance(u, v);
            if (addBuffer)
            {
                newWeight += BUFFER_VALUE;
                newParent += int.MinValue;
            }
            if (newWeight < Distance(v))
            {
                if (IsMergeableWithParent(u, v))
                {
                    newParent = Parent(u);
                }
                SetDistance(v, newWeight);
                SetParent(v, newParent);
                return true;
            }
            return false;
        }

        // If parent(u),u,v collinear, remove u from path, except when u is at an outer corner.
        private bool IsMergeableWithParent(int u, int v)
        {
            if (u == -1) return false;
            int p = Parent(u);
            if (p == -1) return false; // u is start point.
            int ux = ToTwoDimX(u);
            int uy = ToTwoDimY(u);
            if (IsOuterCorner(ux, uy)) return false; // u is outer corner

            int vx = ToTwoDimX(v);
            int vy = ToTwoDimY(v);
            int px = ToTwoDimX(p);
            int py = ToTwoDimY(p);

            return IsCollinear(px, py, ux, uy, vx, vy);
        }

        protected bool IsOuterCorner(int x, int y)
        {
            bool a = graph.IsBlocked(x - 1, y - 1);
            bool b = graph.IsBlocked(x, y - 1);
            bool c = graph.IsBlocked(x, y);
            bool d = graph.IsBlocked(x - 1, y);

            return ((!a && !c) || (!d && !b)) && (a || b || c || d);
        }

        protected bool IsCollinear(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            // (y2-y1)/(x2-x1) == (y3-y2)/(x3-x2)
            // <=>
            return (y2 - y1) * (x3 - x2) == (y3 - y2) * (x2 - x1);
        }

        /**
         * Checks whether the path v, u, p=parent(u) is taut.
         */
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
