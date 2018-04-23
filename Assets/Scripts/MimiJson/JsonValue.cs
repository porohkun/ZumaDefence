using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

namespace MimiJson
{
    public class JsonValue : IEnumerable<JsonValue>, IEnumerable<KeyValuePair<string, JsonValue>>, IJsonLineInfo, IEquatable<JsonValue>
    {
        #region IJsonLineInfo

        private int _keyLineNumber;
        private int _keyLinePosition;
        private int _valueLineNumber;
        private int _valueLinePosition;
        int IJsonLineInfo.KeyLineNumber { get { return _keyLineNumber; } }
        int IJsonLineInfo.KeyLinePosition { get { return _keyLinePosition; } }
        int IJsonLineInfo.ValueLineNumber { get { return _valueLineNumber; } }
        int IJsonLineInfo.ValueLinePosition { get { return _valueLinePosition; } }

        #endregion

        private JsonObject _object;
        private JsonArray _array;

        public JsonValueType Type { get; private set; }
        public string String { get; set; }
        public double Number { get; set; }
        public JsonObject Object { get { return _object; } }
        public JsonArray Array { get { return _array; } }
        public bool Boolean { get; set; }

        public JsonValue this[int i]
        {
            get
            {
                if (Type != JsonValueType.Array)
                    throw GetImplisitException(JsonValueType.Array, Type);
                return Array[i];
            }
        }

        public JsonValue this[string name]
        {
            get
            {
                if (Type != JsonValueType.Object)
                    throw GetImplisitException(JsonValueType.Object, Type);
                return Object[name];
            }
        }

        #region constructors

        public JsonValue()
        {
            Type = JsonValueType.Null;
        }

        public JsonValue(JsonValueType type)
        {
            Type = type;
            if (type == JsonValueType.String)
            {
                String = "";
            }
            else if (type == JsonValueType.Number)
            {
                Number = 0;
            }
            else if (type == JsonValueType.Boolean)
            {
                Boolean = false;
            }
            else if (type == JsonValueType.Object)
            {
                _object = new JsonObject();
            }
            else if (type == JsonValueType.Array)
            {
                _array = new JsonArray();
            }
        }

        public JsonValue(string text)
        {
            if (text == null)
                Type = JsonValueType.Null;
            else
            {
                Type = JsonValueType.String;
                String = text;
            }
        }

        public JsonValue(double number)
        {
            if (double.IsNaN(number) || double.IsInfinity(number))
                Type = JsonValueType.Null;
            else
            {
                Type = JsonValueType.Number;
                Number = number;
            }
        }

        public JsonValue(bool boolean)
        {
            Type = JsonValueType.Boolean;
            Boolean = boolean;
        }

        public JsonValue(JsonObject obj)
        {
            if (obj == null)
                Type = JsonValueType.Null;
            else
            {
                Type = JsonValueType.Object;
                _object = obj;
            }
        }

        public JsonValue(JsonArray array)
        {
            if (array == null)
                Type = JsonValueType.Null;
            else
            {
                Type = JsonValueType.Array;
                _array = array;
            }
        }

        #endregion

        #region implisit to JsonValue

        public static implicit operator JsonValue(string value)
        {
            return new JsonValue(value);
        }

        public static implicit operator JsonValue(double number)
        {
            return new JsonValue(number);
        }

        public static implicit operator JsonValue(JsonObject obj)
        {
            return new JsonValue(obj);
        }

        public static implicit operator JsonValue(JsonArray array)
        {
            return new JsonValue(array);
        }

        public static implicit operator JsonValue(bool boolean)
        {
            return new JsonValue(boolean);
        }

        #endregion

        #region implisit from JsonValue

        private static InvalidCastException GetImplisitException(JsonValueType tryType, JsonValueType realType)
        {
            return new InvalidCastException(String.Concat("Wrong type '", tryType.ToString(), "'. Real type is '", realType.ToString(), "'."));
        }

        public static implicit operator double(JsonValue value)
        {
            if (value.Type != JsonValueType.Number)
                throw GetImplisitException(JsonValueType.Number, value.Type);
            return value.Number;
        }

        public static implicit operator float(JsonValue value)
        {
            if (value.Type != JsonValueType.Number)
                throw GetImplisitException(JsonValueType.Number, value.Type);
            return (float)value.Number;
        }

