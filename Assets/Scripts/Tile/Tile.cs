using System;
using UnityEngine;

namespace ThetaStar.Grid
{
    [Serializable]
    public struct Tile
    {
        public Vector3 Position;
        public Vector3 PositionTopLeftCorner;
        public bool IsBlocked;
        public int RowIdx;
        public int ColIdx;

        public Tile(Vector3 position, Vector3 positionTopLeftCorner, bool isBlocked, int row, int col)
        {
            Position = position;
            PositionTopLeftCorner = positionTopLeftCorner;
            IsBlocked = isBlocked;
            RowIdx = row;
            ColIdx = col;
        }
    }
}
