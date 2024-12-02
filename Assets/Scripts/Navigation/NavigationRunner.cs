using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using ThetaStar.Grid;
using ThetaStar.Grid.Data;
using ThetaStar.Navigation.Data;
using ThetaStar.Pathfinding.Grid;
using ThetaStar.Pathfinding.Algorithms;
using ThetaStar.Pathfinding.Algorithms.Enums;
using ThetaStar.Pathfinding.Algorithms.Theta;
using System;
using Random = UnityEngine.Random;

namespace ThetaStar.Navigation
{
    public class NavigationRunner : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private UnityGrid grid;

        [Header("Settings")]
        [SerializeField] private AlgorithmType algorithmType;
        [SerializeField] private StartEnd goals;

        [Header("Debug Options")]
        [SerializeField] private bool drawPath = false;
        [SerializeField] private LineRenderer lineRenderer;
        [Space, SerializeField] private bool displayPathString = false;
        [SerializeField] private bool printAlgorithmRuntime = false;
        [SerializeField] private bool printPathLength = false;
        [SerializeField] private int multipleTestsAmount = 100;

        public List<GridTarget> ComputePath(GridTarget start, GridTarget end)
        {
            goals.Start = start;
            goals.End = end;

            return ComputePath();
        }

        public void TestMultipleTimes()
        {
            lineRenderer.positionCount = 0;

            List<Tile> tiles = grid.GetTiles();

            GridGraph graph = GenerateInternalGraph(tiles);

            long counter = 0;
            float distance = 0;
            var rnd = new System.Random(2222);
            PathFindingAlgorithm algorithm = null; TimeSpan timeSpan = TimeSpan.Zero;
            for (int i = 0; i < multipleTestsAmount; ++i)
            {
                (algorithm, timeSpan) = PrepareAlgorithm(graph, GetRandomGoals(rnd), true);

                if (algorithm.GetPathLength() == 0)
                {
                    i--;
                    continue;
                }
                
                distance += algorithm.GetPathLength();
                counter += timeSpan.Ticks;
            }

            counter /= multipleTestsAmount;
            distance /= multipleTestsAmount;

            print($"{algorithmType.ToString()} computation time: " + counter / 10000f);
            print("Path length: " + distance * grid.TileSize);

            int[][] path = algorithm.GetPath();
            List<GridTarget> pathConverted = ConvertPath(path);
            DisplayPath(pathConverted);
        }

        public List<GridTarget> ComputePath()
        {
            lineRenderer.positionCount = 0;

            List<Tile> tiles = grid.GetTiles();

            GridGraph graph = GenerateInternalGraph(tiles);

            (PathFindingAlgorithm algorithm, TimeSpan time) = PrepareAlgorithm(graph, goals,  printAlgorithmRuntime);

            if (printAlgorithmRuntime) { print($"{algorithmType.ToString()} computation time: " + time); }

            int[][] path = algorithm.GetPath();

            if (printPathLength) print("Path length: " + algorithm.GetPathLength() * grid.TileSize);

            List<GridTarget> pathConverted = ConvertPath(path);
            DisplayPath(pathConverted);

            return pathConverted;
        }

        private GridGraph GenerateInternalGraph(List<Tile> tiles)
        {
            GridGraph graph = new GridGraph(grid.TilesInRow, grid.TilesInCol);

            foreach (Tile tile in tiles)
            {
                graph.SetBlocked(tile.ColIdx, tile.RowIdx, tile.Weight);
            }

            return graph;
        }

        private (PathFindingAlgorithm, TimeSpan) PrepareAlgorithm(GridGraph graph, StartEnd goals, bool returnTimeSpan)
        {
            PathFindingAlgorithm algorithm = null;

            switch (algorithmType)
            {
                case AlgorithmType.WeightedAStar:
                    algorithm = new WeightedAStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.WeightedAStarPostSmooth:
                    algorithm = WeightedAStar.PostSmooth(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.WeightedThetaStar:
                    algorithm = new WeightedBasicThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.WeightedLazyThetaStar:
                    algorithm = new WeightedLazyThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.WeightedStrictThetaStar:
                    algorithm = new WeightedStrictThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.WeightedStrictThetaStarRecursive:
                    algorithm = new WeightedRecursiveStrictThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.AStar:
                    algorithm = new AStarStaticMemory(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.AStarPostSmooth:
                    algorithm = AStarStaticMemory.PostSmooth(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.ThetaStar:
                    algorithm = new BasicThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.LazyThetaStar:
                    algorithm = new LazyThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.StrictThetaStar:
                    algorithm = new StrictThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
                case AlgorithmType.StrictThetaStarRecursive:
                    algorithm = new RecursiveStrictThetaStar(graph, goals.Start.Col, goals.Start.Row, goals.End.Col, goals.End.Row);
                    break;
            }

            if (returnTimeSpan)
            {
                Stopwatch watch = Stopwatch.StartNew();

                algorithm.ComputePath();

                watch.Stop();

                return (algorithm, watch.Elapsed);
            }
            else
            {
                algorithm.ComputePath();

                return (algorithm, new TimeSpan(0));
            }
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

            string debug = "Path: ";
            for (int i = 0; i < path.Count; i++)
            {
                int row = path[i].Row; int col = path[i].Col;

                Vector3 tilePosition = grid.GetTilePosition(row, col);
                points.Add(grid.TileTopLeftPosition(tilePosition));

                if (displayPathString) debug += $"[{row}; {col}] ->";
            }

            if (displayPathString)
            {
                debug = debug.Substring(0, debug.Length - 3);
                print(debug);
            }

            if (drawPath)
            {
                lineRenderer.positionCount = points.Count;
                lineRenderer.SetPositions(points.ToArray());
            }
        }

        private StartEnd GetRandomGoals(System.Random rnd)
        {
            return new StartEnd()
            {
                Start = new GridTarget()
                {
                    Row = rnd.Next(0, grid.TilesInCol),
                    Col = rnd.Next(0, grid.TilesInRow)
                },
                End = new GridTarget()
                {
                    Row = rnd.Next(0, grid.TilesInCol),
                    Col = rnd.Next(0, grid.TilesInRow)
                }
            };
        }
    }
}
