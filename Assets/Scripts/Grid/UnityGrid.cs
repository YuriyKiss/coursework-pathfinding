using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using ThetaStar.Grid.Data;

namespace ThetaStar.Grid
{
    public class UnityGrid : MonoBehaviour
    {
        [Header("Grid Data")]
        [SerializeField] private List<Tile> tiles = new List<Tile>();
        [SerializeField] private GridSettings settings = new GridSettings();
        [Header("Debug Settings")]
        [SerializeField] private bool displayNodes = false;
        [SerializeField] private float nodeRadius = 0.05f;
        [SerializeField] private Color nodeColor = new Color(0, 0.2f, 1, 0.5f);
        [Space, SerializeField] private bool displayTiles = false;
        [SerializeField] private Color walkableTileColor = Color.green;
        [SerializeField] private Color blockedTileColor = Color.red;

        private const float TILE_HEIGHT = 0.05f;
        private const float TILE_SIZE_MODIFIER = 0.8f;

        public List<Tile> GetTiles() => tiles;
        public int TilesInRow => settings.TilesInRow;
        public int TilesInCol => settings.TilesInCol;
        public float TileSize => settings.TileSize;
        public float MinYHeight => settings.MinYHeight;

        #region Initialization

        public void Clear()
        {
            tiles.Clear();
            settings.Clear();
        }

        public void SetTilesInRowAndCol(int rowAmount, int colAmount)
        {
            settings.TilesInRow = rowAmount;
            settings.TilesInCol = colAmount;
        }

        public void SetTileSize(float size)
        {
            settings.TileSize = size;
        }

        public void SetGridPositionAndScale(Vector3 position, Vector3 scale)
        {
            transform.position = position;
            transform.localScale = scale;
        }

        #endregion

        public Vector3 GetTilePosition(int row, int col)
        {
            float posX = transform.localPosition.x; float posZ = transform.localPosition.z;
            float sizeX = transform.localScale.x; float sizeZ = transform.localScale.z;

            float tilePosX = posX - sizeX / 2 + TileSize * (0.5f + row);
            float tilePosZ = posZ - sizeZ / 2 + TileSize * (0.5f + col);

            return new Vector3(tilePosX, MinYHeight, tilePosZ);
        }

        public void AddTile(Tile tile)
        {
            if (tiles.Count >= TilesInRow * TilesInCol)
            {
                Debug.LogError("[GridGenerator] Too many tiles spawned. Stopping GridGenerator");
                return;
            }

            tiles.Add(tile);
        }

        public void CleanupTiles()
        {
            settings.MinYHeight = tiles.Min(tile => tile.Position.y);

            for (int i = 0; i < tiles.Count; i++)
            {
                Vector3 currentPosition = tiles[i].Position;
                Vector3 newPosition = new Vector3(currentPosition.x, MinYHeight, currentPosition.z);
                Vector3 topLeftCornerPosition = TileTopLeftPosition(newPosition);
                tiles[i] = new Tile(newPosition, topLeftCornerPosition, tiles[i].IsBlocked, tiles[i].RowIdx, tiles[i].ColIdx);
            }
        }

        public Vector3 TileTopLeftPosition(Vector3 tileCenterPosition)
        {
            return tileCenterPosition +
                   Vector3.up * 0.05f +
                   new Vector3(-1f, 0f, -1f) * TileSize / 2f;
        }

        public (int, int) FindClosestTile(Vector3 position)
        {
            int c = 0, r = 0;
            float minDistance = Mathf.Infinity;

            for (int row = 0; row <= TilesInCol; row++)
            {
                for (int col = 0; col <= TilesInRow; col++)
                {
                    Vector3 tilePosition = GetTilePosition(row, col);
                    Vector3 tileTopLeftPosition = TileTopLeftPosition(tilePosition);

                    float distance = Vector3.Distance(new Vector3(position.x, tileTopLeftPosition.y, position.z), tileTopLeftPosition);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        c = col;
                        r = row;
                    }
                }
            }

            return (r, c);
        }

        #region Visualization

        private void OnDrawGizmosSelected()
        {
            if (tiles == null || tiles.Count == 0) return;

            if (displayTiles)
            {
                DisplayTiles();
            }

            if (displayNodes)
            {
                DisplayNodes();
            }
        }

        private void DisplayTiles()
        {
            foreach (Tile tile in tiles)
            {
                Gizmos.color = tile.IsBlocked ? blockedTileColor : walkableTileColor;

                Vector3 tileScale = new Vector3(TileSize * TILE_SIZE_MODIFIER, TILE_HEIGHT, TileSize * TILE_SIZE_MODIFIER);

                Gizmos.DrawWireCube(tile.Position, tileScale);
            }
        }

        private void DisplayNodes()
        {
            for (int row = 0; row <= TilesInCol; row++)
            {
                for (int col = 0; col <= TilesInRow; col++)
                {
                    Gizmos.color = nodeColor;

                    Vector3 tilePosition = GetTilePosition(row, col);
                    Vector3 tileTopLeftPosition = TileTopLeftPosition(tilePosition);

                    Gizmos.DrawSphere(tileTopLeftPosition, nodeRadius);
                }
            }
        }

        #endregion
    }
}
