using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class FloatText : MonoBehaviour
{
    [SerializeField]
    private TextMesh _textMesh;
    [SerializeField]
    private MeshRenderer _meshRenderer;

    public string Text
    {
        get { return _textMesh.text; }
        set { _textMesh.text = value; }
    }

    public float LifeTime = 1f;
    public float Distance = 32f;

    private void Start()
    {
        StartCoroutine(FloatUp());
        _meshRenderer.sortingLayerName = "FloatText";
    }

    private IEnumerator FloatUp()
    {
        var time = 0f;
        var start = transform.position;
        while (time < LifeTime)
        {
            transform.position = start + Distance * Vector3.up * EaseOutPower(time / LifeTime, 2);
            time += Time.deltaTime;
            yield return null;
        }
        Destroy(gameObject);
    }

    private static float EaseOutPower(float progress, int power)
    {
        int sign = power % 2 == 0 ? -1 : 1;
        return (sign * (Mathf.Pow(progress - 1, power) + sign));
    }
}
