using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    public static Vector3 RoundToInt(this Vector3 vector)
    {
        return new Vector3(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), Mathf.RoundToInt(vector.z));
    }

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        return RotateRadians(v, degrees * Mathf.Deg2Rad);
    }

    public static Vector2 RotateRadians(this Vector2 v, float radians)
    {
        var ca = Mathf.Cos(radians);
        var sa = Mathf.Sin(radians);
        return new Vector2(ca * v.x - sa * v.y, sa * v.x + ca * v.y);
    }

    /// <summary>
    /// Поэлементное перемножение векторов
    /// </summary>
    public static Vector2 ScaleBy(this Vector2 vector, Vector2 other)
    {
        return new Vector2(vector.x * other.x, vector.y * other.y);
    }

    /// <summary>
    /// Поэлементное перемножение векторов
    /// </summary>
    public static Vector3 ScaleBy(this Vector3 vector, Vector3 other)
    {
        return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
    }

    public static Vector2 DivideBy(this Vector2 v1, Vector2 v2)
    {
        return new Vector2(v1.x / v2.x, v1.y / v2.y);
    }

    public static float DistanceTo(this Vector2 from, Vector2 to)
    {
        return Vector2.Distance(from, to);
    }

    public static float DistanceTo(this Vector3 from, Vector3 to)
    {
        return Vector3.Distance(from, to);
    }

    /// <summary>
    /// Возвращает положение объекта в мировых координатах, с учётом его центра.
    /// </summary>
    /// <param name="item">Объект, для когорого будет расчитываться положение</param>    
    public static Vector3 CenterPosition(this RectTransform item)
    {
        var corners = new Vector3[4];
        item.GetWorldCorners(corners);
        var offset = corners[2] - corners[0];
        return item.position + offset.ScaleBy(Vector2.one / 2f - item.pivot);
    }

    public static Vector2 CenterPositionRelativeTo(this RectTransform item, RectTransform target)
    {
        return (Vector2)target.InverseTransformPoint(item.position) + (Vector2.one / 2f - item.pivot).ScaleBy(item.rect.size);
    }

    public static Vector2 CenterPositionRelativeTo(this Vector3 position, RectTransform target)
    {
        return (Vector2)target.InverseTransformPoint(position);
    }

    public static Vector3 AddZ(this Vector2 vector, float z)
    {
        return new Vector3(vector.x, vector.y, z);
    }

    public static IEnumerable<T> Enumerate<T>(this T[,] array)
    {
        for (int x = 0; x < array.GetLength(0); x++)
            for (int y = 0; y < array.GetLength(1); y++)
                yield return array[x, y];
    }
}
