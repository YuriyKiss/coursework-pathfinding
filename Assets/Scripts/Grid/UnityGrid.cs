using System.Collections.Generic;
using System.Linq;
using ThetaStar.Grid.Generator;
using UnityEngine;

namespace ThetaStar.Grid
{
    public class UnityGrid : MonoBehaviour
    {
        [SerializeField] private GridGenerator generator;
        [SerializeField] private List<Tile> tiles = new List<Tile>();
        [SerializeField] private int tilesInRow = 0;
        [SerializeField] private int tilesInCol = 0;
        [SerializeField] private float tileSize = 0.5f;
        [SerializeField] private float minYHeight = 0.05f;
        [Header("Display Settings")]
        [SerializeField] private bool displayNodes = false;
        [SerializeField] private bool displayTiles = false;

        private const float TILE_HEIGHT = 0.05f;
        private const float TILE_SIZE_MODIFIER = 0.8f;

        public List<Tile> GetTiles() => tiles;
        public int TilesInRow => tilesInRow;
        public int TilesInCol => tilesInCol;
        public float TileSize => tileSize;
        public float MinYHeight => minYHeight;

        public void Clear()
        {
            tiles.Clear();
            tilesInRow = 0;
            tilesInCol = 0;
            tileSize = 0.5f;
        }

        public void SetTilesInRowAndCol(int rowAmount, int colAmount)
        {
            tilesInRow = rowAmount;
            tilesInCol = colAmount;
        }

        public void SetTileSize(float size)
        {
            tileSize = size;
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
            minYHeight = tiles.Min(tile => tile.Position.y);

            for (int i = 0; i < tiles.Count; i++)
            {
                Vector3 currentPosition = tiles[i].Position;
                Vector3 newPosition = new Vector3(currentPosition.x, minYHeight, currentPosition.z);
                tiles[i] = new Tile(newPosition, tiles[i].IsBlocked, tiles[i].RowIdx, tiles[i].ColIdx);
            }
        }

        public Vector3 TileTopLeftPosition(Vector3 tileCenterPosition)
        {
            return tileCenterPosition + Vector3.up * 0.2f + new Vector3(-1f, 0f, -1f) * TileSize / 2f;
        }

        private void OnDrawGizmosSelected()
        {
            if (tiles == null || tiles.Count == 0) return;

            if (displayTiles)
            {
                foreach (Tile tile in tiles)
                {
                    Gizmos.color = tile.IsBlocked ? Color.red : Color.green;

                    Vector3 tileScale = new Vector3(tileSize * TILE_SIZE_MODIFIER, TILE_HEIGHT, tileSize * TILE_SIZE_MODIFIER);

                    Gizmos.DrawWireCube(tile.Position, tileScale);
                }
            }

            if (displayNodes)
            {
                for (int row = 0; row <= tilesInCol; row++)
                {
                    for (int col = 0; col <= tilesInRow; col++)
                    {
                        Gizmos.color = new Color(0, 0.2f, 1, 0.5f);

                        Gizmos.DrawSphere(TileTopLeftPosition(generator.GetTilePosition(row, col)), 0.05f);
                    }
                }
            }
        }
    }
}
