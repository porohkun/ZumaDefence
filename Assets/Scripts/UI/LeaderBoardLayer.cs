using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardLayer : LayerBase
{
    [SerializeField]
    private RectTransform _easyTable;
    [SerializeField]
    private RectTransform _mediumTable;
    [SerializeField]
    private RectTransform _hardTable;
    [SerializeField]
    private LeaderBoardItem _leaderBoardItemPrefab;
    [SerializeField]
    private RectTransform _spinner;

    private LeaderBoardItem[] _easyItems;
    private LeaderBoardItem[] _mediumItems;
    private LeaderBoardItem[] _hardItems;

    private int _dataReceived;

    private void Awake()
    {
        _easyItems = new LeaderBoardItem[10];
        _mediumItems = new LeaderBoardItem[10];
        _hardItems = new LeaderBoardItem[10];

        for (int i = 0; i < 10; i++)
        {
            _easyItems[i] = Instantiate(_leaderBoardItemPrefab, _easyTable);
            _mediumItems[i] = Instantiate(_leaderBoardItemPrefab, _mediumTable);
            _hardItems[i] = Instantiate(_leaderBoardItemPrefab, _hardTable);
        }
    }

    internal override void OnFloatUp()
    {
        base.OnFloatUp();
        _spinner.gameObject.SetActive(true);
        _dataReceived = 0;
        ShowEasyBoard();
        ShowMediumBoard();
        ShowHardBoard();

        PlayFab.GetScores("Easy", (lb) =>
        {
            LeaderBoard.EasyBoard = lb;
            ShowEasyBoard();
            StopSpinner();
        });
        PlayFab.GetScores("Medium", (lb) =>
        {
            LeaderBoard.MediumBoard = lb;
            ShowMediumBoard();
            StopSpinner();
        });
        PlayFab.GetScores("Hard", (lb) =>
        {
            LeaderBoard.HardBoard = lb;
            ShowHardBoard();
            StopSpinner();
        });
    }

    private void ShowEasyBoard()
    {
        for (int i = 0; i < LeaderBoard.EasyBoard.Table.Length; i++)
            _easyItems[i].SetScore(LeaderBoard.EasyBoard.Table[i]);
    }

    private void ShowMediumBoard()
    {
        for (int i = 0; i < LeaderBoard.MediumBoard.Table.Length; i++)
            _mediumItems[i].SetScore(LeaderBoard.MediumBoard.Table[i]);
    }

    private void ShowHardBoard()
    {
        for (int i = 0; i < LeaderBoard.HardBoard.Table.Length; i++)
            _hardItems[i].SetScore(LeaderBoard.HardBoard.Table[i]);
    }

    private void StopSpinner()
    {
        _dataReceived++;
        if (_dataReceived >= 3)
            _spinner.gameObject.SetActive(false);
    }

    private void Update()
    {
        _spinner.Rotate(0f, 0f, 500f * Time.unscaledDeltaTime);
    }

    public override void OnQuit()
    {
        LayersManager.FadeOut(0.5f, () =>
        {
            LayersManager.PopTill<MenuLayer>();
            LayersManager.FadeIn(0.5f, null);
        });
    }
}
