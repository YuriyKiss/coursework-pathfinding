using grid;
using priorityqueue;

namespace Algorithms
{
    public class LazyThetaStar : BasicThetaStar
    {
        public LazyThetaStar(GridGraph graph, int sx, int sy, int ex, int ey) : base(graph, sx, sy, ex, ey)
        {
        }

        public override void computePath()
        {
            int totalSize = (graph.sizeX + 1) * (graph.sizeY + 1);

            int start = toOneDimIndex(sx, sy);
            finish = toOneDimIndex(ex, ey);

            pq = new ReusableIndirectHeap(totalSize);
            this.initialiseMemory(totalSize, float.PositiveInfinity, -1, false);

            Initialise(start);

            while (!pq.isEmpty())
            {
                int current = pq.popMinIndex();
                int x = toTwoDimX(current);
                int y = toTwoDimY(current);

                int parentIndex = Parent(current);
                if (parentIndex != -1)
                {
                    int parX = toTwoDimX(parentIndex);
                    int parY = toTwoDimY(parentIndex);

                    if (!graph.LineOfSight(x, y, parX, parY))
                    {
                        FindPath1Parent(current, x, y);
                    }
                }

                if (current == finish || Distance(current) == float.PositiveInfinity)
                {
                    maybeSaveSearchSnapshot();
                    break;
                }
                SetVisited(current, true);

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

        private void FindPath1Parent(int current, int x, int y)
        {
            SetDistance(current, float.PositiveInfinity);
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if (i == 0 && j == 0) continue;
                    int px = x + i;
                    int py = y + j;
                    if (!graph.IsValidBlock(px, py)) continue;
                    int index = graph.ToOneDimIndex(px, py);
                    if (!Visited(index)) continue;
                    if (!graph.NeighbourLineOfSight(x, y, px, py)) continue;

                    float gValue = Distance(index) + graph.Distance(x, y, px, py);
                    if (gValue < Distance(current))
                    {
                        SetDistance(current, gValue);
                        SetParent(current, index);
                    }
                }
            }
        }

        protected override bool Relax(int u, int v, float weightUV)
        {
            if (Parent(u) != -1)
            {
                u = Parent(u);
            }

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
