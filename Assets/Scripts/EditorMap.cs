using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class EditorMap : MonoBehaviour
{
    [SerializeField]
    private Map _map;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor && Application.isPlaying)
        {
            if (_map != null)
                for (int i = 0; i < _map.Waypoints.Length; i++)
                    _map.Waypoints[i].gameObject.SetActive(false);
            Destroy(this);
        }
    }

    private void Update()
    {
        if (_map != null)
        {
            for (int i = 0; i < _map.Waypoints.Length; i++)
            {
                var vp = _map.Waypoints[i];
                vp.name = "Waypoint_" + i;
                if (vp.Line == null)
                {
                    var lr = vp.GetComponent<LineRenderer>();
                    if (lr == null)
                        lr = vp.gameObject.AddComponent<LineRenderer>();
                    vp.Line = lr;
                }
                if (i < _map.Waypoints.Length - 1)
                {
                    if (vp.Next == null)
                        vp.Next = _map.Waypoints[i + 1];
                    vp.Line.SetPosition(1, vp.Next.transform.position - vp.transform.position);
                }
            }
        }
    }
}
