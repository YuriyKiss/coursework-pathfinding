using UnityEngine;

namespace ThetaStar.Grid.Generator
{
    public class GridGenerator : MonoBehaviour
    {
        // Theta Star algorithms work with uniform tiles only
        [SerializeField] private float tileSize = 0.5f;
        [SerializeField] private UnityGrid grid;

        private readonly Vector3 RAY_DIRECTION = Vector3.down;
        private const float MAX_DISTANCE = Mathf.Infinity;
        private const int LAYER_MASK = 1 << 0;
        private readonly QueryTriggerInteraction TRIGGER_INTERACTION = QueryTriggerInteraction.Ignore;
        private readonly string WALKABLE_TAG = "Walkable";

        public void RegenerateGrid()
        {
            float posX = transform.localPosition.x; float posY = transform.localPosition.y; float posZ = transform.localPosition.z;
            float sizeX = transform.localScale.x;   float sizeY = transform.localScale.y;   float sizeZ = transform.localScale.z;

            int tilesInRow = (int)Mathf.Floor(sizeZ / tileSize);
            int tilesInCol = (int)Mathf.Floor(sizeX / tileSize);

            // TODO: compute spacing
            // float leftoverRowLength = sizeZ % tileSize;
            // float leftoverColLength = sizeX % tileSize;

            grid.Clear();
            grid.SetTilesInRowAndCol(tilesInRow, tilesInCol);
            grid.SetTileSize(tileSize);

            for (int row = 0; row < tilesInCol; row++)
            {
                for (int col = 0; col < tilesInRow; col++)
                {
                    float tilePosX = posX - sizeX / 2 + tileSize * (0.5f + row);
                    float tilePosZ = posZ - sizeZ / 2 + tileSize * (0.5f + col);

                    Vector3 origin = new Vector3(tilePosX, posY + sizeY, tilePosZ);

                    bool isHit = Physics.Raycast(origin, RAY_DIRECTION, out RaycastHit hitInfo, 
                        MAX_DISTANCE, LAYER_MASK, TRIGGER_INTERACTION);

                    if (!isHit)
                    {
                        Vector3 tilePosition = new Vector3(tilePosX, Mathf.Infinity, tilePosZ);
                        Tile currentTile = new Tile(tilePosition, true, row, col);
                        grid.AddTile(currentTile);
                    }
                    else if (!hitInfo.collider.CompareTag(WALKABLE_TAG))
                    {
                        Vector3 tilePosition = new Vector3(tilePosX, hitInfo.point.y, tilePosZ);
                        Tile currentTile = new Tile(tilePosition, true, row, col);
                        grid.AddTile(currentTile);
                    }
                    else
                    {
                        Vector3 tilePosition = new Vector3(tilePosX, hitInfo.point.y, tilePosZ);
                        Tile currentTile = new Tile(tilePosition, false, row, col);
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
        }
    }
}
