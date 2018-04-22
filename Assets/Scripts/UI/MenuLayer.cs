using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MenuLayer : LayerBase
{
    public void OnEasy()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.Push<GameLayer>().Initialize(0.7f, 1.5f);
            LayersManager.FadeIn(0.5f, null);
        });
    }

    public void OnMedium()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.Push<GameLayer>().Initialize(1f, 1f);
            LayersManager.FadeIn(0.5f, null);
        });
    }

    public void OnHard()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.Push<GameLayer>().Initialize(1f, 0.65f);
            LayersManager.FadeIn(0.5f, null);
        });
    }
}
