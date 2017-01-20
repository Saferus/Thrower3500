using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private Transform trajectory;

    [Header("Physics")] [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 steering;

    [Header("Movemental")] [SerializeField] private float changeDistance;
    [SerializeField] private RoadPoint waypoint;

    private List<Transform> waypoints;

    private Vector3 directionToPoint;

    // Use this for initialization
    void Start()
    {
        var transformChildCount = trajectory.transform.childCount;

        waypoints = new List<Transform>(transformChildCount);

        for (int i = 0; i < transformChildCount; i++)
        {
            var child = trajectory.GetChild(i);
            waypoints.Add(child);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
//        DebugWaypoint();
        Accelerate();

        float distance = Vector3.Distance(waypoint.transform.position, transform.position);

        if (distance < changeDistance)
        {
            NextWayPoint();
        }

        transform.position += velocity;
    }

    private void DebugWaypoint()
    {
        Debug.DrawLine(transform.position, waypoint.transform.position, Color.gray);
    }

    private void Accelerate()
    {
        Vector3 diseredVelocity = GetDirectionToPoint(waypoint.transform.position).normalized * maxSpeed;
//        Debug.DrawLine(transform.position, transform.position + diseredVelocity, Color.blue);

        steering = diseredVelocity - velocity;
        steering = steering.normalized*maxForce;
//        Debug.DrawLine(transform.position, transform.position + steering, Color.green);

        velocity += steering;
//        Debug.DrawLine(transform.position, transform.position + velocity, Color.red);

        velocity.y = 0;
        transform.position += velocity;

        transform.LookAt(transform.position + velocity);
    }

    private Vector3 GetDirectionToPoint(Vector3 target)
    {
        return target - transform.position;
    }

    private void NextWayPoint()
    {
        waypoint = waypoint.nextPoint[0];
    }
}