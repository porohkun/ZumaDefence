using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class RepairTower : Tower
{
    protected override void ApplyTowerAction()
    {
        if (_target == null || _target.Enemy != Enemy || _target.Destroyed || (_target != Next && _target != Preview) || _target.Health * 1.2f >= _target.MaxHealth)
            FindTarget();

        if (_target != null)
        {
            _laser.gameObject.SetActive(true);
            var direction = (_target.transform.localPosition - transform.localPosition).normalized;
            //Rotation = Quaternion.LookRotation(Vector3.forward, direction);
            _laser.SetPosition(0, transform.position + direction * 13f);
            _laser.SetPosition(1, _target.transform.position);
            _target.Health += _dps * Time.deltaTime;
        }
        else
            _laser.gameObject.SetActive(false);
    }

    private void FindTarget()
    {
        if (Next != null && Next.Enemy == Enemy && Next.Health * 2f < Next.MaxHealth)
            _target = Next;
        else if (Preview != null && Preview.Enemy == Enemy && Preview.Health * 2f < Preview.MaxHealth)
            _target = Preview;
        //else if (Next != null && Next.Enemy == Enemy)
        //    _target = Next;
        //else if (Preview != null && Preview.Enemy == Enemy)
        //    _target = Preview;
        else
            _target = null;
    }

}
