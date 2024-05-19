using UnityEngine;

namespace ThetaStar.Grid.Generator
{
    public class GridGenerator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private UnityGrid grid;
        [Header("Settings")]
        // Theta Star algorithms work with uniform tiles only
        [SerializeField] private float tileSize = 0.4f;
        [SerializeField, ReadOnly] private Vector3 generationRayDirection = Vector3.down;
        [SerializeField, ReadOnly] private string walkableObjectTag = "Walkable";
        [SerializeField, ReadOnly] private LayerMask raycastLayerMask = 1 << 0;
        [Header("Debug Settings")]
        [SerializeField] private bool displayGenerationZone;
        [SerializeField] private Color generationZoneColor = Color.black;

        private const float MAX_DISTANCE = Mathf.Infinity;
        private readonly QueryTriggerInteraction TRIGGER_INTERACTION = QueryTriggerInteraction.Ignore;

        public void RegenerateGrid()
        {
            if (grid == null) Debug.LogError("UnityGrid component is not set.");

            float posY = transform.localPosition.y;
            float sizeX = transform.localScale.x; float sizeY = transform.localScale.y; float sizeZ = transform.localScale.z;

            int tilesInRow = (int)Mathf.Floor(sizeZ / tileSize);
            int tilesInCol = (int)Mathf.Floor(sizeX / tileSize);

            // TODO: compute spacing
            // float leftoverRowLength = sizeZ % tileSize;
            // float leftoverColLength = sizeX % tileSize;

            grid.Clear();
            grid.SetTilesInRowAndCol(tilesInRow, tilesInCol);
            grid.SetTileSize(tileSize);
            grid.SetGridPositionAndScale(transform.position, transform.localScale);

            for (int row = 0; row < tilesInCol; row++)
            {
                for (int col = 0; col < tilesInRow; col++)
                {
                    Vector3 tilePos = grid.GetTilePosition(row, col);
                    Vector3 origin = new Vector3(tilePos.x, posY + sizeY / 2, tilePos.z);

                    bool isHit = Physics.Raycast(origin, generationRayDirection, out RaycastHit hitInfo, 
                        MAX_DISTANCE, raycastLayerMask, TRIGGER_INTERACTION);

                    Tile currentTile;
                    if (!isHit)
                    {
                        Vector3 tilePosition = new Vector3(tilePos.x, Mathf.Infinity, tilePos.z);
                        currentTile = new Tile(tilePosition, Vector3.zero, true, row, col);
                    }
                    else if (!hitInfo.collider.CompareTag(walkableObjectTag))
                    {
                        Vector3 tilePosition = new Vector3(tilePos.x, hitInfo.point.y, tilePos.z);
                        currentTile = new Tile(tilePosition, Vector3.zero, true, row, col);
                    }
                    else
                    {
                        Vector3 tilePosition = new Vector3(tilePos.x, hitInfo.point.y, tilePos.z);
                        currentTile = new Tile(tilePosition, Vector3.zero, false, row, col);
                    }

                    grid.AddTile(currentTile);
                }
            }

            grid.CleanupTiles();
        }

        private void OnDrawGizmosSelected()
        {
            if (!displayGenerationZone) return;

            Gizmos.color = generationZoneColor;

            Gizmos.DrawWireCube(transform.localPosition, transform.localScale);
        }
    }
}
