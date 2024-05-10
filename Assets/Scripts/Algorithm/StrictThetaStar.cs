using grid;
using priorityqueue;

// Assuming you have the necessary classes like GridGraph, ReusableIndirectHeap, etc.
namespace Algorithms.StrictThetaStar
{
    /// <summary>
    /// An modification of Theta* that I am experimenting with. -Oh
    /// </summary>
    public class StrictThetaStar : BasicThetaStar
    {
        private float BUFFER_VALUE = 0.42f;

        public StrictThetaStar(GridGraph graph, int sx, int sy, int ex, int ey) : base(graph, sx, sy, ex, ey)
        {
        }

        public static StrictThetaStar SetBuffer(GridGraph graph, int sx, int sy, int ex, int ey, float bufferValue)
        {
            StrictThetaStar algo = new StrictThetaStar(graph, sx, sy, ex, ey);
            algo.BUFFER_VALUE = bufferValue;
            return algo;
        }

        public new static StrictThetaStar NoHeuristic(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            StrictThetaStar algo = new StrictThetaStar(graph, sx, sy, ex, ey);
            algo.heuristicWeight = 0f;
            return algo;
        }

        public new static StrictThetaStar PostSmooth(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            StrictThetaStar algo = new StrictThetaStar(graph, sx, sy, ex, ey);
            algo.postSmoothingOn = true;
            return algo;
        }

        public override void computePath()
        {
            int totalSize = (graph.sizeX + 1) * (graph.sizeY + 1);

            int start = toOneDimIndex(sx, sy);
            finish = toOneDimIndex(ex, ey);

            pq = new ReusableIndirectHeap(totalSize);
            initialiseMemory(totalSize, float.PositiveInfinity, -1, false);

            Initialise(start);

            while (!pq.isEmpty())
            {
                int current = pq.popMinIndex();
                TryFixBufferValue(current);

                if (current == finish || Distance(current) == float.PositiveInfinity)
                {
                    maybeSaveSearchSnapshot();
                    break;
                }
                SetVisited(current, true);

                int x = toTwoDimX(current);
                int y = toTwoDimY(current);

                TryRelaxNeighbour(current, x, y, x - 1, y - 1);
                TryRelaxNeighbour(current, x, y, x, y - 1);
                TryRelaxNeighbour(current, x, y, x + 1, y - 1);

                TryRelaxNeighbour(current, x, y, x - 1, y);
                TryRelaxNeighbour(current, x, y, x + 1, y);

                TryRelaxNeighbour(current, x, y, x - 1, y + 1);
                TryRelaxNeighbour(current, x, y, x, y + 1);
                TryRelaxNeighbour(current, x, y, x + 1, y + 1);

                maybeSaveSearchSnapshot();
            }

            MaybePostSmooth();
        }

        protected new float Heuristic(int x, int y)
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

        protected new void TryRelaxNeighbour(int current, int currentX, int currentY, int x, int y)
        {
            if (!graph.IsValidCoordinate(x, y))
                return;

            int destination = toOneDimIndex(x, y);
            if (Visited(destination))
                return;
            if (Parent(current) != -1 && Parent(current) == Parent(destination)) // OPTIMISATION: [TI]
                return; // Idea: don't bother trying to relax if parents are equal. using triangle inequality.
            if (!graph.NeighbourLineOfSight(currentX, currentY, x, y))
                return;

            if (Relax(current, destination, Weight(currentX, currentY, x, y)))
            {
                // If relaxation is done.
                pq.decreaseKey(destination, Distance(destination) + Heuristic(x, y));
            }
        }

        protected override bool Relax(int u, int v, float weightUV)
        {
            // return true iff relaxation is done.
            int par = Parent(u);
            if (LineOfSight(Parent(u), v))
            {
                float newWeight = Distance(par) + PhysicalDistance(par, v);
                return RelaxTarget(v, par, newWeight);
            }
            else
            {
                float newWeight = Distance(u) + PhysicalDistance(u, v);
                return RelaxTarget(v, u, newWeight);
            }
        }

        private bool RelaxTarget(int v, int par, float newWeight)
        {
            if (newWeight < Distance(v))
            {
                if (!IsTaut(v, par))
                {
                    newWeight += BUFFER_VALUE;
                    par += int.MinValue;
                }
                SetDistance(v, newWeight);
                SetParent(v, par);
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
            int x1 = toTwoDimX(v);
            int y1 = toTwoDimY(v);
            int x2 = toTwoDimX(u);
            int y2 = toTwoDimY(u);
            int x3 = toTwoDimX(p);
            int y3 = toTwoDimY(p);
            return graph.IsTaut(x1, y1, x2, y2, x3, y3);
        }
    }
}