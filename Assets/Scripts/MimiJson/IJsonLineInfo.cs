namespace MimiJson
{
    public interface IJsonLineInfo
    {
        int KeyLineNumber { get; }
        int KeyLinePosition { get; }
        int ValueLineNumber { get; }
        int ValueLinePosition { get; }
    }
}
