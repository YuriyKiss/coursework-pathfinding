using System;
using System.Collections.Generic;
using UnityEngine;
using ThetaStar.Grid;
using ThetaStar.Algorithms;
using ThetaStar.Grid.Generator;

namespace ThetaStar.Navigation 
{ 
    public class NavigationRunner : MonoBehaviour
    {
        [Serializable]
        private struct StartEnd
        {
            public GridTarget Start;
            public GridTarget End;
        }

        [Serializable]
        private struct GridTarget
        {
            public int Row;
            public int Col;
        }

        [SerializeField] private UnityGrid grid;
        [SerializeField] private GridGenerator generator;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private StartEnd goals;
        [Header("Debug Options")]
        [SerializeField] private bool displayPathString = false;

        public void ComputePath()
        {
            PathFindingAlgorithm.ClearStaticData();

            List<Tile> tiles = grid.GetTiles();

            GridGraph graph = GenerateInternalGraph(tiles);

            PathFindingAlgorithm algorithm = PrepareAlgorithm(graph);

            int[][] path = algorithm.GetPath();
            List<GridTarget> pathConverted = ConvertPath(path);
            DisplayPath(pathConverted);
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
                                                                   goals.Start.Col, goals.Start.Row,
                                                                   goals.End.Col, goals.End.Row);
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

        private void DisplayPath(List<GridTarget> path)
        {
            List<Vector3> points = new List<Vector3>();

            string debug = "";
            for (int i = 0; i < path.Count; i++)
            {
                int row = path[i].Row;
                int col = path[i].Col;

                Vector3 tilePosition = generator.GetTilePosition(row, col);
                points.Add(grid.TileTopLeftPosition(tilePosition));

                debug += $"[{row}; {col}] ->";
            }

            if (displayPathString)
            {
                debug = debug.Substring(0, debug.Length - 3);
                print(debug);
            }

            lineRenderer.positionCount = points.Count;
            lineRenderer.SetPositions(points.ToArray());
        }
    }
}
