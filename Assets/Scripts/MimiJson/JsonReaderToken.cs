namespace MimiJson
{
    public enum JsonReaderToken
    {
        Empty,
        Limiter, 
        Separator,
        Colon,
        ArrayStart,
        ArrayEnd,
        ObjectStart,
        ObjectEnd,
        Other
    }
}
