using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class MoneyTower : Tower
{
    float _time = 0;
    protected override void ApplyTowerAction()
    {

    }

    protected override void InnerUpdate()
    {
        base.InnerUpdate();

        _time += Time.deltaTime;
        if (_time > 1f)
        {
            _time -= 1f;
            Map.Instance.Money += (int)_dps;
            var floatText = Instantiate(Settings.FloatTextPrefab);
            floatText.Text = "$" + (int)_dps;
            floatText.transform.position = transform.position + Vector3.back * 5;
        }
    }
}
