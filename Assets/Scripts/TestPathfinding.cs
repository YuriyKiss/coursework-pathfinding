using System.Collections.Generic;
using UnityEngine;
using AnyAnglePathfinding.Partition;
using AnyAnglePathfinding;
using SharpMath2;

public class TestPathfinding : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Entites")]
    [SerializeField] private Transform agentTransform;
    [SerializeField] private List<Transform> obstacleList;
    [SerializeField] private Transform goalTransform;

    private void Start()
    {
        RecalculatePath();
    }

    [ContextMenu("Recalculate")]
    private void RecalculatePath()
    {
        var map = new RectPartitionAAMap<AACollidable>(10, 10);

        var agent = new AACollidable()
        {
            Position = new Vector2(agentTransform.position.x, agentTransform.position.z),
            Bounds = ShapeUtils.CreateCircle(radius: agentTransform.localScale.x, segments: 16)
        };
        map.Register(agent);

        foreach (Transform obstacleTransform in obstacleList)
        {
            var obstacle = new AACollidable()
            {
                Position = new Vector2(obstacleTransform.position.x, obstacleTransform.position.z),
                Bounds = ShapeUtils.CreateRectangle(width: obstacleTransform.localScale.x * 2, height: obstacleTransform.localScale.z * 2)
            };
            map.Register(obstacle);
        }

        var excludeIds = new HashSet<int>();
        excludeIds.Add(agent.ID);

        List<Vector2> path = new AAPathfinder<AACollidable>
            (map, agent.Bounds, agent.Position,
            new Vector2(goalTransform.position.x, goalTransform.position.z), excludeIds, 0).
                CalculatePath();

        if (path == null)
        {
            print("Path not found");
            return;
        }

        List<Vector3> positions = new List<Vector3>();
        if (path.Count == 1)
        {
            positions.Add(agentTransform.position);
            positions.Add(goalTransform.position);
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

        print($"entity 1 at {agent.Position}");
        foreach (var loc in path)
        {
            print($"moved entity 1 to {loc}");
        }
    }
}
