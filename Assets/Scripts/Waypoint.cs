using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{
    public LineRenderer Line;
    public Waypoint Preview;
    public Waypoint Next;

    public float DistanceToNext { get; private set; }
    public Vector3 Direction { get { return (Next.transform.position - transform.position).normalized; } }

    public float InLinePosition
    {
        get
        {
            var result = 0f;
            var current = Preview;
            while (current != null)
            {
                result += current.DistanceToNext;
                current = current.Preview;
            }
            return result;
        }
    }

    public void Awake()
    {
        if (Next != null)
            DistanceToNext = transform.position.DistanceTo(Next.transform.position);
    }
}
