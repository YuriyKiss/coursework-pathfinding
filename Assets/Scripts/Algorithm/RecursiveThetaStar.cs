using grid;

namespace algorithms
{
    public class RecursiveThetaStar : BasicThetaStar
    {
        public RecursiveThetaStar(GridGraph graph, int startX, int startY, int endX, int endY) : base(graph, startX, startY, endX, endY)
        {
        }

        protected override bool Relax(int u, int v, float weightUV)
        {
            // return true iff relaxation is done.

            if (LineOfSight(Parent(u), v))
            {
                u = Parent(u);
                return Relax(u, v, weightUV);
            }
            else
            {
                float newWeight = Distance(u) + PhysicalDistance(u, v);
                if (newWeight < Distance(v))
                {
                    SetDistance(v, newWeight);
                    SetParent(v, u);
                    //SetParentGranular(v, u);
                    return true;
                }
                return false;
            }
        }

        protected void SetParentGranular(int v, int u)
        {
            int x1 = toTwoDimX(u);
            int y1 = toTwoDimY(u);
            int x2 = toTwoDimX(v);
            int y2 = toTwoDimY(v);

            int dx = x2 - x1;
            int dy = y2 - y1;
            int gcd = GCD(dx, dy);
            if (gcd < 0) gcd = -gcd;
            dx = dx / gcd;
            dy = dy / gcd;
            int par = u;
            for (int i = 1; i <= gcd; ++i)
            {
                x1 += dx;
                y1 += dy;
                int curr = toOneDimIndex(x1, y1);

                SetParent(curr, par);
                par = curr;
            }
        }

        public static int GCD(int a, int b)
        {
            return a == 0 ? b : GCD(b % a, a);
        }
    }
}