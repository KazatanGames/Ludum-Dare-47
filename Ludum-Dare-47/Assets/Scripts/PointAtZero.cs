using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointAtZero : MonoBehaviour
{
    protected void Awake()
    {
        Point();
    }

    public void Point()
    {
        transform.LookAt(Vector3.zero);
    }
}
