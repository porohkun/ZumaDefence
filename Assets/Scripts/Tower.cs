using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Tower : ZumaItem
{
    public int Cost;

    private bool _inLine = false;

    public void BeginFly(List<ZumaItem> items, Vector3 direction)
    {
        StartCoroutine(FlyToLine(items, direction));
    }

    private IEnumerator FlyToLine(List<ZumaItem> items, Vector3 direction)
    {
        ZumaItem collider = null;

        while (collider == null)
        {
            yield return null;
            transform.localPosition += direction * Settings.TowerFlySpeed * Time.deltaTime;

            if (transform.position.x < -500 || transform.position.x > 500 || transform.position.y < -300 || transform.position.y > 300)
                Destroy(gameObject);

            foreach (var item in items)
                if (item.transform.position.DistanceTo(transform.position) <= Settings.ItemSize)
                {
                    collider = item;
                    break;
                }
        }

        var wpDist = collider.LastWaypoint.transform.position.DistanceTo(transform.position);
        var wpNextDist = collider.LastWaypoint.Next.transform.position.DistanceTo(transform.position);
        var inLinePosition = wpDist / (wpDist + wpNextDist) * collider.LastWaypoint.DistanceToNext + collider.LastWaypoint.InLinePosition;
        if (collider.Distance > inLinePosition)
            collider = collider.Preview;

        var time = Settings.InsertTime;
        var startPosition = transform.position;
        while (time > 0f)
        {
            var interpolation = 1f - time / Settings.InsertTime;
            transform.position = Vector3.Lerp(startPosition, GetInLinePosition(collider.Distance + Settings.ItemSize), interpolation);
            var offset = Mathf.Lerp(0f, Settings.ItemSize, interpolation);
            foreach (var next in collider.Forward(false))
                next.Offset = offset;
            time -= Time.deltaTime;
            yield return null;
        }
        Next = collider.Next;
        collider.Next = this;
        if (Next != null)
            Next.Preview = this;
        items.Insert(items.IndexOf(collider) + 1, this);
        Preview = collider;
        foreach (var next in collider.Forward(false))
        {
            next.Distance = next.Preview.Distance + Settings.ItemSize;
            next.Offset = 0f;
        }
        _inLine = true;
    }

    protected override void Update()
    {
        if (_inLine)
        {
            base.Update();
            Health -= Time.deltaTime;
        }
    }
}
