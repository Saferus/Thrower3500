using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Trajectory : MonoBehaviour
{

    private List<RoadPoint> points;

    void Start()
    {
        var componentsInChildren = GetComponentsInChildren<RoadPoint>();

        points = new List<RoadPoint>(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            points.Add(componentsInChildren[i]);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 prevPos = Vector3.one;
        for (int i = 0; i < points.Count; i++)
        {
            if (i != 0)
            {
                Debug.DrawLine(prevPos, points[i].transform.position);
            }

            prevPos = points[i].transform.position;
        }
    }

    public List<RoadPoint> GetRoadPoints()
    {
        return points;
    }
}