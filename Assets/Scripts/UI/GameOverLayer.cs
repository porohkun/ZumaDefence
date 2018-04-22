using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameOverLayer : LayerBase
{
    [SerializeField]
    private Text _timeLabel;
    [SerializeField]
    private string _timePrefix;
    [SerializeField]
    private Text _scoreLabel;
    [SerializeField]
    private string _scorePrefix;

    public override void OnQuit()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.PopTill<MenuLayer>();
            LayersManager.FadeIn(0.5f, null);
        });
    }

    internal void Initialize(float spendTime, int score)
    {
        _timeLabel.text = _timePrefix + new TimeSpan(0, 0, (int)spendTime).ToString();
        _scoreLabel.text = _scorePrefix + score;
    }
}
