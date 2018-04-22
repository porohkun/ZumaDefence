using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ZumaItem : MonoBehaviour, ITwoDirections<ZumaItem>
{
    [SerializeField]
    protected SpriteRenderer _sprite;
    [SerializeField]
    private SpriteRenderer _hpRenderer;

    public Waypoint[] Waypoints { get; set; }
    public Waypoint LastWaypoint { get; set; }

    [SerializeField]
    private float _maxHealth;
    private float _health;

    public bool Enemy;
    public int Reward;
    public int Score;

    public float Health
    {
        get { return _health; }
        set
        {
            _health = Mathf.Min(Mathf.Max(value, 0f), _maxHealth);
            _hpRenderer.sprite = Settings.HealthSprites[(int)(_health / _maxHealth * (Settings.HealthSprites.Length - 1))];
        }
    }
    public float MaxHealth { get { return _maxHealth; } }

    public Quaternion Rotation
    {
        get { return _sprite.transform.rotation; }
        set { _sprite.transform.rotation = value; }
    }

    public Vector3 Position { get; private set; }

    public virtual void BeforeDestroyAction()
    {

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
        _hpRenderer.color = Enemy ? Settings.EnemyHpColor : Settings.FriendlyHpColor;
    }

    private void Update()
    {
        if (Offset > 0.001f)
        {
            var offset = Mathf.Min(Offset, Settings.ItemSize * Time.deltaTime / Settings.InsertTime);
            Offset -= offset;
        }
        else if (Offset < -0.001f)
        {
            var offset = Mathf.Min(-Offset, Settings.ItemSize * Time.deltaTime / Settings.InsertTime);
            Offset += offset;
        }
        else if (Preview != null)
        {
            var offset = Preview.Distance + Settings.ItemSize - Distance;
            Distance += offset;
            offset -= offset;
        }
        var dist = Distance + Offset;
        foreach (var wp in Waypoints)
        {
            if (dist < wp.DistanceToNext)
            {
                Position = wp.transform.position + wp.Direction * dist;
                LastWaypoint = wp;
                break;
            }
            else
            {
                dist -= wp.DistanceToNext;
                if (wp.Next == null)
                {
                    Time.timeScale = 0f;
                    LayersManager.FadeOut(0.5f, () =>
                    {
                        LayersManager.Push<GameOverLayer>().Initialize(Map.Instance.SpendTime, Map.Instance.Score);
                        LayersManager.GetLayer<GameLayer>().Clear();
                        LayersManager.FadeIn(0.5f, null);
                    });
                    enabled = false;
                }
            }
        }
        InnerUpdate();
    }

    protected virtual void InnerUpdate()
    {
        if (!Destroyed)
        {
            transform.position = Position;

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
                if (Reward > 0)
                {
                    var floatText = Instantiate(Settings.FloatTextPrefab);
                    floatText.Text = "$" + Mathf.RoundToInt(Reward * Settings.MoneyMod);
                    floatText.transform.position = transform.position + Vector3.back * 5;
                }
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

    public Sprite GetSprite()
    {
        return _sprite.sprite;
    }
}
