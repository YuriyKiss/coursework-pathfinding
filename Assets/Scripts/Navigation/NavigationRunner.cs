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
                graph.SetBlocked(tile.PosX, tile.PosZ, tile.IsWalkable);
            }

            // Preparing algorithm
            PathFindingAlgorithm algorithm = new AStarStaticMemory(graph, 0, 9, 19, 9);
            algorithm.ComputePath();
            int[][] path = algorithm.GetPath();

            for (int i = 0; i < path.Length - 1; i++)
            {
                print($"{path[i][0]}, {path[i][1]}, {path[i + 1][0]}, {path[i + 1][1]}");
            }
        }
    }
}
