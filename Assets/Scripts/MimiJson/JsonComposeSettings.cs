using System.Text;

namespace MimiJson
{
    public class JsonComposeSettings
    {
        public readonly bool Formatted;
        public readonly int Tabs;
        public readonly bool CrLn;
        public readonly Encoding Encoding;

        public JsonComposeSettings(bool formatted = true, int tabs = 4, bool crLn = true, Encoding encoding = null)
        {
            Formatted = formatted;
            Tabs = tabs;
            CrLn = crLn;
            Encoding = encoding == null ? Encoding.UTF8 : encoding;
        }

    }
}
