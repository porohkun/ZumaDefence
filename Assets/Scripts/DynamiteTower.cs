using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DynamiteTower : Tower
{
    [SerializeField]
    private Transform _explosionPrefab;

    protected override void ApplyTowerAction()
    {

    }

    public override void BeforeDestroyAction()
    {
        if (Preview != null)
        {
            Preview.Health = 0;
            if (Preview.Preview != null)
                Preview.Preview.Health -= Preview.Preview.MaxHealth / 2f;
        }
        if (Next != null)
        {
            Next.Health = 0;
            if (Next.Next != null)
                Next.Next.Health -= Next.Next.MaxHealth / 2f;
        }

        var expl = Instantiate(_explosionPrefab);
        expl.position = transform.position;
        var direction = (LastWaypoint.Next.transform.localPosition - LastWaypoint.transform.localPosition).normalized;
        expl.rotation = Quaternion.LookRotation(Vector3.forward, direction);
    }
}
