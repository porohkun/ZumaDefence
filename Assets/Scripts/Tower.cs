using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Tower : ZumaItem
{
    const float _flySpeed = 400f;
    const float _insertTime = 0.1f;

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
            transform.localPosition += direction * _flySpeed * Time.deltaTime;

            if (transform.position.x < -500 || transform.position.x > 500 || transform.position.y < -300 || transform.position.y > 300)
                Destroy(gameObject);

            foreach (var item in items)
                if (item.transform.position.DistanceTo(transform.position) <= Map.ItemSize)
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

        var time = _insertTime;
        var startPosition = transform.position;
        while (time > 0f)
        {
            var interpolation = 1f - time / _insertTime;
            transform.position = Vector3.Lerp(startPosition, GetInLinePosition(collider.Distance + Map.ItemSize), interpolation);
            var offset = Mathf.Lerp(collider.Distance + Map.ItemSize, collider.Distance + Map.ItemSize * 2, interpolation);
            var next = collider.Next;
            while (next != null)
            {
                next.Distance = offset;
                next = next.Next;
                offset += Map.ItemSize;
            }
            time -= Time.deltaTime;
            yield return null;
        }
        Next = collider.Next;
        collider.Next = this;
        if (Next != null)
            Next.Preview = this;
        items.Insert(items.IndexOf(collider) + 1, this);
        Preview = collider;
        {
            var next = collider.Next;
            while (next != null)
            {
                next.Distance = next.Preview.Distance + Map.ItemSize;
                next = next.Next;
            }
        }
        _inLine = true;
    }

    protected override void Update()
    {
        if (_inLine)
            base.Update();
    }
}
