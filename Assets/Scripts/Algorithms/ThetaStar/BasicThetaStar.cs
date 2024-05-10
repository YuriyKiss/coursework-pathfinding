using Grid;

namespace Algorithms.Theta
{
    public class BasicThetaStar : AStarStaticMemory
    {

        public BasicThetaStar(GridGraph graph, int sx, int sy, int ex, int ey) : base(graph, sx, sy, ex, ey)
        {
        }

        public new static BasicThetaStar PostSmooth(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            BasicThetaStar bts = new BasicThetaStar(graph, sx, sy, ex, ey);
            bts.postSmoothingOn = true;
            return bts;
        }

        public static BasicThetaStar NoHeuristic(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            BasicThetaStar bts = new BasicThetaStar(graph, sx, sy, ex, ey);
            bts.heuristicWeight = 0;
            return bts;
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

            if (Relax(current, destination, 0))
            {
                // If relaxation is done.
                pq.DecreaseKey(destination, Distance(destination) + Heuristic(x, y));
            }
        }

        protected override bool Relax(int u, int v, float weightUV)
        {
            // return true iff relaxation is done.

            if (LineOfSight(Parent(u), v))
            {
                u = Parent(u);

                float newWeight = Distance(u) + PhysicalDistance(u, v);
                if (newWeight < Distance(v))
                {
                    SetDistance(v, newWeight);
                    SetParent(v, u);
                    return true;
                }
                return false;
            }
            else
            {
                float newWeight = Distance(u) + PhysicalDistance(u, v);
                if (newWeight < Distance(v))
                {
                    SetDistance(v, newWeight);
                    SetParent(v, u);
                    return true;
                }
                return false;
            }
        }

    }
}
