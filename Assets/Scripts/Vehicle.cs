using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class Vehicle : MonoBehaviour
{
    [Header("Physics")] [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxForce;
    [SerializeField] private Vector3 velocity;
    [SerializeField] private Vector3 steering;

    [Header("Movemental")] [SerializeField] private float changeDistance;
    [SerializeField] private float slowMovemntRaius;
    [SerializeField] private RoadPoint waypoint;
    [SerializeField] private Vector3 pathPoint;

    [Header("Other")] [SerializeField] private bool isMoving = true;
    [SerializeField] private bool isCollisionDetect;
    [SerializeField] private bool isDrawDetectors;
    [SerializeField] private float distance;

    [Header("Overtaking Detectors")] [SerializeField] private List<Transform> overtakingDetectors =
        new List<Transform>();

    [SerializeField] private bool isAiEnabled = false;
    [SerializeField] private float collisionDetectionDistance;
    [SerializeField] private float overtakingOffset;

    [Header("Turning Detectors")] [SerializeField] private List<Transform> turningDetectors = new List<Transform>();

    private List<RoadPoint> waypoints;

    private Vector3 directionToPoint;

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(pathPoint, .05f);

        if (isDrawDetectors)
        {
            foreach (var detector in overtakingDetectors)
            {
                Gizmos.DrawLine(detector.position, detector.position + detector.forward*collisionDetectionDistance);
            }
        }
    }

    void Start()
    {
        velocity = transform.forward.normalized*maxSpeed;
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


        Vector3 V2 = Vector3.Project(V1, onNormal);

        pathPoint = waypoint.prevPoint.transform.position + V2 + onNormal.normalized*changeDistance;
        if (isCollisionDetect)
        {
            pathPoint += (Quaternion.Euler(0, -90, 0)*V2.normalized*overtakingOffset);
        }
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
                    var component = hit.transform.gameObject.GetComponent<Vehicle>();
                    if (component.velocity.magnitude > maxSpeed)
                    {
                        return;
                    }

                    var hitDistance = hit.distance;

                    if (hitDistance <= collisionDetectionDistance)
                    {
                        Debug.DrawLine(overtakingDetector.position,
                            overtakingDetector.position + overtakingDetector.forward*hitDistance, Color.red);
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

        Vector3 diseredVelocity = toPoint.normalized*maxSpeed;
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