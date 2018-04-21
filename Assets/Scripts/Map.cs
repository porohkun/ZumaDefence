using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Map : MonoBehaviour
{
    public const float ItemSize = 32;
    [SerializeField]
    private float _speed = 5f;
    public Waypoint[] Waypoints;
    [SerializeField]
    private Transform _unitsRoot;
    [SerializeField]
    private Unit _unitPrefab;

    private List<ZumaItem> _items = new List<ZumaItem>();

    private void Update()
    {
        if (_items.Count == 0 || (_items[0].transform.position - Waypoints[0].transform.position).magnitude >= ItemSize)
        {
            CreateUnit();
            UpdateIndexes();
        }

        foreach (var item in _items)
        {
            item.Distance += _speed * Time.deltaTime;
        }
    }

    public void CreateTower(Tower prefab, Vector3 startPosition, Vector3 direction)
    {
        var tower = Instantiate(prefab, _unitsRoot);
        tower.transform.localPosition = startPosition;
        tower.Waypoints = Waypoints;
        tower.BeginFly(_items,direction);
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
            unit.transform.localPosition = Waypoints[0].transform.localPosition + dir.normalized * (magn - ItemSize);

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
