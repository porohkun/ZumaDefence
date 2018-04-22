using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class StartLayer : LayerBase
{
    void Start()
    {
        LayersManager.Push<MenuLayer>();
        LayersManager.FadeIn(0.5f, null);
    }
}
