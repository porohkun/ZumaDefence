using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameLayer : MonoBehaviour
{
    [Serializable]
    public class NextTowerData
    {
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Text _cost;

        public void SetTower(Sprite sprite, int cost)
        {
            _image.sprite = sprite;
            _cost.text = '$' + cost.ToString();
        }
    }
    [SerializeField]
    private Text _moneyLabel;
    [SerializeField]
    private string _moneyPrefix;
    [SerializeField]
    private Text _scoreLabel;
    [SerializeField]
    private string _scorePrefix;
    [SerializeField]
    private NextTowerData[] _nextTowers;
}
