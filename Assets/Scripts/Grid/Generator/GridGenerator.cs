using System;
using UnityEngine;
using ThetaStar.Grid.Generator.Enum;
using Random = UnityEngine.Random;

namespace ThetaStar.Grid.Generator
{
    public class GridGenerator : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private UnityGrid grid;
        [Header("Settings")]
        // Theta Star algorithms work with uniform tiles only
        [SerializeField] private float tileSize = 0.4f;
        [SerializeField] private bool addRandomWeights = true;
        [SerializeField] private PhysicsMode physicsMode = PhysicsMode.Physics3D;
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

                    bool isHit = false; string colliderTag = null; Vector3 hitPoint = Vector3.zero;
                    if (physicsMode == PhysicsMode.Physics3D)
                    {
                        isHit = Physics.Raycast(origin, generationRayDirection, out RaycastHit hitInfo,
                            MAX_DISTANCE, raycastLayerMask, TRIGGER_INTERACTION);

                        colliderTag = hitInfo.collider.tag;
                        hitPoint = hitInfo.point;
                    }
                    else if (physicsMode == PhysicsMode.Physics2D)
                    {
                        RaycastHit2D hitInfo = Physics2D.Raycast(origin, generationRayDirection, MAX_DISTANCE, raycastLayerMask);

                        colliderTag = hitInfo.collider.tag;
                        hitPoint = hitInfo.point;
                    }

                    Tile currentTile;
                    if (!isHit)
                    {
                        Vector3 tilePosition = new Vector3(tilePos.x, Mathf.Infinity, tilePos.z);
                        currentTile = new Tile(tilePosition, Vector3.zero, -1, row, col);
                    }
                    else if (colliderTag != walkableObjectTag)
                    {
                        Vector3 tilePosition = new Vector3(tilePos.x, hitPoint.y, tilePos.z);
                        currentTile = new Tile(tilePosition, Vector3.zero, -1, row, col);
                    }
                    else
                    {
                        Vector3 tilePosition = new Vector3(tilePos.x, hitPoint.y, tilePos.z);
                        currentTile = new Tile(tilePosition, Vector3.zero, GenerateRandomWeight(), row, col);
                    }

                    grid.AddTile(currentTile);
                }
            }

            grid.CleanupTiles();
        }

        private float GenerateRandomWeight() 
        {
            float weight = 1f;

            if (addRandomWeights && Random.Range(1, 3) == 1) {
                double value = (double)(new decimal(Random.Range(0.5f, 2f)));
                weight += (float)Math.Round(value, 2);
            }

            return weight;
        }

        private void OnDrawGizmosSelected()
        {
            if (!displayGenerationZone) return;

            Gizmos.color = generationZoneColor;

            Gizmos.DrawWireCube(transform.localPosition, transform.localScale);
        }
    }
}
