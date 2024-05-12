using System.Collections.Generic;
using UnityEngine;

namespace ThetaStar.Grid.Generator
{
    public class GridGenerator : MonoBehaviour
    {
        // Theta Star algorithm works with uniform tiles only
        [SerializeField] private float tileSize = 0.5f;
        [SerializeField] private UnityGrid grid;

        private readonly Vector3 RAY_DIRECTION = Vector3.down;
        private const float MAX_DISTANCE = Mathf.Infinity;
        private const int LAYER_MASK = 1 << 0;
        private readonly QueryTriggerInteraction TRIGGER_INTERACTION = QueryTriggerInteraction.Ignore;
        private readonly string WALKABLE_TAG = "Walkable";

        public UnityGrid Grid => grid;

        public void RegenerateGrid()
        {
            float posX = transform.localPosition.x;
            float posY = transform.localPosition.y;
            float posZ = transform.localPosition.z;

            float sizeX = transform.localScale.x;
            float sizeY = transform.localScale.y;
            float sizeZ = transform.localScale.z;

            int tilesInRow = (int)Mathf.Floor(sizeZ / tileSize);
            float leftoverRowLength = sizeZ % tileSize;

            int tilesInCol = (int)Mathf.Floor(sizeX / tileSize);
            float leftoverColLength = sizeX % tileSize;

            grid.Clear();
            grid.SetTilesInRowAndCol(tilesInRow, tilesInCol);

            for (int i = 0; i < tilesInRow; i++)
            {
                for (int j = 0; j < tilesInCol; j++)
                {
                    float tilePosX = posX - sizeX / 2 + tileSize * (0.5f + j);
                    float tilePosZ = posZ - sizeZ / 2 + tileSize * (0.5f + i);

                    Vector3 origin = new Vector3(tilePosX, posY + sizeY / 2, tilePosZ);

                    bool isHit = Physics.Raycast(origin, RAY_DIRECTION, out RaycastHit hitInfo, 
                        MAX_DISTANCE, LAYER_MASK, TRIGGER_INTERACTION);

                    if (!isHit)
                    {
                        Vector3 position = new Vector3(tilePosX, Mathf.Infinity, tilePosZ);
                        Tile currentTile = new Tile(position, false, i, j);
                        grid.AddTile(currentTile);
                    }
                    else if (!hitInfo.collider.CompareTag(WALKABLE_TAG))
                    {
                        Vector3 position = new Vector3(tilePosX, hitInfo.point.y, tilePosZ);
                        Tile currentTile = new Tile(position, false, i, j);
                        grid.AddTile(currentTile);
                    }
                    else
                    {
                        Vector3 position = new Vector3(tilePosX, hitInfo.point.y, tilePosZ);
                        Tile currentTile = new Tile(position, true, i, j);
                        grid.AddTile(currentTile);
                    }
                }
            }

            grid.CleanupNotFoundTiles();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;

            Vector3 position = transform.localPosition;
            Vector3 scale = new Vector3(transform.localScale.x, 1f, transform.localScale.z);

            Gizmos.DrawWireCube(position, scale);

            if (grid.TilesAmount == 0)
            {
                return;
            }

            List<Tile> tiles = grid.GetTiles();

            foreach (Tile tile in tiles)
            {
                Gizmos.color = tile.IsWalkable ? Color.green : Color.red;

                Vector3 tileScale = new Vector3(tileSize * 0.8f, 0.05f, tileSize * 0.8f);

                Gizmos.DrawWireCube(tile.Position, tileScale);
            }
        }
    }
}
