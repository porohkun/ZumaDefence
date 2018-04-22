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

    public List<Tower> NextTowers { get; private set; }

    private void Awake()
    {
        NextTowers = new List<Tower>();
    }

    private void Update()
    {
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
