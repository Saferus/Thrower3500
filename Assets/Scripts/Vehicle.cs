using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    [SerializeField] private Trajectory trajectory;

    [Header("Physics")] [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 steering;

    [Header("Movemental")]
    [SerializeField] private float changeDistance;
    [SerializeField] private float slowMovemntRaius;
    [SerializeField] private RoadPoint waypoint;

    private float distance;

    private List<RoadPoint> waypoints;

    private Vector3 directionToPoint;

    // Update is called once per frame
    void FixedUpdate()
    {
        distance = Vector3.Distance(waypoint.transform.position, transform.position);

        //        DebugWaypoint();
        Accelerate();

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
        var toPoint = GetDirectionToPoint(waypoint.transform.position);
        
//        float localMaxSpeed = Arrival(maxSpeed);

        Vector3 diseredVelocity = toPoint.normalized * maxSpeed;
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

    private float Arrival(float localMaxSpeed)
    {
        if (distance < slowMovemntRaius)
        {
            localMaxSpeed = Mathf.Lerp(0, maxSpeed, slowMovemntRaius*distance);
        }
        return localMaxSpeed;
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