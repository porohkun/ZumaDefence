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
    private Map _map;
    [SerializeField]
    private TurretMotor _turret;
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

    private void Update()
    {
        _moneyLabel.text = _moneyPrefix + _map.Money;
        _scoreLabel.text = _scorePrefix + _map.Score;

        for (int i = 0; i < Math.Min(_turret.NextTowers.Count, _nextTowers.Length); i++)
        {
            var t = _turret.NextTowers[i];
            _nextTowers[i].SetTower(t.GetSprite(), t.Cost);
        }
    }
}
