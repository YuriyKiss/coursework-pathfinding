using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ThetaStar.Grid
{
    public class UnityGrid : MonoBehaviour
    {
        [SerializeField] private List<Tile> tiles = new List<Tile>();
        [SerializeField] private int tilesInRow = 0;
        [SerializeField] private int tilesInCol = 0;
        [SerializeField] private float tileSize = 0.5f;

        private const float TILE_HEIGHT = 0.05f;
        private const float TILE_SIZE_MODIFIER = 0.8f;

        public List<Tile> GetTiles() => tiles;
        public int TilesInRow => tilesInRow;
        public int TilesInCol => tilesInCol;

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
            float yMinHeight = tiles.Min(tile => tile.Position.y);

            for (int i = 0; i < tiles.Count; i++)
            {
                Vector3 currentPosition = tiles[i].Position;

                if (currentPosition.y == Mathf.Infinity)
                {
                    Vector3 newPosition = new Vector3(currentPosition.x, yMinHeight, currentPosition.z);

                    tiles[i] = new Tile(newPosition, tiles[i].IsBlocked, tiles[i].RowIdx, tiles[i].ColIdx);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (tiles == null || tiles.Count == 0) return;

            foreach (Tile tile in tiles)
            {
                Gizmos.color = tile.IsBlocked ? Color.red : Color.green;

                Vector3 tileScale = new Vector3(tileSize * TILE_SIZE_MODIFIER, TILE_HEIGHT, tileSize * TILE_SIZE_MODIFIER);

                Gizmos.DrawWireCube(tile.Position, tileScale);
            }
        }
    }
}
