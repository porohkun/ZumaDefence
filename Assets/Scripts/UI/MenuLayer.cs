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
            LayersManager.Push<GameLayer>().Initialize("Easy", 0.7f, 1.5f);
            LayersManager.FadeIn(0.5f, null);
        });
    }

    public void OnMedium()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.Push<GameLayer>().Initialize("Medium", 1f, 1f);
            LayersManager.FadeIn(0.5f, null);
        });
    }

    public void OnHard()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.Push<GameLayer>().Initialize("Hard", 1f, 0.65f);
            LayersManager.FadeIn(0.5f, null);
        });
    }

    public void OnLeaderBoard()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.Push<LeaderBoardLayer>();
            LayersManager.FadeIn(0.5f, null);
        });
    }
}
