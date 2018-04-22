using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Map : MonoBehaviour
{
    public static Map Instance { get; private set; }

    [Serializable]
    public class UnitWeightData
    {
        [Serializable]
        public class WeightTime
        {
            public float Time;
            public float Weight;
        }
        public ZumaItem Prefab;
        public WeightTime[] Weights;

        public float GetWeight(float time)
        {
            WeightTime a = null;
            WeightTime b = null;

            foreach (var weight in Weights)
            {
                b = weight;
                if (weight.Time > time)
                    break;
                a = weight;
            }

            if (a == b)
                return a.Weight;

            var d = b.Time - a.Time;
            var s = time - a.Time;
            return Mathf.Lerp(a.Weight, b.Weight, s / d);
        }
    }

    [SerializeField]
    private float _speed = 5f;
    public Waypoint[] Waypoints;
    [SerializeField]
    private Transform _unitsRoot;
    [SerializeField]
    private UnitWeightData[] _unitPrefabs;
    public int Money;
    public int Score;

    public float SpendTime { get; private set; }

    private List<ZumaItem> _items = new List<ZumaItem>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        SpendTime += Time.deltaTime;

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
                Money += item.Reward;
                Score += item.Score;
                Destroy(item.gameObject);
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
        var unit = Instantiate(GetUnitPrefab(), _unitsRoot);
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

    private ZumaItem GetUnitPrefab()
    {
        var weights = _unitPrefabs.Select(p => p.GetWeight(SpendTime)).ToArray();

        for (int i = weights.Length - 1; i >= 0; i--)
            for (int j = i + 1; j < weights.Length; j++)
                weights[j] += weights[i];
        var result = weights.Length - 1;
        var roll = UnityEngine.Random.Range(0f, weights[result]);
        for (int i = 0; i < weights.Length; i++)
        {
            if (weights[i] > roll)
            {
                result = i;
                break;
            }
        }
        return _unitPrefabs[result].Prefab;
    }

    private void UpdateIndexes()
    {
        for (int i = 0; i < _items.Count; i++)
            _items[i].Index = i;
    }
}
