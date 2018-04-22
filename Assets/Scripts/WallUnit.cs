using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WallUnit : ZumaItem
{
    [SerializeField]
    private float _jumpTime;

    private float _time;

    protected override void InnerUpdate()
    {
        base.InnerUpdate();

        _time += Time.deltaTime;
        if (_time > 2f)
        {
            _time -= 2f;
            if (Next != null && Preview != null && (Next.Enemy || Preview.Enemy))
            {
                var n = Next;
                var p = Preview;

                p.Next = n;
                n.Preview = p;
                Preview = n;
                Next = n.Next;
                n.Next = this;
                if (Next != null)
                    Next.Preview = this;

                n.Distance -= Settings.ItemSize;
                n.Offset += Settings.ItemSize;
                Distance += Settings.ItemSize;
                Offset -= Settings.ItemSize;
            }
        }
    }
}
