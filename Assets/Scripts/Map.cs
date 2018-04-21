using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Map : MonoBehaviour
{
    public Waypoint[] Waypoints;
    [SerializeField]
    private Transform _unitsRoot;
    


    private List<Unit> _units;
}
