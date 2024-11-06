using System;
using System.Text;

namespace ThetaStar.Pathfinding.PriorityQueue
{
    /*
     * Indirect binary heap. Used for O(lgn) deleteMin and O(lgn) decreaseKey.
     */
    public class ReusableIndirectHeap
    {
        private static readonly object lockObject = new object();

        private static float[] keyList;
        private static int[] inList;
        private static int[] outList;
        private int heapSize;

        private static float defaultKey = float.PositiveInfinity;

        private static int[] ticketCheck;
        private static int ticketNumber = 0;

        public static void Initialise(int size, float defaultKey)
        {
            ReusableIndirectHeap.defaultKey = defaultKey;

            if (ticketCheck == null || ticketCheck.Length != size)
            {
                //Console.WriteLine("REINITIALISE HEAP " + size);
                keyList = new float[size];
                inList = new int[size];
                outList = new int[size];
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
        }

        public static float GetKey(int index)
        {
            return ticketCheck[index] == ticketNumber ? keyList[index] : defaultKey;
        }

        public static int GetIn(int index)
        {
            return ticketCheck[index] == ticketNumber ? inList[index] : index;
        }

        public static int GetOut(int index)
        {
            return ticketCheck[index] == ticketNumber ? outList[index] : index;
        }

        public static void SetKey(int index, float value)
        {
            if (ticketCheck[index] != ticketNumber)
            {
                keyList[index] = value;
                inList[index] = index;
                outList[index] = index;
                ticketCheck[index] = ticketNumber;
            }
            else
            {
                keyList[index] = value;
            }
        }

        public static void SetIn(int index, int value)
        {
            if (ticketCheck[index] != ticketNumber)
            {
                keyList[index] = defaultKey;
                inList[index] = value;
                outList[index] = index;
                ticketCheck[index] = ticketNumber;
            }
            else
            {
                inList[index] = value;
            }
        }

        public static void SetOut(int index, int value)
        {
            if (ticketCheck[index] != ticketNumber)
            {
                keyList[index] = defaultKey;
                inList[index] = index;
                outList[index] = value;
                ticketCheck[index] = ticketNumber;
            }
            else
            {
                outList[index] = value;
            }
        }

        /*
         * Runtime: O(1)
         */
        public ReusableIndirectHeap(int size)
        {
            Initialise(size, float.PositiveInfinity);
            heapSize = 0;
        }

        /*
         * Runtime: O(1)
         */
        public ReusableIndirectHeap(int size, int memorySize)
        {
            Initialise(memorySize, float.PositiveInfinity);
            heapSize = 0;
        }

        private void BubbleUp(int index)
        {
            int parent = (index - 1) / 2;
            while (index > 0 && GetKey(index) < GetKey(parent))
            {
                // If meets the conditions to bubble up,
                SwapData(index, parent);
                index = parent;
                parent = (index - 1) / 2;
            }
        }

        private void SwapData(int a, int b)
        {
            // s = Data at a = out[a]
            // t = Data at b = out[b]
            // key[a] <-> key[b]
            // in[s] <-> in[t]
            // out[a] <-> out[b]

            int s = GetOut(a);
            int t = GetOut(b);

            SwapKey(a, b);
            SwapIn(s, t);
            SwapOut(a, b);
        }

        /*
         * swap integers in list
         */
        private void SwapKey(int i1, int i2)
        {
            float temp = GetKey(i1);
            SetKey(i1, GetKey(i2));
            SetKey(i2, temp);
        }

        /*
         * swap integers in list
         */
        private void SwapOut(int i1, int i2)
        {
            int temp = GetOut(i1);
            SetOut(i1, GetOut(i2));
            SetOut(i2, temp);
        }

        /*
         * swap integers in list
         */
        private void SwapIn(int i1, int i2)
        {
            int temp = GetIn(i1);
            SetIn(i1, GetIn(i2));
            SetIn(i2, temp);
        }

        private int SmallerNode(int index1, int index2)
        {
            if (index1 >= heapSize)
            {
                if (index2 >= heapSize)
                    return -1;

                return index2;
            }
            if (index2 >= heapSize)
                return index1;

            return GetKey(index1) < GetKey(index2) ? index1 : index2;
        }

        private void BubbleDown(int index)
        {
            int leftChild = 2 * index + 1;
            int rightChild = 2 * index + 2;
            int smallerChild = SmallerNode(leftChild, rightChild);

            while (smallerChild != -1 && GetKey(index) > GetKey(smallerChild))
            {
                // If meets the conditions to bubble down,
                SwapData(index, smallerChild);

                // Recurse
                index = smallerChild;
                leftChild = 2 * index + 1;
                rightChild = 2 * index + 2;
                smallerChild = SmallerNode(leftChild, rightChild);
            }
        }

        /*
         * Runtime: O(lgn)
         */
        public void DecreaseKey(int outIndex, float newKey)
        {
            lock (lockObject) {
                // Assume newKey < old key
                int inIndex = GetIn(outIndex);

                // Optimisation: Jump the newly set value to the bottom of the heap.
                // Faster if there are a lot of POSITIVE_INFINITY values.
                // This is equivalent to an insert operation.
                if (GetKey(inIndex) == float.PositiveInfinity) {
                    SwapData(inIndex, heapSize);
                    inIndex = heapSize;
                    ++heapSize;
                }
                SetKey(inIndex, newKey);

                BubbleUp(inIndex);
            }
        }

        public float GetMinValue()
        {
            return GetKey(0);
        }

        /**
         * Runtime: O(lgn)
         * @return index of min element
         */
        public int PopMinIndex()
        {
            if (heapSize == 0)
                throw new NullReferenceException("Indirect Heap is empty!");
            else if (heapSize == 1)
            {
                --heapSize;
                return GetOut(0);
            }
            // nodeList.size() > 1

            // s = Data at 0 = out[0]
            // t = Data at lastIndex = out[lastIndex]
            // key[0] = key[lastIndex], remove key[lastIndex]
            // in[s] = -1
            // in[t] = 0
            // out[0] = out[lastIndex], remove out[lastIndex]

            //E temp = keyList.get(0);
            int s = GetOut(0);
            SwapData(0, heapSize - 1);

            --heapSize;
            BubbleDown(0);

            return s;
        }

        public string ArrayToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < ticketCheck.Length; i++)
            {
                if (i == heapSize) sb.Append("* ");
                sb.Append("[");
                sb.Append(GetOut(i));
                sb.Append(" ");
                float val = GetKey(i);
                sb.Append(val == float.PositiveInfinity ? "_" : (int)val);
                sb.Append("], ");
            }
            return GetIn(1) + " / " + sb.ToString();
        }

        /*
        private static final int parent(int index) {
            return (index-1)/2;
        }
    
        private static final int leftChild(int index) {
            return 2*index+1;
        }
    
        private static final int rightChild(int index) {
            return 2*index+2;
        }
        */

        public int Size()
        {
            return heapSize;
        }

        public bool IsEmpty()
        {
            return heapSize <= 0;
        }

        public static void ClearMemory()
        {
            keyList = null;
            inList = null;
            outList = null;
            ticketCheck = null;
            GC.Collect();
        }

        // Reusable Indirect Heap Context
        public struct Context
        {
            public float[] keyList;
            public int[] inList;
            public int[] outList;
            public int[] ticketCheck;
            public int ticketNumber;
        }

        public static void LoadContext(Context context)
        {
            keyList = context.keyList;
            inList = context.inList;
            outList = context.outList;
            ticketCheck = context.ticketCheck;
            ticketNumber = context.ticketNumber;
        }

        public static void SaveContext(Context context)
        {
            context.keyList = keyList;
            context.inList = inList;
            context.outList = outList;
            context.ticketCheck = ticketCheck;
            context.ticketNumber = ticketNumber;
        }
    }
}