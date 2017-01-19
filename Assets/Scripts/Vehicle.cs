using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vehicle : MonoBehaviour
{

    [SerializeField]
    private Transform trajectory;

    [SerializeField]
    private float speed;

    [SerializeField]
    private float changeDistance;

    [SerializeField]
    private RoadPoint waypoint;
   
    private List<Transform> waypoints;

	// Use this for initialization
	void Start ()
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
	void FixedUpdate ()
	{
	    float dt = speed*Time.fixedDeltaTime;

	    float distance = Vector3.Distance(waypoint.transform.position, transform.position);

	    Debug.Log(distance);

	    if (distance < changeDistance)
	    {
	        NextWayPoint();
	    }

	    var directionToPoint = GetDirectionToPoint().normalized;

	    Vector3 movement = new Vector3(directionToPoint.x * dt, 0f, directionToPoint.z * dt);
	    transform.position += movement;
	}

    private Vector3 GetDirectionToPoint()
    {
        return waypoint.transform.position - transform.position;
    }

    private void NextWayPoint()
    {
        waypoint = waypoint.nextPoint[0];

        var directionToPoint = GetDirectionToPoint();
        directionToPoint.x = 0;
        directionToPoint.z = 0;
        transform.LookAt(waypoint.transform.position);
    }
}
