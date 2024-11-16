using ThetaStar.Pathfinding.Grid;

namespace ThetaStar.Pathfinding.Algorithms.Theta
{
    public class WeightedBasicThetaStar : WeightedAStar
    {
        public WeightedBasicThetaStar(GridGraph graph, int sx, int sy, int ex, int ey) : base(graph, sx, sy, ex, ey)
        {
        }

        public new static WeightedBasicThetaStar PostSmooth(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            WeightedBasicThetaStar bts = new WeightedBasicThetaStar(graph, sx, sy, ex, ey);
            bts.postSmoothingOn = true;
            return bts;
        }

        public static WeightedBasicThetaStar NoHeuristic(GridGraph graph, int sx, int sy, int ex, int ey)
        {
            WeightedBasicThetaStar bts = new WeightedBasicThetaStar(graph, sx, sy, ex, ey);
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
            //if (Parent(current) != -1 && Parent(current) == Parent(destination)) // OPTIMISATION: [TI]
            //    return; // Idea: don't bother trying to relax if parents are equal. using triangle inequality.
            if (!graph.NeighbourLineOfSight(currentX, currentY, x, y))
                return;

            float weight = Weight(currentX, currentY, x, y);
            if (Relax(current, destination, 0))
            {
                // If relaxation is done.
                pq.DecreaseKey(destination, Distance(destination) + weight);
            }
        }

        protected override bool Relax(int u, int v, float weightUV)
        {
            float path1Weight = Distance(u) + PhysicalDistance(u, v);

            int pu = Parent(u);
            /* Path 2 */
            if (LineOfSight(pu, v))
            {
                float path2Weight = Distance(pu) + PhysicalDistance(pu, v);
                if (path1Weight > path2Weight && path2Weight < Distance(v))
                {
                    SetDistance(v, path2Weight);
                    SetParent(v, pu);
                    return true;
                }
            }

            /* Path 1 */
            if (path1Weight < Distance(v)) {
                SetDistance(v, path1Weight);
                SetParent(v, u);
                return true;
            }
            return false;
        }
    }
}
