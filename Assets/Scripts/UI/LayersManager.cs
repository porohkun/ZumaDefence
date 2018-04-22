using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayersManager : MonoBehaviour
{
    public static bool Initialized { get { return _instance != null; } }
    private static LayersManager _instance;

    private static Dictionary<string, LayerBase> _instances;

    private static List<LayerBase> _stack;

    public Canvas UiCanvas;
    public RectTransform Container;
    public CanvasGroup Fader;

    public bool ShowLayersInGui = false;
    public bool _DontDestroyOnLoad = true;

    void Awake()
    {
        Fader.alpha = 1f;
        if (_instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        if (_DontDestroyOnLoad)
            DontDestroyOnLoad(this.gameObject);
        _instance = this;
        _instances = new Dictionary<string, LayerBase>();
        _stack = new List<LayerBase>();
        if (UiCanvas == null) UiCanvas = GetComponent<Canvas>();
        if (Container == null) Container = UiCanvas.transform as RectTransform;

        var preloadedLayers = new List<LayerBase>();
        for (int i = 0; i < Container.childCount; i++)
            preloadedLayers.Add(Container.GetChild(i).GetComponent<LayerBase>());
        foreach (var layer in preloadedLayers)
        {
            _instances.Add(layer.GetType().Name, layer);
            layer.Instantiate();
            layer.gameObject.SetActive(false);
        }
        if (preloadedLayers.Count > 0)
            Push(preloadedLayers[0]);
    }

    #region Fading

    public static void FadeIn(float fadeTime, Action afterFade)
    {
        _instance.StartCoroutine(_instance.FadeRoutine(1f, 0f, fadeTime, afterFade));
    }

    public static void FadeOut(float fadeTime, Action afterFade)
    {
        _instance.StartCoroutine(_instance.FadeRoutine(0f, 1f, fadeTime, afterFade));
    }

    private IEnumerator FadeRoutine(float from, float to, float duration, Action finalAction)
    {
        Fader.blocksRaycasts = true;
        float time = Time.realtimeSinceStartup;
        Fader.alpha = from;
        while (Time.realtimeSinceStartup < time + duration)
        {
            Fader.alpha = Mathf.Lerp(from, to, (Time.realtimeSinceStartup - time) / duration);
            yield return new WaitForEndOfFrame();
        }
        Fader.alpha = to;
        Fader.blocksRaycasts = to != 0f;
        if (finalAction != null)
            finalAction();
    }

    #endregion

    public static TLayer GetLayer<TLayer>() where TLayer : LayerBase
    {
        var layerName = typeof(TLayer).Name;
        if (_instances.ContainsKey(layerName))
            return _instances[layerName] as TLayer;

        return InstantiateFromPrefab((TLayer)Resources.Load(string.Format("Layers/{0}", layerName), typeof(TLayer))) as TLayer;
    }

    public static LayerBase InstantiateFromPrefab(LayerBase prefab)
    {
        var layerName = prefab.GetType().Name;
        var instance = Instantiate(prefab);
        if (!instance.DestroyOnLoad)
            DontDestroyOnLoad(instance);
        instance.name = layerName;
        var rt = instance.transform as RectTransform;
        rt.SetParent(LayersManager._instance.Container);
        rt.localScale = Vector3.one;
        instance.transform.position = Vector3.zero;
        rt.anchoredPosition = Vector3.zero;
        rt.sizeDelta = Vector3.zero;

        instance.Enabled = false;
        instance.Instantiate();
        if (_instances.ContainsKey(layerName))
            _instances.Remove(layerName);
        _instances.Add(layerName, instance);
        return instance;
    }

    public static bool HaveLayer<TLayer>()
    {
        var layerName = typeof(TLayer).Name;
        return _instances.ContainsKey(layerName);
    }

    public static void DestroyLayer(LayerBase layer)
    {
        var layerName = layer.GetType().Name;
        if (_instances.ContainsKey(layerName))
        {
            Pop(layer);
            _instances.Remove(layerName);
        }
        GameObject.Destroy(layer.gameObject);
    }

    public static LayerBase TopLayer { get { if (_stack.Count > 0) return _stack.Last(); else return null; } }

    private void Push(LayerBase layer, bool withSwitch)
    {
        Pop(layer);
        _stack.Add(layer);
        layer.transform.SetAsLastSibling();
        if (withSwitch)
            SwitchLayersActivity();
    }

    public static void Push(LayerBase layer)
    {
        _instance.Push(layer, true);
    }

    public static TLayer Push<TLayer>() where TLayer : LayerBase
    {
        var layer = GetLayer<TLayer>();
        Push(layer);
        return layer;
    }

    private LayerBase Pop(bool withSwitch)
    {
        var count = _stack.Count;
        LayerBase result = null;
        if (count > 1)
        {
            result = _stack[count - 1];
            result.Enabled = false;
            _stack.RemoveAt(count - 1);
        }
        if (withSwitch)
            SwitchLayersActivity();
        return result;
    }

    public static LayerBase Pop()
    {
        return _instance.Pop(true);
    }

    internal static void Hide(bool val)
    {
        _instance.gameObject.SetActive(!val);
    }

    public static void Pop(LayerBase layer)
    {
        if (_stack != null && _stack.Contains(layer))
        {
            layer.Enabled = false;
            _stack.Remove(layer);
            _instance.SwitchLayersActivity();
        }
    }

    public static void Pop<TLayer>() where TLayer : LayerBase
    {
        LayerBase layerForRemove = null;
        if (_stack == null) return;
        foreach (var layer in _stack)
        {
            if (layer.GetType() == typeof(TLayer))
            {
                layerForRemove = layer;
                break;
            }
        }
        if (layerForRemove != null)
            Pop(layerForRemove);
    }

    public static void PopTill(LayerBase layer, bool destroy = false, bool include = false)
    {
        while (_stack.Count > 1)
        {
            var pLayer = _instance.Pop(false);
            if (pLayer != null)
            {
                if (pLayer == layer)
                {
                    if (!include)
                        _instance.Push(pLayer, false);
                    else
                        DestroyLayer(pLayer);
                    break;
                }
                else
                    DestroyLayer(pLayer);
            }
        }
        _instance.SwitchLayersActivity();
    }

    public static void PopTill<TLayer>(bool destroy = false, bool include = false) where TLayer : LayerBase
    {
        while (_stack.Count > 1)
        {
            var layer = _instance.Pop(false);
            if (layer != null)
            {
                if (layer.GetType() == typeof(TLayer))
                {
                    if (!include)
                        _instance.Push(layer, false);
                    else
                        DestroyLayer(layer);
                    break;
                }
                else
                    DestroyLayer(layer);
            }
        }
        _instance.SwitchLayersActivity();
    }

    public static void Top(LayerBase layer)
    {
        if (_stack.Contains(layer))
        {
            _stack.Remove(layer);
            Push(layer);
        }
    }

    public static void Top<TLayer>() where TLayer : LayerBase
    {
        LayerBase layerForTop = null;
        foreach (var layer in _stack)
        {
            if (layer.GetType() == typeof(TLayer))
            {
                layerForTop = layer;
                break;
            }
        }
        if (layerForTop != null)
            Top(layerForTop);
    }


    public static bool Contains<TLayer>() where TLayer : LayerBase
    {
        foreach (var layer in _stack)
            if (layer.GetType() == typeof(TLayer))
                return true;
        return false;
    }

    private void SwitchLayersActivity()
    {
        int hiding = -1;

        for (int i = _stack.Count - 1; i >= 0; i--)
        {
            var layer = _stack[i];
            if (layer.OnTop)
                layer.Enabled = true;
            else
                layer.Enabled = (int)layer.Visibility > hiding;
            hiding = Math.Max(hiding, (int)layer.Hiding);
        }
        if (TopLayer != null)
        {
            TopLayer.OnFloatUp();
        }
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (ShowLayersInGui)
            for (int i = 0; i < _stack.Count; i++)
            {
                GUI.Label(new Rect(Screen.width - 150, Screen.height - 25 * (_stack.Count - i), 150, 25), _stack[i].GetType().Name);
            }
    }
#endif
}