        public static implicit operator decimal(JsonValue value)
        {
            if (value.Type != JsonValueType.Number)
                throw GetImplisitException(JsonValueType.Number, value.Type);
            return (decimal)value.Number;
        }

        public static implicit operator byte(JsonValue value)
        {
            if (value.Type != JsonValueType.Number)
                throw GetImplisitException(JsonValueType.Number, value.Type);
            return (byte)value.Number;
        }

        public static implicit operator int(JsonValue value)
        {
            if (value.Type != JsonValueType.Number)
                throw GetImplisitException(JsonValueType.Number, value.Type);
            return (int)value.Number;
        }

        public static implicit operator long(JsonValue value)
        {
            if (value.Type != JsonValueType.Number)
                throw GetImplisitException(JsonValueType.Number, value.Type);
            return (long)value.Number;
        }

        public static implicit operator string(JsonValue value)
        {
            if (value.Type != JsonValueType.String)
                throw GetImplisitException(JsonValueType.String, value.Type);
            return value.String;
        }

        public static implicit operator JsonObject(JsonValue value)
        {
            if (value.Type != JsonValueType.Object)
                throw GetImplisitException(JsonValueType.Object, value.Type);
            return value.Object;
        }

        public static implicit operator JsonArray(JsonValue value)
        {
            if (value.Type != JsonValueType.Array)
                throw GetImplisitException(JsonValueType.Array, value.Type);
            return value.Array;
        }

        public static implicit operator bool(JsonValue value)
        {
            if (value.Type != JsonValueType.Boolean)
                throw GetImplisitException(JsonValueType.Boolean, value.Type);
            return value.Boolean;
        }

        #endregion

        #region parsing

        public static JsonValue Parse(string jString, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            if (!jString.IsNullOrWhiteSpace())
                using (var stream = new MemoryStream(encoding.GetBytes(jString)))
                    return Parse(stream, encoding);
            return new JsonValue(JsonValueType.Null);
        }

        public static JsonValue Parse(Stream stream, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            using (var reader = new JsonReader(stream, encoding))
                return Parse(reader);
        }

        public static JsonValue ParseFile(string path, Encoding encoding = null)
        {
            if (System.IO.File.Exists(path))
                using (var reader = new JsonReader(path, encoding == null ? Encoding.UTF8 : encoding))
                    return Parse(reader);
            return new JsonValue(JsonValueType.Null);
        }

        internal static JsonValue Parse(JsonReader reader)
        {
            reader.ReadEmpty();
            var next = reader.CheckNext();

            if (next == JsonReaderToken.ObjectStart)
                return new JsonValue(JsonObject.Parse(reader));

            if (next == JsonReaderToken.ArrayStart)
                return new JsonValue(JsonArray.Parse(reader));

            if (next == JsonReaderToken.Limiter)
            {
                reader.ReadLimiter(reader.BaseStream.Position + ": wrong token. Limiter expected");
                var text = reader.ReadText();
                reader.ReadLimiter(reader.BaseStream.Position + ": wrong token. Limiter expected");
                return new JsonValue(text);
            }
            else
            {
                var text = reader.ReadTextWithoutLimiter().ToLowerInvariant();
                if (text == "null")
                    return new JsonValue();
                double d;
                if (Double.TryParse(text, NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out d))
                    return new JsonValue(d);
                bool b;
                if (Boolean.TryParse(text, out b))
                    return new JsonValue(b);
                throw new Exception(reader.BaseStream.Position + ": wrong token. Value expected");
            }
        }

        #endregion

        #region composing

        public override string ToString()
        {
            return ToString(new JsonComposeSettings());
        }

        public string ToString(JsonComposeSettings settings)
        {
#if NET45
            using (var stream = new MemoryStream())
#else
            using (var stream = new MyMemoryStream())
#endif
            {
                ToStream(stream, settings);
                stream.Position = 0;
                var result = settings.Encoding.GetString(stream.ToArray());
#if !NET45
                stream.ReallyClose();
#endif
                return result;
            }
        }

        public byte[] ToBytes(JsonComposeSettings settings)
        {
#if NET45
            using (var stream = new MemoryStream())
#else
            using (var stream = new MyMemoryStream())
#endif
            {
                ToStream(stream, settings);
                stream.Position = 0;
                var bytes = stream.ToArray();
#if !NET45
                stream.ReallyClose();
#endif
                return bytes;
            }
        }

