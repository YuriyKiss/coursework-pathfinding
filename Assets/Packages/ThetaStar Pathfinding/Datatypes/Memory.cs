using System;

namespace ThetaStar.Datatypes
{
    public static class Memory
    {
        private static float[] distance;
        private static int[] parent;
        private static bool[] visited;

        private static float defaultDistance = 0;
        private static int defaultParent = -1;
        private static bool defaultVisited = false;

        private static int[] ticketCheck;
        private static int ticketNumber = 0;

        private static int size = 0;

        public static int Initialise(int size, float defaultDistance, int defaultParent, bool defaultVisited)
        {
            Memory.defaultDistance = defaultDistance;
            Memory.defaultParent = defaultParent;
            Memory.defaultVisited = defaultVisited;
            Memory.size = size;

            if (ticketCheck == null || ticketCheck.Length != size)
            {
                distance = new float[size];
                parent = new int[size];
                visited = new bool[size];
                ticketCheck = new int[size];
                ticketNumber = 1;
            }
            else if (ticketNumber == -1)
            {
                ticketCheck = new int[size];
                ticketNumber = 1;
            }
            else
            {
                ticketNumber++;
            }

            return ticketNumber;
        }

        public static int CurrentTicket()
        {
            return ticketNumber;
        }

        public static int Size()
        {
            return size;
        }

        public static float Distance(int index)
        {
            if (ticketCheck[index] != ticketNumber) return defaultDistance;

            return distance[index];
        }

        public static int Parent(int index)
        {
            if (ticketCheck[index] != ticketNumber) return defaultParent;

            return parent[index];
        }

        public static bool Visited(int index)
        {
            if (ticketCheck[index] != ticketNumber) return defaultVisited;

            return visited[index];
        }

        public static void SetDistance(int index, float value)
        {
            if (ticketCheck[index] != ticketNumber)
            {
                distance[index] = value;
                parent[index] = defaultParent;
                visited[index] = defaultVisited;
                ticketCheck[index] = ticketNumber;
            }
            else
            {
                distance[index] = value;
            }
        }

        public static void SetParent(int index, int value)
        {
            if (ticketCheck[index] != ticketNumber)
            {
                distance[index] = defaultDistance;
                parent[index] = value;
                visited[index] = defaultVisited;
                ticketCheck[index] = ticketNumber;
            }
            else
            {
                parent[index] = value;
            }
        }

        public static void SetVisited(int index, bool value)
        {
            if (ticketCheck[index] != ticketNumber)
            {
                distance[index] = defaultDistance;
                parent[index] = defaultParent;
                visited[index] = value;
                ticketCheck[index] = ticketNumber;
            }
            else
            {
                visited[index] = value;
            }
        }

        public static void ClearMemory()
        {
            distance = null;
            parent = null;
            visited = null;
            ticketCheck = null;
            GC.Collect();
        }

        // Memory Context struct
        public struct Context
        {
            public float[] Distance;
            public int[] Parent;
            public bool[] Visited;
            public float DefaultDistance;
            public int DefaultParent;
            public bool DefaultVisited;
            public int[] TicketCheck;
            public int TicketNumber;
            public int Size;
        }

        public static void LoadContext(Context context)
        {
            distance = context.Distance;
            parent = context.Parent;
            visited = context.Visited;
            defaultDistance = context.DefaultDistance;
            defaultParent = context.DefaultParent;
            defaultVisited = context.DefaultVisited;
            ticketCheck = context.TicketCheck;
            ticketNumber = context.TicketNumber;
            size = context.Size;
        }

        public static void SaveContext(Context context)
        {
            context.Distance = distance;
            context.Parent = parent;
            context.Visited = visited;
            context.DefaultDistance = defaultDistance;
            context.DefaultParent = defaultParent;
            context.DefaultVisited = defaultVisited;
            context.TicketCheck = ticketCheck;
            context.TicketNumber = ticketNumber;
            context.Size = size;
        }
    }
}
