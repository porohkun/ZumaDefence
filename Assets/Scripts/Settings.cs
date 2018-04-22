using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Settings : MonoBehaviour
{
    [SerializeField]
    private float _itemSize = 32f;
    [SerializeField]
    private float _towerFlySpeed = 400f;
    [SerializeField]
    private float _insertTime = 0.1f;
    [SerializeField]
    private Color _friendlyHpColor;
    [SerializeField]
    private Color _enemyHpColor;
    [SerializeField]
    private Sprite[] _healthSprites;
    [SerializeField]
    private FloatText _floatTextPrefab;


    public static float ItemSize { get { return Instance._itemSize; } }
    public static float TowerFlySpeed { get { return Instance._towerFlySpeed; } }
    public static float InsertTime { get { return Instance._insertTime; } }
    public static Color FriendlyHpColor { get { return Instance._friendlyHpColor; } }
    public static Color EnemyHpColor { get { return Instance._enemyHpColor; } }
    public static Sprite[] HealthSprites { get { return Instance._healthSprites; } }

    public static float MoneyMod { get; internal set; }

    public static FloatText FloatTextPrefab { get { return Instance._floatTextPrefab; } }


    private bool _initialized;
    private static Settings _instance = null;
    private static Settings Instance
    {
        get
        {
            if (!Application.isPlaying)
                return null;
            if (_instance != null)
                return _instance;

            //Don't touch this
            var inst = GameObject.FindObjectOfType<Settings>();
            //.~.~'~'~':~:',++== ThIs Is A fUcKiN mAgIc ==++,':~:'~'~'~.~.

            if (inst != null)
                _instance = inst;

            if (_instance == null)
            {
                inst = Instantiate(Prefab);
                if (inst != null)
                    _instance = inst;
            }

            if (!_instance._initialized)
                _instance.Initialize();

            return _instance;
        }
    }

    public static Settings Prefab
    {
        get
        {
            return Resources.Load<Settings>("Settings");
        }
    }

    protected virtual void Initialize()
    {
        this._initialized = true;
        DontDestroyOnLoad(_instance.gameObject);
    }
}
