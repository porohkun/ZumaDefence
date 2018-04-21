using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ZumaItem : MonoBehaviour
{
    [SerializeField]
    protected Transform _sprite;
    public Waypoint[] Waypoints;
    public Waypoint LastWaypoint;

    public Quaternion Rotation
    {
        get { return _sprite.rotation; }
        set { _sprite.rotation = value; }
    }

    public float Distance { get; set; }
    public int Index { get; set; }

    public ZumaItem Preview;
    public ZumaItem Next;
    
    protected virtual void Update()
    {
        var dist = Distance;
        foreach (var wp in Waypoints)
        {
            if (dist < wp.DistanceToNext)
            {
                transform.position = wp.transform.position + wp.Direction * dist;
                LastWaypoint = wp;
                break;
            }
            else
                dist -= wp.DistanceToNext;
        }
        var direction = (LastWaypoint.Next.transform.localPosition - LastWaypoint.transform.localPosition).normalized;
        //_sprite.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }

    protected Vector3 GetInLinePosition(float distance)
    {
        var dist = distance;
        var position = Vector3.zero;
        foreach (var wp in Waypoints)
        {
            if (dist < wp.DistanceToNext)
            {
                position = wp.transform.position + wp.Direction * dist;
                break;
            }
            else
                dist -= wp.DistanceToNext;
        }
        return position;
    }
}
