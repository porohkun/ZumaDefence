using System;
using UnityEngine;

public class LayerBase : MonoBehaviour
{
    private bool _instantiated = false;

    public virtual bool Enabled
    {
        get
        {
            return gameObject.activeSelf;
        }
        set
        {
            if (gameObject.activeSelf != value)
                gameObject.SetActive(value);
        }
    }
    public virtual bool OnTop { get { return this == LayersManager.TopLayer; } }
    public virtual VisibilityType Visibility { get { return VisibilityType.Default; } }
    public virtual HidingType Hiding { get { return HidingType.Default; } }
    public virtual bool DestroyOnLoad { get { return false; } }

    public event Action FloatUp;

    public void Instantiate()
    {
        if (!_instantiated)
        {
            OnInstantiate();
            _instantiated = true;
        }
    }

    protected virtual void OnInstantiate() { }

    public virtual void OnQuit()
    {
        LayersManager.Pop();
    }

    internal virtual void OnFloatUp()
    {
        if (FloatUp != null)
            FloatUp();
    }

    public virtual bool Hide() { return false; }

    /// <summary>
    /// Тип видимости слоя. В каких случаях слой скрывается верхними слоями.
    /// </summary>
    public enum VisibilityType
    {
        /// <summary>
        /// Значение по умолчанию. Слой скрывается, если выше есть непрозрачный слой.
        /// </summary>
        Default = 0,
        /// <summary>
        /// Слой никогда не скрывается, кроме случая, когда выше есть непрозрачный слой с опцией принудительного скрывания нижних слоев.
        /// </summary>
        NeverHiding = 1,
        /// <summary>
        /// Слой скрывается всегда, если выше него есть хотя бы один другой слой.
        /// </summary>
        OnlyOnTop = -1
    }

    /// <summary>
    /// Степень прозрачности слоя. В каких случаях слой скрывает нижние слои.
    /// </summary>
    public enum HidingType
    {
        /// <summary>
        /// Значение по умолчанию. Слой скрывает все нижние слои с типом видимости "по умолчанию".
        /// </summary>
        Default = 0,
        /// <summary>
        /// Слой не скрывает нижние слои.
        /// </summary>
        Transparent = -1,
        /// <summary>
        /// Слой скрывает все нижние слои.
        /// </summary>
        HideAll = 1
    }
}