        private void ToStream(Stream stream, JsonComposeSettings settings)
        {
#if NET45
            using (var writer = new JsonWriter(stream, settings, 1024, true))
            {
                Compose(writer);
            }
#else
            using (var writer = new JsonWriter(stream, settings))
            {
                Compose(writer);
            }
#endif
        }

        public void ToFile(string path, JsonComposeSettings settings = null)
        {
            using (var writer = new JsonWriter(path, settings == null ? new JsonComposeSettings() : settings))
            {
                Compose(writer);
            }
        }

        internal void Compose(JsonWriter writer)
        {
            if (Type == JsonValueType.Object)
            {
                Object.Compose(writer);
            }
            else if (Type == JsonValueType.Array)
            {
                Array.Compose(writer);
            }
            else if (Type == JsonValueType.Boolean)
            {
                writer.WriteBoolean(Boolean);
            }
            else if (Type == JsonValueType.Number)
            {
                writer.WriteNumber(Number);
            }
            else if (Type == JsonValueType.String)
            {
                writer.WriteString(String);
            }
            else if (Type == JsonValueType.Null)
            {
                writer.WriteNull(Number);
            }
            //writer.WriteNewLine();
        }

        #endregion

        public JsonValue Clone()
        {
            var result = new JsonValue(Type);
            switch (Type)
            {
                case JsonValueType.String:
                    result.String = String;
                    break;

                case JsonValueType.Boolean:
                    result.Boolean = Boolean;
                    break;

                case JsonValueType.Number:
                    result.Number = Number;
                    break;

                case JsonValueType.Object:
                    result._object = Object.Clone();
                    break;

                case JsonValueType.Array:
                    result._array = Array.Clone();
                    break;
            }
            return result;
        }

        #region misc

        public IEnumerator<JsonValue> GetEnumerator()
        {
            return ((IEnumerable<JsonValue>)Array).GetEnumerator();
        }

        IEnumerator<KeyValuePair<string, JsonValue>> IEnumerable<KeyValuePair<string, JsonValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, JsonValue>>)Object).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            if (Type == JsonValueType.Array)
                return ((IEnumerable)Array).GetEnumerator();
            if (Type == JsonValueType.Object)
                return ((IEnumerable)Object).GetEnumerator();
            return new EmptyEnumerator();
        }

        public string DebugString
        {
            get
            {
                switch (Type)
                {
                    case JsonValueType.Null:
                        return "NULL";
                    case JsonValueType.Array:
                        return "ARRAY";
                    case JsonValueType.Boolean:
                        return Boolean.ToString().ToUpper();
                    case JsonValueType.Number:
                        return Number.ToString();
                    case JsonValueType.String:
                        return String;
                    case JsonValueType.Object:
                        return "OBJECT";
                    default:
                        return "UNKNOWN";
                }
            }
        }

        public bool Equals(JsonValue other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (Type != other.Type)
                return false;
            if (Type == JsonValueType.String)
                return string.Equals(String, other.String);
            if (Type == JsonValueType.Number)
                return Number.Equals(other.Number);
            if (Type == JsonValueType.Boolean)
                return Boolean == other.Boolean;
            if (Type == JsonValueType.Object)
                return Object.Equals(other.Object);
            if (Type == JsonValueType.Array)
                return Array.Equals(other.Array);
            if (Type == JsonValueType.Null)
                return true;
            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((JsonValue)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)Type;

                if (Type == JsonValueType.String)
                    hashCode = (hashCode * 397) ^ (String != null ? String.GetHashCode() : 0);
                else if (Type == JsonValueType.Number)
                    hashCode = (hashCode * 397) ^ Number.GetHashCode();
                else if (Type == JsonValueType.Boolean)
                    hashCode = (hashCode * 397) ^ Boolean.GetHashCode();
                else if (Type == JsonValueType.Object)
                    hashCode = (hashCode * 397) ^ Object.GetHashCode();
                else if (Type == JsonValueType.Array)
                    hashCode = (hashCode * 397) ^ Array.GetHashCode();

                return hashCode;
            }
        }

        #endregion
    }
}
