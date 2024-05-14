using System.Collections.Generic;
using UnityEngine;
using ThetaStar.Grid;
using ThetaStar.Grid.Data;
using ThetaStar.Grid.Generator;
using ThetaStar.Navigation.Data;
using ThetaStar.Pathfinding.Grid;
using ThetaStar.Pathfinding.Algorithms;
using ThetaStar.Pathfinding.Algorithms.Enums;
using ThetaStar.Pathfinding.Algorithms.Theta;

namespace ThetaStar.Navigation
{
    public class NavigationRunner : MonoBehaviour
    {
        [SerializeField] private AlgorithmType algorithmType;

        [Space, SerializeField] private UnityGrid grid;
        [SerializeField] private GridGenerator generator;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private StartEnd goals;
        [Header("Debug Options")]
        [SerializeField] private bool displayPathString = false;

        public void ComputePath()
        {
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
            PathFindingAlgorithm algorithm = null;

            switch (algorithmType)
            {
                case AlgorithmType.Dijkstra:
                    algorithm = AStarStaticMemory.Dijkstra(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.AStar:
                    algorithm = new AStarStaticMemory(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.AStarPostSmooth:
                    algorithm = AStarStaticMemory.PostSmooth(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.AStarRepeatedPostSmooth:
                    algorithm = AStarStaticMemory.RepeatedPostSmooth(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.ThetaStar:
                    algorithm = new BasicThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.ThetaStarNoHeuristic:
                    algorithm = BasicThetaStar.NoHeuristic(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.ThetaStarPostSmooth:
                    algorithm = BasicThetaStar.PostSmooth(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.ThetaStarRecursive:
                    algorithm = new RecursiveThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.LazyThetaStar:
                    algorithm = new LazyThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.StrictThetaStar:
                    algorithm = new StrictThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.StrictThetaStarNoHeuristic:
                    algorithm = StrictThetaStar.NoHeuristic(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.StrictThetaStarPostSmooth:
                    algorithm = StrictThetaStar.PostSmooth(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.StrictThetaStarCustomBuffer:
                    algorithm = StrictThetaStar.SetBuffer(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row, 0.6f);
                    break;
                case AlgorithmType.StrictThetaStarRecursive:
                    algorithm = new RecursiveStrictThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.StrictThetaStarRecursiveCustomBuffer:
                    algorithm = RecursiveStrictThetaStar.SetBuffer(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row, 0.6f);
                    break;
                case AlgorithmType.StrictThetaStarRecursiveNoHeuristic:
                    algorithm = RecursiveStrictThetaStar.NoHeuristic(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.StrictThetaStarRecursiveCustomDepth:
                    algorithm = RecursiveStrictThetaStar.DepthLimit(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row, 0);
                    break;
                case AlgorithmType.StrictThetaStarRecursivePostSmooth:
                    algorithm = RecursiveStrictThetaStar.PostSmooth(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
            }

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

                if (displayPathString) debug += $"[{row}; {col}] ->";
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
