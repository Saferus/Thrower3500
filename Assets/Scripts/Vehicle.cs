using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Vehicle : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 steering;

    [Header("Movemental")]
    [SerializeField] private float changeDistance;
    [SerializeField] private float slowMovemntRaius;
    [SerializeField] private RoadPoint waypoint;
    [SerializeField] private Vector3 pathPoint;

    [Header("Other")]
    [SerializeField] private bool isMoving;
    [SerializeField] private bool isCollisionDetect;
    [SerializeField] private bool isDrawDetectors;
    [SerializeField] private float distance;

    [Header("Detecters")]
    [SerializeField] private List<Transform> overtakingDetectors = new List<Transform>();
    [SerializeField] private float collisionDetectionDistance;
    [SerializeField] private float overtakingOffset;
    
    private List<RoadPoint> waypoints;

    private Vector3 directionToPoint;

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pathPoint, .05f);

        if (isDrawDetectors)
        {
            foreach (var detector in overtakingDetectors)
            {
                Gizmos.DrawLine(detector.position, detector.position + detector.forward * collisionDetectionDistance);
            }
        }

    }

    void Start()
    {
        velocity = transform.forward.normalized * maxSpeed;
    }
    
    void FixedUpdate()
    {
        if (!isMoving)
        {
            return;
        }

        CheckCollision();
        FindPathPoint();

        distance = Vector3.Distance(waypoint.transform.position, transform.position);
        
        Accelerate();

        if (distance < changeDistance)
        {
            NextWayPoint();
        }
    }

    private void FindPathPoint()
    {
        Vector3 V1 = transform.position - waypoint.prevPoint.transform.position;
        var onNormal = waypoint.transform.position - waypoint.prevPoint.transform.position;

        if (isCollisionDetect)
        {
            onNormal -= Vector3.left * overtakingOffset;
        }

        Vector3 V2 = Vector3.Project(V1, onNormal);

        pathPoint = waypoint.prevPoint.transform.position + V2 + onNormal.normalized * changeDistance;
    }

    private void CheckCollision()
    {
        isCollisionDetect = false;

        CheckOvertakingCollision();
    }

    private void CheckOvertakingCollision()
    {
        RaycastHit hit;
        for (int i = 0; i < overtakingDetectors.Count; i++)
        {
            var overtakingDetector = overtakingDetectors[i];

            //FrontLeft
            Vector3 fwd = overtakingDetector.transform.forward;
            if (Physics.Raycast(overtakingDetector.position, fwd, out hit))
            {
                if (hit.transform.tag == "Vehicle")
                {
                    var hitDistance = hit.distance;

                    if (hitDistance <= collisionDetectionDistance)
                    {
                        Debug.DrawLine(overtakingDetector.position, overtakingDetector.position + overtakingDetector.forward * hitDistance, Color.red);
                        isCollisionDetect = true;
                        return;
                    }

                }
            }
        }
    }

    private void DebugWaypoint()
    {
        Debug.DrawLine(transform.position, waypoint.transform.position, Color.gray);
    }

    private void Accelerate()
    {
        Vector3 toPoint = GetDirectionToPoint(pathPoint);
        
//        float localMaxSpeed = Arrival(maxSpeed);

        Vector3 diseredVelocity = toPoint.normalized * maxSpeed;
//        Debug.DrawLine(transform.position, transform.position + diseredVelocity, Color.blue);

        steering = diseredVelocity - velocity;
        steering = steering.normalized*maxForce;
//        Debug.DrawLine(transform.position, transform.position + steering * 100, Color.green);

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