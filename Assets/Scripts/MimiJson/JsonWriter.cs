using System;
using System.IO;
using System.Text;

namespace MimiJson
{
    public class JsonWriter : StreamWriter
    {
        #region overrided constructors

        public JsonWriter(Stream stream) : base(stream) { _settings = new JsonComposeSettings(); }
        public JsonWriter(Stream stream, Encoding encoding) : base(stream, encoding) { _settings = new JsonComposeSettings(); }
        public JsonWriter(Stream stream, Encoding encoding, int bufferSize) : base(stream, encoding, bufferSize) { _settings = new JsonComposeSettings(); }
#if NET45
        public JsonWriter(Stream stream, Encoding encoding, int bufferSize, bool leaveOpen) : base(stream, encoding, bufferSize, leaveOpen) { _settings = new JsonComposeSettings(); }
#endif
        public JsonWriter(string path) : base(path) { _settings = new JsonComposeSettings(); }
        public JsonWriter(string path, bool append) : base(path, append) { _settings = new JsonComposeSettings(); }
        public JsonWriter(string path, bool append, Encoding encoding) : base(path, append, encoding) { _settings = new JsonComposeSettings(); }
        public JsonWriter(string path, bool append, Encoding encoding, int bufferSize) : base(path, append, encoding, bufferSize) { _settings = new JsonComposeSettings(); }

        #endregion

        private JsonComposeSettings _settings;
        private int _indent = 0;
        private readonly string _null = "null";
        private readonly string _true = "true";
        private readonly string _false = "false";
        private readonly string _bslash = @"\\";
        private readonly string _slash = @"\/";
        private readonly string _a = @"\a";
        private readonly string _b = @"\b";
        private readonly string _f = @"\f";
        private readonly string _n = @"\n";
        private readonly string _r = @"\r";
        private readonly string _t = @"\t";
        private readonly string _v = @"\v";
        private readonly string _comma = @"\'";
        private readonly string _dcomma = @"\""";

        public JsonWriter(Stream stream, JsonComposeSettings settings) : base(stream, settings.Encoding)
        {
            _settings = settings;
        }
#if NET45
        public JsonWriter(Stream stream, JsonComposeSettings settings, int bufferSize, bool leaveOpen) : base(stream, settings.Encoding, bufferSize, leaveOpen)
        {
            _settings = settings;
        }
#endif
        public JsonWriter(string path, JsonComposeSettings settings) : base(path, false, settings.Encoding)
        {
            _settings = settings;
        }

        public JsonWriter(string path, JsonComposeSettings settings, bool append, int bufferSize) : base(path, append, settings.Encoding, bufferSize)
        {
            _settings = new JsonComposeSettings();
        }
        
        public void WriteBoolean(bool boolean)
        {
            Write(boolean ? _true : _false);
        }

        public void WriteNumber(double number)
        {
            Write(number.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        public void WriteString(string text)
        {
            WriteLimiter();
            foreach (var c in text)
            {
                if (c == '\\') Write(_bslash);
                else if (c == '/') Write(_slash);
                else if (c == '\a') Write(_a);
                else if (c == '\b') Write(_b);
                else if (c == '\f') Write(_f);
                else if (c == '\n') Write(_n);
                else if (c == '\r') Write(_r);
                else if (c == '\t') Write(_t);
                else if (c == '\v') Write(_v);
                else if (c == '\'') Write(_comma);
                else if (c == '\"') Write(_dcomma);
                else Write(c);
            }
            WriteLimiter();
        }

        public void WriteNull(double number)
        {
            Write(_null);
        }

        public void IndentIncrease()
        {
            _indent++;
        }

        public void IndentDecrease()
        {
            _indent--;
        }

        public void WriteNewLine()
        {
            if (!_settings.Formatted) return;
            if (_settings.CrLn)
                Write('\r');
            Write('\n');
            for (int i = 0; i < _indent * _settings.Tabs; i++)
                Write(' ');
        }

        public void WriteLimiter()
        {
            Write('"');
        }

        public void WriteSeparator()
        {
            Write(',');
        }

        public void WriteColon()
        {
            Write(':');
            if (_settings.Formatted)
                Write(' ');
        }

        public void WriteArrayStart()
        {
            Write('[');
        }

        public void WriteArrayEnd()
        {
            Write(']');
        }

        public void WriteObjectStart()
        {
            Write('{');
        }

        public void WriteObjectEnd()
        {
            Write('}');
        }
    }
}
