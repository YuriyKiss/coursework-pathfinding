using UnityEngine;
using ThetaStar.Grid;
using ThetaStar.Grid.Generator;
using ThetaStar.Algorithms;
using System.Collections.Generic;

namespace ThetaStar.Navigation 
{ 
    public class NavigationRunner : MonoBehaviour
    {
        [SerializeField] private GridGenerator gridGenerator;
        [SerializeField] private LineRenderer lineRenderer;

        private UnityGrid _grid;

        private void Awake()
        {
            _grid = gridGenerator.Grid;
        }

        [ContextMenu("Compute Path")]
        public void ComputePath()
        {
            // Preparing graph
            GridGraph graph = new GridGraph(_grid.TilesInCol, _grid.TilesInRow);
            List<Tile> tiles = _grid.GetTiles();
            foreach (Tile tile in tiles)
            {
                graph.SetBlocked(tile.PosZ, tile.PosX, tile.IsBlocked);
            }

            // Preparing algorithm
            PathFindingAlgorithm algorithm = new AStarStaticMemory(graph, 0, 0, 19, 19);
            algorithm.ComputePath();
            int[][] path = algorithm.GetPath();

            List<Vector3> points = new List<Vector3>();

            Tile firstTile = tiles[_grid.TilesInCol * path[0][1] + path[0][0]];
            points.Add(firstTile.Position + Vector3.up * 0.3f);

            for (int i = 0; i < path.Length - 1; i++)
            {
                print($"{path[i][0]}, {path[i][1]}, {path[i + 1][0]}, {path[i + 1][1]}");
                Tile nextTile = tiles[_grid.TilesInCol * path[i + 1][1] + path[i + 1][0]];
                points.Add(nextTile.Position + Vector3.up * 0.3f);
            }

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }
}
