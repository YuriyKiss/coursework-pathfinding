using System;

namespace algorithms.datatypes
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

        public sealed class Context
        {
            public float[] distance;
            public int[] parent;
            public bool[] visited;
            public float defaultDistance;
            public int defaultParent;
            public bool defaultVisited;
            public int[] ticketCheck;
            public int ticketNumber;
            public int size;

            public Context() { }
        }

        public static void LoadContext(Context context)
        {
            Memory.distance = context.distance;
            Memory.parent = context.parent;
            Memory.visited = context.visited;
            Memory.defaultDistance = context.defaultDistance;
            Memory.defaultParent = context.defaultParent;
            Memory.defaultVisited = context.defaultVisited;
            Memory.ticketCheck = context.ticketCheck;
            Memory.ticketNumber = context.ticketNumber;
            Memory.size = context.size;
        }

        public static void SaveContext(Context context)
        {
            context.distance = Memory.distance;
            context.parent = Memory.parent;
            context.visited = Memory.visited;
            context.defaultDistance = Memory.defaultDistance;
            context.defaultParent = Memory.defaultParent;
            context.defaultVisited = Memory.defaultVisited;
            context.ticketCheck = Memory.ticketCheck;
            context.ticketNumber = Memory.ticketNumber;
            context.size = Memory.size;
        }

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
    }
}
