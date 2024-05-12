using System.Collections.Generic;
using System.Drawing;

namespace ThetaStar.Datatypes
{
    /*
     * Contains a [x1,y1,x2,y2] or [x,y] and a colour.
     * Refer to GridObjects.cs for how the path array works.
     */
    public sealed class SnapshotItem
    {
        private static Dictionary<SnapshotItem, SnapshotItem> cached;
        public readonly int[] path;
        public readonly Color color;

        private SnapshotItem(int[] path, Color color)
        {
            this.path = path;
            this.color = color;
        }

        public static SnapshotItem Generate(int[] path, Color color)
        {
            return GetCached(new SnapshotItem(path, color));
        }

        public static SnapshotItem Generate(int[] path)
        {
            return GetCached(new SnapshotItem(path, Color.Empty));
        }

        public static void ClearCached()
        {
            if (cached == null) return;

            cached.Clear();
            cached = null;
        }

        private static SnapshotItem GetCached(SnapshotItem item)
        {
            if (cached == null)
            {
                cached = new Dictionary<SnapshotItem, SnapshotItem>();
            }
            if (!cached.TryGetValue(item, out SnapshotItem get))
            {
                cached[item] = item;
                return item;
            }
            else
            {
                return get;
            }
        }

        public override int GetHashCode()
        {
            const int prime = 31;
            int result = 1;
            result = prime * result + (color.IsEmpty ? 0 : color.GetHashCode());
            result = prime * result + path[0];
            result = prime * result + ((path != null) ? path.GetHashCode() : 0);
            return result;
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            if (GetType() != obj.GetType())
            {
                return false;
            }

            SnapshotItem other = (SnapshotItem)obj;
            if (color.IsEmpty)
            {
                if (!other.color.IsEmpty)
                {
                    return false;
                }
            }
            else if (!color.Equals(other.color))
            {
                return false;
            }

            if (path == null)
            {
                if (other.path != null)
                {
                    return false;
                }
            }
            else if (!Equals(path, other.path))
            {
                return false;
            }

            return true;
        }
    }
}
