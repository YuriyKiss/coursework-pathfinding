using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ThetaStar.Grid
{
    [Serializable]
    public class UnityGrid
    {
        [SerializeField] private List<Tile> tiles = new List<Tile>();
        [SerializeField] private int tilesInRow = 0;
        [SerializeField] private int tilesInCol = 0;

        public int TilesAmount => tiles.Count;
        public List<Tile> GetTiles() => tiles;
        public int TilesInRow => tilesInRow;
        public int TilesInCol => tilesInCol;

        public void Clear()
        {
            tiles.Clear();
            tilesInRow = 0;
            tilesInCol = 0;
        }

        public void SetTilesInRowAndCol(int rowAmount, int colAmount)
        {
            tilesInRow = rowAmount;
            tilesInCol = colAmount;
        }

        public void AddTile(Tile tile)
        {
            if (tiles.Count >= tilesInRow * tilesInCol) 
            {
                Debug.LogError("[GridGenerator] Too many tiles spawned. Stopping GridGenerator");
                return;
            }

            tiles.Add(tile);
        }

        public void CleanupNotFoundTiles()
        {
            float yMinHeight = tiles.Min(tile => tile.Position.y);

            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].Position.y == Mathf.Infinity)
                {
                    Vector3 newPosition = new Vector3(tiles[i].Position.x, yMinHeight, tiles[i].Position.z);

                    tiles[i] = new Tile(newPosition, tiles[i].IsWalkable, tiles[i].PosX, tiles[i].PosZ);
                }
            }
        }
    }
}
