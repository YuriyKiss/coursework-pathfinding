using System;
using System.Collections.Generic;
using System.Drawing;
using ThetaStar.Pathfinding.Datatypes;
using ThetaStar.Pathfinding.Grid;
using ThetaStar.Pathfinding.PriorityQueue;

namespace ThetaStar.Pathfinding.Algorithms
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

        public static void ClearStaticData()
        {
            Memory.ClearMemory();
            SnapshotItem.ClearCached();
            ReusableIndirectHeap.ClearMemory();
        }

        protected void InitialiseMemory(int size, float defaultDistance, int defaultParent, bool defaultVisited)
        {
            usingStaticMemory = true;
            ticketNumber = Memory.Initialise(size, defaultDistance, defaultParent, defaultVisited);
        }

        public void StartRecording()
        {
            recordingMode = true;
        }

        public void StopRecording()
        {
            recordingMode = false;
        }

        public List<List<SnapshotItem>> RetrieveSnapshotList()
        {
            return snapshotList;
        }

        public abstract void ComputePath();

        public abstract int[][] GetPath();

        public abstract float GetPathLength();

        public virtual void PrintStatistics() { }

        protected int ToOneDimIndex(int x, int y)
        {
            return graph.ToOneDimIndex(x, y);
        }

        protected int ToTwoDimX(int index)
        {
            return graph.ToTwoDimX(index);
        }

        protected int ToTwoDimY(int index)
        {
            return graph.ToTwoDimY(index);
        }

        protected void MaybeSaveSearchSnapshot()
        {
            if (recordingMode)
            {
                if (usingStaticMemory && ticketNumber != Memory.CurrentTicket())
                    throw new Exception("Ticket does not match!");

                SaveSearchSnapshot();
            }
        }

        public List<SnapshotItem> GetCurrentSearchSnapshot()
        {
            return ComputeSearchSnapshot();
        }

        protected bool IsRecording()
        {
            return recordingMode;
        }

        private void SaveSearchSnapshot()
        {
            if (snapshotCountdown > 0)
            {
                snapshotCountdown--;
                return;
            }
            snapshotCountdown = SNAPSHOT_INTERVAL;

            snapshotList.Add(ComputeSearchSnapshot());
        }

        protected void AddSnapshot(List<SnapshotItem> snapshotItemList)
        {
            snapshotList.Add(snapshotItemList);
        }

        protected int GoalParentIndex()
        {
            return ToOneDimIndex(ex, ey);
        }

        private int GetParent(int index)
        {
            if (usingStaticMemory) return Memory.Parent(index);
            else return parent[index];
        }

        private void SetParent(int index, int value)
        {
            if (usingStaticMemory) Memory.SetParent(index, value);
            else parent[index] = value;
        }

        protected int GetSize()
        {
            if (usingStaticMemory) return Memory.Size();
            else return parent.Length;
        }

        protected List<SnapshotItem> ComputeSearchSnapshot()
        {
            List<SnapshotItem> list = new List<SnapshotItem>();
            int current = GoalParentIndex();
            HashSet<int> finalPathSet = null;
            if (GetParent(current) >= 0)
            {
                finalPathSet = new HashSet<int>();
                while (current >= 0)
                {
                    finalPathSet.Add(current);
                    current = GetParent(current);
                }
            }

            int size = GetSize();
            for (int i = 0; i < size; i++)
            {
                if (GetParent(i) != -1)
                {
                    if (finalPathSet != null && finalPathSet.Contains(i))
                    {
                        list.Add(SnapshotItem.Generate(SnapshotEdge(i), Color.Blue));
                    }
                    else
                    {
                        list.Add(SnapshotItem.Generate(SnapshotEdge(i)));
                    }
                }
                int[] vertexSnapshot = SnapshotVertex(i);
                if (vertexSnapshot != null)
                {
                    list.Add(SnapshotItem.Generate(vertexSnapshot));
                }
            }

            return list;
        }

        protected int[] SnapshotEdge(int endIndex)
        {
            int[] edge = new int[4];
            int startIndex = GetParent(endIndex);
            edge[2] = ToTwoDimX(endIndex);
            edge[3] = ToTwoDimY(endIndex);
            if (startIndex < 0)
            {
                edge[0] = edge[2];
                edge[1] = edge[3];
            }
            else
            {
                edge[0] = ToTwoDimX(startIndex);
                edge[1] = ToTwoDimY(startIndex);
            }

            return edge;
        }

        protected int[] SnapshotVertex(int index)
        {
            if (Selected(index))
            {
                int[] edge = new int[2];
                edge[0] = ToTwoDimX(index);
                edge[1] = ToTwoDimY(index);
                return edge;
            }
            return null;
        }

        protected virtual bool Selected(int index)
        {
            return false;
        }

        protected void InheritSnapshotListFrom(PathFindingAlgorithm algo)
        {
            this.snapshotList = algo.snapshotList;
        }
    }
}
