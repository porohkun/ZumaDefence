using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameLayer : LayerBase
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
    private Map _mapPrefab;
    [SerializeField]
    private Text _moneyLabel;
    [SerializeField]
    private string _moneyPrefix;
    [SerializeField]
    private Text _scoreLabel;
    [SerializeField]
    private string _scorePrefix;
    [SerializeField]
    private Text _timeLabel;
    [SerializeField]
    private string _timePrefix;
    [SerializeField]
    private NextTowerData[] _nextTowers;

    private Map _map;
    private TurretMotor _turret;

    public void Initialize(string difficulty, float speed, float moneyMod)
    {
        _map = Instantiate(_mapPrefab);
        _map.Difficulty = difficulty;
        _map.transform.position = Vector3.zero;
        _map.transform.localScale = Vector3.one;
        _turret = _map.Turret;
        Settings.MoneyMod = moneyMod;
        Time.timeScale = speed;
    }

    private void Update()
    {
        //if (Application.isEditor)
        //{
        //    var hurry = Input.GetKey(KeyCode.H);
        //    var slow = Input.GetKey(KeyCode.S);

        //    Time.timeScale = hurry ? 10f : (slow ? 0.1f : 1f);
        //}

        _moneyLabel.text = _moneyPrefix + _map.Money;
        _scoreLabel.text = _scorePrefix + _map.Score;
        _timeLabel.text = _timePrefix + new TimeSpan(0, 0, (int)_map.SpendTime).ToString();

        for (int i = 0; i < Math.Min(_turret.NextTowers.Count, _nextTowers.Length); i++)
        {
            var t = _turret.NextTowers[i];
            _nextTowers[i].SetTower(t.GetSprite(), t.Cost);
        }
    }

    internal void Clear()
    {
        Destroy(_map.gameObject);
    }
}
