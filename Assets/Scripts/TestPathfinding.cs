using System.Collections.Generic;
using UnityEngine;
using AnyAnglePathfinding.Partition;
using AnyAnglePathfinding;
using SharpMath2;

public class TestPathfinding : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LineRenderer lineRendererPrefab;

    [Header("Entites")]
    [SerializeField] private Transform agentTransform;
    [SerializeField] private List<Transform> obstacleList;
    [SerializeField] private Transform goalTransform;

    private void Start()
    {
        ContextRecalculate();
    }

    [ContextMenu("Recalculate")]
    private void ContextRecalculate()
    {
        RecalculatePath(goalTransform.position);
    }

    private void RecalculatePath(Vector3 goal)
    {
        var map = new RectPartitionAAMap<AACollidable>(10, 10);

        var agent = new AACollidable()
        {
            Position = new Vector2(agentTransform.position.x, 
                                   agentTransform.position.z),
            Bounds = ShapeUtils.CreateCircle(radius: agentTransform.localScale.x, segments: 8)
        };
        map.Register(agent);

        /*
        LineRenderer rendererAgent = Instantiate(lineRendererPrefab);
        rendererAgent.positionCount = agent.Bounds.Vertices.Length;

        List<Vector3> agentVertices = new List<Vector3>();
        foreach (Vector3 v in agent.Bounds.Vertices)
        {
            agentVertices.Add(new Vector3(v.x + agent.Position.x, 0.15f, v.y + agent.Position.y));
        }
        rendererAgent.SetPositions(agentVertices.ToArray());
        */

        foreach (Transform obstacleTransform in obstacleList)
        {
            var obstacle = new AACollidable()
            {
                Position = new Vector2(obstacleTransform.position.x - obstacleTransform.localScale.x / 2, 
                                       obstacleTransform.position.z - obstacleTransform.localScale.z / 2),
                Bounds = ShapeUtils.CreateRectangle(width: obstacleTransform.localScale.x, 
                                                    height: obstacleTransform.localScale.z)
            };
            /*
            LineRenderer renderer = Instantiate(lineRendererPrefab);
            renderer.positionCount = obstacle.Bounds.Vertices.Length;
            
            List<Vector3> obstacleVertices = new List<Vector3>();
            foreach (Vector3 v in obstacle.Bounds.Vertices)
            {   
                obstacleVertices.Add(new Vector3(v.x + obstacle.Position.x, 0.15f, v.y + obstacle.Position.y));
            }
            renderer.SetPositions(obstacleVertices.ToArray());
            */
            map.Register(obstacle);
        }

        var excludeIds = new HashSet<int>();
        excludeIds.Add(agent.ID);

        List<Vector2> path = new AAPathfinder<AACollidable>
            (map, agent.Bounds, agent.Position,
            new Vector2(goal.x, goal.z), excludeIds, 0).
                CalculatePath();

        if (path == null)
        {
            print("Path not found");
            lineRenderer.positionCount = 0;
            return;
        }

        List<Vector3> positions = new List<Vector3>();
        if (path.Count == 1)
        {
            positions.Add(agentTransform.position);
            positions.Add(goal);
        }
        else
        {
            foreach (Vector2 vector in path)
            {
                positions.Add(new Vector3(vector.x, 0.075f, vector.y));
            }
        }

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());

        //print($"entity 1 at {agent.Position}");
        //foreach (var loc in path)
        //{
        //    print($"moved entity 1 to {loc}");
        //}
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray.origin, ray.direction, out RaycastHit m_HitInfo))
            {
                RecalculatePath(m_HitInfo.point);
            }
        }

    }
}
