using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardItem : MonoBehaviour
{
    [SerializeField]
    private Text _scoreLabel;
    [SerializeField]
    private Text _nameLabel;

    public void SetScore(string name, int score)
    {
        _nameLabel.text = name;
        _scoreLabel.text = score.ToString();
    }

    public void SetScore(LeaderBoard.LeaderBoardItem lbi)
    {
        SetScore(lbi.Name, lbi.Score);
    }
    
}
