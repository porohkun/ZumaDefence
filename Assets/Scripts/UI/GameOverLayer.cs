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
    [SerializeField]
    private InputField _nameField;
    [SerializeField]
    private GameObject _bestScorePanel;
    [SerializeField]
    private GameObject _pausePanel;

    private string _difficulty;
    private int _score;

    public override void OnQuit()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.PopTill<MenuLayer>();
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

    public void OnSend()
    {
        if (_nameField.text.Length > 2)
        {
            _pausePanel.SetActive(true);
            PlayFab.LoginUser(() =>
            {
                PlayFab.SendName(_nameField.text, () =>
                {
                    PlayFab.SendScores(_difficulty, _score, () =>
                     {
                         LayersManager.FadeOut(0.5f, () =>
                         {
                             LayersManager.Push<LeaderBoardLayer>();
                             LayersManager.FadeIn(0.5f, null);
                         });
                     });
                });
            });
        }
    }

    internal void Initialize(string difficulty, float spendTime, int score)
    {
        _pausePanel.SetActive(false);

        _difficulty = difficulty;
        _score = score;
        _timeLabel.text = _timePrefix + new TimeSpan(0, 0, (int)spendTime).ToString();
        _scoreLabel.text = _scorePrefix + score;

        LeaderBoard lb;
        switch (_difficulty)
        {
            case "Easy":
            default:
                lb = LeaderBoard.EasyBoard;
                break;
            case "Medium":
                lb = LeaderBoard.MediumBoard;
                break;
            case "Hard":
                lb = LeaderBoard.HardBoard;
                break;
        }
        _bestScorePanel.SetActive(false);
        foreach (var item in lb.Table)
            if (item.Score < _score)
                _bestScorePanel.SetActive(true);
    }
}
