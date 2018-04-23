
public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string value)
    {
        if (value == null)
            return true;
        for (int index = 0; index < value.Length; ++index)
        {
            if (!char.IsWhiteSpace(value[index]))
                return false;
        }
        return true;
    }
}
