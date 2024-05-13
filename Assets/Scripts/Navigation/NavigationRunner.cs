using System;
using System.Collections.Generic;
using UnityEngine;
using ThetaStar.Grid;
using ThetaStar.Algorithms;

namespace ThetaStar.Navigation 
{ 
    public class NavigationRunner : MonoBehaviour
    {
        [Serializable]
        private struct GridTarget
        {
            public int Row;
            public int Col;
        }

        [SerializeField] private UnityGrid grid;
        [SerializeField] private LineRenderer lineRenderer;
        [Space]
        [SerializeField] private GridTarget startIndices;
        [SerializeField] private GridTarget endIndices;

        public void ComputePath()
        {
            PathFindingAlgorithm.ClearStaticData();

            List<Tile> tiles = grid.GetTiles();

            GridGraph graph = GenerateInternalGraph(tiles);

            PathFindingAlgorithm algorithm = PrepareAlgorithm(graph);

            int[][] path = algorithm.GetPath();
            List<GridTarget> pathConverted = ConvertPath(path);
            DisplayPath(tiles, pathConverted);
        }

        private GridGraph GenerateInternalGraph(List<Tile> tiles)
        {
            GridGraph graph = new GridGraph(grid.TilesInRow, grid.TilesInCol);

            foreach (Tile tile in tiles)
            {
                graph.SetBlocked(tile.ColIdx, tile.RowIdx, tile.IsBlocked);
            }

            return graph;
        }

        private PathFindingAlgorithm PrepareAlgorithm(GridGraph graph)
        {
            PathFindingAlgorithm algorithm = new AStarStaticMemory(graph,
                                                                   startIndices.Col, startIndices.Row,
                                                                   endIndices.Col, endIndices.Row);
            algorithm.ComputePath();

            return algorithm;
        }

        private List<GridTarget> ConvertPath(int[][] path)
        {
            List<GridTarget> points = new List<GridTarget>
            {
                new GridTarget() { Row = path[0][1], Col = path[0][0] }
            };

            for (int i = 0; i < path.Length - 1; i++)
            {
                points.Add(new GridTarget() { Row = path[i + 1][1], Col = path[i + 1][0] });
            }

            return points;
        }

        private void DisplayPath(List<Tile> tiles, List<GridTarget> path)
        {
            List<Vector3> points = new List<Vector3>();

            for (int i = 0; i < path.Count; i++)
            {
                int row = path[i].Row;
                int col = path[i].Col;
                print($"[{row}; {col}]");

                Tile currentTile = tiles[row * grid.TilesInRow + col];
                points.Add(currentTile.Position + Vector3.up * 0.2f + new Vector3(-1f, 0f, -1f) * grid.TileSize / 2f);
            }

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }
}
