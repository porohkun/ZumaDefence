using System;
using System.IO;
using System.Text;

namespace MimiJson
{
    public class JsonReader : StreamReader
    {
        #region overrided constructors

        public JsonReader(Stream stream) : base(stream) { }
        public JsonReader(Stream stream, bool detectEncodingFromByteOrderMarks) : base(stream, detectEncodingFromByteOrderMarks) { }
        public JsonReader(Stream stream, Encoding encoding) : base(stream, encoding) { }
        public JsonReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(stream, encoding, detectEncodingFromByteOrderMarks) { }
        public JsonReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }
#if NET45
        public JsonReader(Stream stream, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize, bool leaveOpen) : base(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize, leaveOpen) { }
#endif
        public JsonReader(string path) : base(path) { }
        public JsonReader(string path, bool detectEncodingFromByteOrderMarks) : base(path, detectEncodingFromByteOrderMarks) { }
        public JsonReader(string path, Encoding encoding) : base(path, encoding) { }
        public JsonReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks) : base(path, encoding, detectEncodingFromByteOrderMarks) { }
        public JsonReader(string path, Encoding encoding, bool detectEncodingFromByteOrderMarks, int bufferSize) : base(path, encoding, detectEncodingFromByteOrderMarks, bufferSize) { }

#endregion

        public JsonReaderToken CheckNext()
        {
            var next = Peek();
            if (IsEmpty(next)) return JsonReaderToken.Empty;
            if (IsLimiter(next)) return JsonReaderToken.Limiter;
            if (IsSeparator(next)) return JsonReaderToken.Separator;
            if (IsArrayStart(next)) return JsonReaderToken.ArrayStart;
            if (IsArrayEnd(next)) return JsonReaderToken.ArrayEnd;
            if (IsObjectStart(next)) return JsonReaderToken.ObjectStart;
            if (IsObjectEnd(next)) return JsonReaderToken.ObjectEnd;
            return JsonReaderToken.Other;
        }

        public void ReadEmpty()
        {
            while (IsEmpty(Peek()))
                Read();
        }

        public bool ReadLimiter(string exceptionText = null)
        {
            if (IsLimiter(Read()))
                return true;
            if (!string.IsNullOrEmpty(exceptionText))
                throw new Exception(exceptionText);
            return false;
        }

        public bool ReadSeparator(string exceptionText = null)
        {
            if (IsSeparator(Read()))
                return true;
            if (!string.IsNullOrEmpty(exceptionText))
                throw new Exception(exceptionText);
            return false;
        }

        public bool ReadColon(string exceptionText = null)
        {
            if (IsColon(Read()))
                return true;
            if (!string.IsNullOrEmpty(exceptionText))
                throw new Exception(exceptionText);
            return false;
        }

        public bool ReadArrayStart(string exceptionText = null)
        {
            if (IsArrayStart(Read()))
                return true;
            if (!string.IsNullOrEmpty(exceptionText))
                throw new Exception(exceptionText);
            return false;
        }

        public bool ReadArrayEnd(string exceptionText = null)
        {
            if (IsArrayEnd(Read()))
                return true;
            if (!string.IsNullOrEmpty(exceptionText))
                throw new Exception(exceptionText);
            return false;
        }

        public bool ReadObjectStart(string exceptionText = null)
        {
            if (IsObjectStart(Read()))
                return true;
            if (!string.IsNullOrEmpty(exceptionText))
                throw new Exception(exceptionText);
            return false;
        }

        public bool ReadObjectEnd(string exceptionText = null)
        {
            if (IsObjectEnd(Read()))
                return true;
            if (!string.IsNullOrEmpty(exceptionText))
                throw new Exception(exceptionText);
            return false;
        }

        public string ReadText()
        {
            var sb = new StringBuilder();

            var esc = false;
            var p = Peek();
            while (!(IsLimiter(p) && !esc))
            {
                if (!esc && p == 92)// '\'
                {
                    Read();
                    esc = true;
                }
                else
                {
                    var c = (char)Read();
                    if (esc)
                    {
                        if (c == 'a') sb.Append('\a');
                        else if (c == 'b') sb.Append('\b');
                        else if (c == 'f') sb.Append('\f');
                        else if (c == 'n') sb.Append('\n');
                        else if (c == 'r') sb.Append('\r');
                        else if (c == 't') sb.Append('\t');
                        else if (c == 'v') sb.Append('\v');
                        else sb.Append(c);
                        esc = false;
                    }
                    else
                        sb.Append(c);
                }
                p = Peek();
            }

            if (sb.Length == 0)
                return String.Empty;
            return sb.ToString();
        }

        public string ReadTextWithoutLimiter()
        {
            var sb = new StringBuilder();

            var p = Peek();
            while (!(IsEmpty(p) || IsSeparator(p) || IsArrayEnd(p) || IsObjectEnd(p)))
            {
                sb.Append((char)Read());
                p = Peek();
            }

            if (sb.Length == 0)
                return null;
            return sb.ToString();
        }

        private static bool IsEmpty(int value)
        {
            return value == 32 || value == 9 || value == 10 || value == 13 || value == 0 || value == 65279;
        }

        private static bool IsLimiter(int value)
        {
            return value == 34;
        }

        private static bool IsSeparator(int value)
        {
            return value == 44;
        }

        private static bool IsColon(int value)
        {
            return value == 58;
        }

        private static bool IsArrayStart(int value)
        {
            return value == 91;
        }

        private static bool IsArrayEnd(int value)
        {
            return value == 93;
        }

        private static bool IsObjectStart(int value)
        {
            return value == 123;
        }

        private static bool IsObjectEnd(int value)
        {
            return value == 125;
        }
    }
}
