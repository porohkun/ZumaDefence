using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Unit : ZumaItem
{
    public Sprite[] HealthSprites;
    
    [SerializeField]
    private SpriteRenderer _hpRenderer;
    
}
