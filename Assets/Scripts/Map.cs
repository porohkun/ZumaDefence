using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;
    public Waypoint[] Waypoints;
    [SerializeField]
    private Transform _unitsRoot;
    [SerializeField]
    private Unit _unitPrefab;
    public int Money;
    public int Score;

    private List<ZumaItem> _items = new List<ZumaItem>();

    private void Update()
    {
        if (_items.Count == 0 || (_items[0].transform.position - Waypoints[0].transform.position).magnitude >= Settings.ItemSize)
        {
            CreateUnit();
            UpdateIndexes();
        }

        for (int i = _items.Count - 1; i >= 0; i--)
        {
            var item = _items[i];
            if (item.Destroyed)
            {
                _items.Remove(item);
                item.BeforeDestroyAction();
                item.transform.position = Vector3.left * 1000f;
                Money += item.Reward;
                Score += item.Score;
            }
            else
                item.Distance += _speed * Time.deltaTime;
        }


    }

    public bool CreateTower(Tower prefab, Vector3 startPosition, Vector3 direction)
    {
        if (Money >= prefab.Cost)
        {
            Money -= prefab.Cost;
            var tower = Instantiate(prefab, _unitsRoot);
            tower.transform.localPosition = startPosition;
            tower.Waypoints = Waypoints;
            tower.BeginFly(_items, direction);
            return true;
        }
        return false;
    }

    private void CreateUnit()
    {
        var unit = Instantiate(_unitPrefab, _unitsRoot);
        unit.Waypoints = Waypoints;
        if (_items.Count == 0)
            unit.transform.localPosition = Waypoints[0].transform.localPosition;
        else
        {
            var dir = Waypoints[0].transform.position - _items[0].transform.position;
            var magn = dir.magnitude;
            unit.transform.localPosition = Waypoints[0].transform.localPosition + dir.normalized * (magn - Settings.ItemSize);

            var next = _items[0];
            unit.Next = next;
            next.Preview = unit;
        }
        unit.LastWaypoint = Waypoints[0];
        _items.Insert(0, unit);
    }

    private void UpdateIndexes()
    {
        for (int i = 0; i < _items.Count; i++)
            _items[i].Index = i;
    }
}
