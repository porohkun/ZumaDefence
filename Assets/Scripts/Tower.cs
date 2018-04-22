using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Tower : ZumaItem
{
    public int Cost;
    [SerializeField]
    private LineRenderer _laser;
    [SerializeField]
    private float _dps;

    private bool _inLine = false;
    private ZumaItem _target;

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
                if (item.Position.DistanceTo(transform.position) <= Settings.ItemSize)
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
        foreach (var next in collider.Forward(false))
        {
            next.Distance += Settings.ItemSize;
            next.Offset -= Settings.ItemSize;
        }

        Next = collider.Next;
        collider.Next = this;
        if (Next != null)
            Next.Preview = this;
        items.Insert(items.IndexOf(collider) + 1, this);
        Preview = collider;
        Distance = Preview.Distance + Settings.ItemSize;
        Offset = Preview.Offset;

        while (time > 0f)
        {
            var interpolation = 1f - time / Settings.InsertTime;
            transform.position = Vector3.Lerp(startPosition, GetInLinePosition(collider.Distance + Settings.ItemSize), interpolation);
            //var offset = Mathf.Lerp(0f, Settings.ItemSize, interpolation);
            time -= Time.deltaTime;
            yield return null;
        }

        //foreach (var next in collider.Forward(false))
        //{
        //    next.Distance = next.Preview.Distance + Settings.ItemSize;
        //    next.Offset = 0f;
        //}
        _inLine = true;
    }

    protected override void InnerUpdate()
    {
        if (_inLine)
        {
            base.InnerUpdate();
            if (Destroyed)
            {
                _laser.gameObject.SetActive(false);
                return;
            }

            Health -= Time.deltaTime;
            ApplyTowerAction();
        }
    }

    protected virtual void ApplyTowerAction()
    {
        _target = (Next != null && Next.Enemy) ? Next :
                         (Preview != null && Preview.Enemy) ? Preview :
                         (Next != null && Next.Next != null && Next.Next.Enemy) ? Next.Next :
                         (Preview != null && Preview.Preview != null && Preview.Preview.Enemy) ? Preview.Preview :
                         null;

        if (_target != null)
        {
            _laser.gameObject.SetActive(true);
            var direction = (_target.transform.localPosition - transform.localPosition).normalized;
            Rotation = Quaternion.LookRotation(Vector3.forward, direction);
            _laser.SetPosition(0, transform.position + direction * 13f);
            _laser.SetPosition(1, _target.transform.position);
            _target.Health -= _dps * Time.deltaTime;
        }
        else
            _laser.gameObject.SetActive(false);
    }
}
