using algorithms.datatypes;
using grid;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace algorithms
{
    public abstract class PathFindingAlgorithm
    {
        private const int SNAPSHOT_INTERVAL = 0;
        private int snapshotCountdown = 0;

        private List<List<SnapshotItem>> snapshotList;
        protected GridGraph graph;

        protected int[] parent;
        protected readonly int sizeX;
        protected readonly int sizeXplusOne;
        protected readonly int sizeY;

        protected readonly int sx;
        protected readonly int sy;
        protected readonly int ex;
        protected readonly int ey;

        private int ticketNumber = -1;

        private bool recordingMode;
        private bool usingStaticMemory = false;

        public PathFindingAlgorithm(GridGraph graph, int sizeX, int sizeY, int sx, int sy, int ex, int ey)
        {
            this.graph = graph;
            this.sizeX = sizeX;
            this.sizeXplusOne = sizeX + 1;
            this.sizeY = sizeY;
            this.sx = sx;
            this.sy = sy;
            this.ex = ex;
            this.ey = ey;
            snapshotList = new List<List<SnapshotItem>>();
        }

        protected void initialiseMemory(int size, float defaultDistance, int defaultParent, bool defaultVisited)
        {
            usingStaticMemory = true;
            ticketNumber = Memory.Initialise(size, defaultDistance, defaultParent, defaultVisited);
        }

        public void startRecording()
        {
            recordingMode = true;
        }

        public void stopRecording()
        {
            recordingMode = false;
        }

        public List<List<SnapshotItem>> retrieveSnapshotList()
        {
            return snapshotList;
        }

        public abstract void computePath();

        public abstract int[][] getPath();

        public abstract float getPathLength();

        public virtual void printStatistics()
        {
        }

        protected int toOneDimIndex(int x, int y)
        {
            return graph.ToOneDimIndex(x, y);
        }

        protected int toTwoDimX(int index)
        {
            return graph.ToTwoDimX(index);
        }

        protected int toTwoDimY(int index)
        {
            return graph.ToTwoDimY(index);
        }

        protected void maybeSaveSearchSnapshot()
        {
            if (recordingMode)
            {
                if (usingStaticMemory && ticketNumber != Memory.CurrentTicket())
                    throw new Exception("Ticket does not match!");

                saveSearchSnapshot();
            }
        }

        public List<SnapshotItem> getCurrentSearchSnapshot()
        {
            return computeSearchSnapshot();
        }

        protected bool isRecording()
        {
            return recordingMode;
        }

        private void saveSearchSnapshot()
        {
            if (snapshotCountdown > 0)
            {
                snapshotCountdown--;
                return;
            }
            snapshotCountdown = SNAPSHOT_INTERVAL;

            snapshotList.Add(computeSearchSnapshot());
        }

        protected void addSnapshot(List<SnapshotItem> snapshotItemList)
        {
            snapshotList.Add(snapshotItemList);
        }

        protected int goalParentIndex()
        {
            return toOneDimIndex(ex, ey);
        }

        private int getParent(int index)
        {
            if (usingStaticMemory) return Memory.Parent(index);
            else return parent[index];
        }

        private void setParent(int index, int value)
        {
            if (usingStaticMemory) Memory.SetParent(index, value);
            else parent[index] = value;
        }

        protected int getSize()
        {
            if (usingStaticMemory) return Memory.Size();
            else return parent.Length;
        }

        protected List<SnapshotItem> computeSearchSnapshot()
        {
            List<SnapshotItem> list = new List<SnapshotItem>();
            int current = goalParentIndex();
            HashSet<int> finalPathSet = null;
            if (getParent(current) >= 0)
            {
                finalPathSet = new HashSet<int>();
                while (current >= 0)
                {
                    finalPathSet.Add(current);
                    current = getParent(current);
                }
            }

            int size = getSize();
            for (int i = 0; i < size; i++)
            {
                if (getParent(i) != -1)
                {
                    if (finalPathSet != null && finalPathSet.Contains(i))
                    {
                        list.Add(SnapshotItem.Generate(snapshotEdge(i), Color.Blue));
                    }
                    else
                    {
                        list.Add(SnapshotItem.Generate(snapshotEdge(i)));
                    }
                }
                int[] vertexSnapshot = snapshotVertex(i);
                if (vertexSnapshot != null)
                {
                    list.Add(SnapshotItem.Generate(vertexSnapshot));
                }
            }

            return list;
        }

        protected int[] snapshotEdge(int endIndex)
        {
            int[] edge = new int[4];
            int startIndex = getParent(endIndex);
            edge[2] = toTwoDimX(endIndex);
            edge[3] = toTwoDimY(endIndex);
            if (startIndex < 0)
            {
                edge[0] = edge[2];
                edge[1] = edge[3];
            }
            else
            {
                edge[0] = toTwoDimX(startIndex);
                edge[1] = toTwoDimY(startIndex);
            }

            return edge;
        }

        protected int[] snapshotVertex(int index)
        {
            if (selected(index))
            {
                int[] edge = new int[2];
                edge[0] = toTwoDimX(index);
                edge[1] = toTwoDimY(index);
                return edge;
            }
            return null;
        }

        protected virtual bool selected(int index)
        {
            return false;
        }

        protected void inheritSnapshotListFrom(PathFindingAlgorithm algo)
        {
            this.snapshotList = algo.snapshotList;
        }
    }
}