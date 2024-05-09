using System.Collections.Generic;
using UnityEngine;
using AnyAnglePathfinding.Partition;
using AnyAnglePathfinding;
using SharpMath2;

public class TestPathfinding : MonoBehaviour
{
    private void Start()
    {
        var map = new RectPartitionAAMap<AACollidable>(200, 100);

        var entity = new AACollidable()
        {
            Position = new Vector2(50, 70),
            Bounds = ShapeUtils.CreateCircle(radius: 10, segments: 7)
        };
        map.Register(entity);

        var entity2 = new AACollidable()
        {
            Position = new Vector2(100, 70),
            Bounds = ShapeUtils.CreateCircle(radius: 7, segments: 5)
        };
        map.Register(entity2);

        var excludeIds = new HashSet<int>();
        excludeIds.Add(entity.ID);

        List<Vector2> path = new AAPathfinder<AACollidable>(
                map, entity.Bounds, entity.Position,
                new Vector2(150, 70), excludeIds, 0
            ).CalculatePath();

        print($"entity 1 at {entity.Position}");
        foreach (var loc in path)
        {
            print($"moved entity 1 to {loc}");
        }
    }
}
