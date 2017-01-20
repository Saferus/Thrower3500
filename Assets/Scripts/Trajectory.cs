using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Trajectory : MonoBehaviour
{

    private List<Transform> points;

    void Start()
    {
        points = new List<Transform>(transform.childCount);
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            points.Add(child);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 prevPos = Vector3.one;
        for (int i = 0; i < points.Count; i++)
        {
            if (i != 0)
            {
                Debug.DrawLine(prevPos, points[i].position);
            }

            prevPos = points[i].transform.position;
        }
    }
}