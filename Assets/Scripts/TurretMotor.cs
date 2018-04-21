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

    private void Update()
    {
        var curPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var direction = (curPos - transform.position).normalized;
        direction.z = 0f;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, direction);

        if (Input.GetMouseButtonDown(0))
        {
            _map.CreateTower(GetRndTowerPrefab(), transform.localPosition, direction);
        }
    }

    private Tower GetRndTowerPrefab()
    {
        return _towerPrefabs[Random.Range(0, _towerPrefabs.Length)];
    }
}
