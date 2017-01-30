using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RoadPoint : MonoBehaviour
{
    public RoadPoint prevPoint;

    public List<RoadPoint> nextPoint;

    void OnDrawGizmos()
    {
        for (int i = 0; i < nextPoint.Count; i++)
        {
            Gizmos.DrawLine(transform.position, nextPoint[i].transform.position);
        }
    }
}
