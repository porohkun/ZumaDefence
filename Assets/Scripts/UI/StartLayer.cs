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

        PlayFab.LoginUser(() =>
        {
            PlayFab.GetScores("Easy", (lb) =>
            {
                LeaderBoard.EasyBoard = lb;
            });
            PlayFab.GetScores("Medium", (lb) =>
            {
                LeaderBoard.MediumBoard = lb;
            });
            PlayFab.GetScores("Hard", (lb) =>
            {
                LeaderBoard.HardBoard = lb;
            });
        });
    }
}
