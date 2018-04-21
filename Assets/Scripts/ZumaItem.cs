using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ZumaItem : MonoBehaviour, ITwoDirections<ZumaItem>
{
    [SerializeField]
    protected Transform _sprite;
    [SerializeField]
    private SpriteRenderer _hpRenderer;
    public Sprite[] HealthSprites;

    public Waypoint[] Waypoints;
    public Waypoint LastWaypoint;

    [SerializeField]
    private float _maxHealth;
    private float _health;

    public float Health
    {
        get { return _health; }
        set
        {
            _health = Math.Max(value, 0f);
            _hpRenderer.sprite = HealthSprites[(int)(_health / _maxHealth * (HealthSprites.Length - 1))];
        }
    }

    public Quaternion Rotation
    {
        get { return _sprite.rotation; }
        set { _sprite.rotation = value; }
    }

    public float Distance { get; set; }
    public float Offset { get; set; }
    public int Index { get; set; }
    public bool Destroyed { get; private set; }

    public ZumaItem Preview { get; set; }
    public ZumaItem Next { get; set; }

    private void Awake()
    {
        _health = _maxHealth;
    }

    protected virtual void Update()
    {
        if (!Destroyed)
        {
            var dist = Distance + Offset;
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
            //var direction = (LastWaypoint.Next.transform.localPosition - LastWaypoint.transform.localPosition).normalized;

            if (Health <= 0f)
            {
                if (Next != null)
                    Next.Preview = Preview;
                if (Preview != null)
                    Preview.Next = Next;
                foreach (var next in this.Forward(false))
                {
                    next.Distance -= Settings.ItemSize;
                    next.Offset += Settings.ItemSize;
                }
                Destroyed = true;
                StartCoroutine(ReturnOffsetAfterDestroy());
            }
        }
    }

    private IEnumerator ReturnOffsetAfterDestroy()
    {
        var sumOffset = 0f;
        while (sumOffset < Settings.ItemSize)
        {
            var offset = Settings.ItemSize * Time.deltaTime / Settings.InsertTime;
            sumOffset += offset;
            foreach (var next in this.Forward(false))
                next.Offset -= offset;
            yield return null;
        }
        foreach (var next in this.Forward(false))
            next.Offset += sumOffset - Settings.ItemSize;
        Destroy(gameObject);
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
