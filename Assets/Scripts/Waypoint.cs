using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour, ITwoDirections<Waypoint>
{
    public LineRenderer Line;
    [SerializeField]
    private Waypoint _previewSerialized;
    [SerializeField]
    private Waypoint _nextSerialized;
    public Waypoint Preview { get { return _previewSerialized; } set { _previewSerialized = value; } }
    public Waypoint Next { get { return _nextSerialized; } set { _nextSerialized = value; } }

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

    public void UpdateDistance()
    {
        if (Next != null)
            DistanceToNext = transform.position.DistanceTo(Next.transform.position);
    }
}
