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
            PathFindingAlgorithm algorithm = new AStarStaticMemory(graph, 5, 5, 15, 15);
            algorithm.ComputePath();
            int[][] path = algorithm.GetPath();
            float pathLength = algorithm.GetPathLength();

            // Output
            string pathString = "";
            for (int i = 0; i < path.Length; ++i)
            {
                pathString += "[";
                for (int j = 0; j < path[i].Length; ++j)
                {
                    pathString += path[i][j].ToString() + " ";
                }
                pathString += "]";
            }

            print(pathString);
            print($"Length: {pathLength}");
        }
    }
}
