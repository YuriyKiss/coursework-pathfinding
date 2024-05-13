using System;
using UnityEngine;

namespace ThetaStar.Grid
{
    [Serializable]
    public struct Tile
    {
        public Vector3 Position;
        public bool IsBlocked;
        public int RowIdx;
        public int ColIdx;

        public Tile(Vector3 position, bool isBlocked, int row, int col)
        {
            Position = position;
            IsBlocked = isBlocked;
            RowIdx = row;
            ColIdx = col;
        }
    }
}
