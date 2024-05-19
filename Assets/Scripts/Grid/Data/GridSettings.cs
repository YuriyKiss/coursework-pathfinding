using System;
using UnityEngine;

namespace ThetaStar.Grid.Data
{
    [Serializable]
    public struct GridSettings
    {
        public int TilesInRow;
        public int TilesInCol;
        public float TileSize;
        public float MinYHeight;

        public void Clear()
        {
            TilesInRow = 0;
            TilesInCol = 0;
            TileSize = 0;
            MinYHeight = Mathf.Infinity;
        }
    }
}
