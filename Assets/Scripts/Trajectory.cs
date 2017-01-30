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
    
    public List<RoadPoint> GetRoadPoints()
    {
        return points;
    }
}