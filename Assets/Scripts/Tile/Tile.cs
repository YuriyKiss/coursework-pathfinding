using System;
using UnityEngine;

namespace ThetaStar.Grid
{
    [Serializable]
    public struct Tile
    {
        public Vector3 Position;
        public Vector3 PositionTopLeftCorner;
        public float Weight;
        public int RowIdx;
        public int ColIdx;

        public Tile(Vector3 position, Vector3 positionTopLeftCorner, float weight, int row, int col)
        {
            Position = position;
            PositionTopLeftCorner = positionTopLeftCorner;
            Weight = weight;
            RowIdx = row;
            ColIdx = col;
        }
    }
}
