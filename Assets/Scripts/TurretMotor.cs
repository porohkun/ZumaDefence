using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TurretMotor : MonoBehaviour
{
    [SerializeField]
    private Map _map;
    [SerializeField]
    private Tower[] _towerPrefabs;
    [SerializeField]
    private float _checkTime = 2f;

    public List<Tower> NextTowers { get; private set; }

    private float _time = 0f;
    private int _check = 0;

    private void Awake()
    {
        NextTowers = new List<Tower>();
    }

    private void Update()
    {
        _time += Time.unscaledDeltaTime;
        if (_time > _checkTime)
        {
            bool check = false;
            _time -= _checkTime;
            if (NextTowers[0].Cost > Map.Instance.Money)
            {
                check = true;
                foreach (var item in Map.Instance.Items)
                    if (!item.Enemy && !(item is RepairTower))
                    {
                        check = false;
                        break;
                    }
            }
            if (check)
                _check++;
            else
                _check = 0;
            if (_check == 3)
            {
                Time.timeScale *= 10f;
            }
        }

        while (NextTowers.Count < 4)
            NextTowers.Add(_towerPrefabs[Random.Range(0, _towerPrefabs.Length)]);
        var curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = (curPos - transform.position).normalized;
        direction.z = 0f;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        if (Input.GetMouseButtonDown(0))
        {
            var nextTower = NextTowers[0];
            if (_map.CreateTower(nextTower, transform.localPosition, direction))
                NextTowers.RemoveAt(0);
        }
    }
}
