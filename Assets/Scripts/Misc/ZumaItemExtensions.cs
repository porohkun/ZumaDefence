using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ZumaItemExtensions
{
    public static IEnumerable<T> Forward<T>(this T item, bool includeThis) where T : ITwoDirections<T>
    {
        var next = includeThis ? item : item.Next;
        while (next != null)
        {
            yield return next;
            next = next.Next;
        }
    }

    public static IEnumerable<T> Backward<T>(this T item, bool includeThis) where T : ITwoDirections<T>
    {
        var preview = includeThis ? item : item.Preview;
        while (preview != null)
        {
            yield return preview;
            preview = preview.Preview;
        }
    }
}
