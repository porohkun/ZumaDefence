using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MimiJson
{
    public class JsonObject : IEnumerable<KeyValuePair<string, JsonValue>>, IJsonLineInfo, IEquatable<JsonObject>
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

        private readonly Dictionary<string, JsonValue> _values = new Dictionary<string, JsonValue>();

        public ICollection<string> Keys { get { return _values.Keys; } }
        public ICollection<JsonValue> Values { get { return _values.Values; } }

        public JsonValue this[string key]
        {
            get { return _values[key]; }
            set { _values[key] = value; }
        }

        #region constructors

        public JsonObject()
        {

        }

        public JsonObject(IEnumerable<JOPair> args) : this()
        {
            foreach (var arg in args)
                Add(arg.Key, arg.Value);
        }

        public JsonObject(params JOPair[] args) : this((IEnumerable<JOPair>)args)
        {
        }

        #endregion

        #region values manipulating

        public bool ContainsKey(string key)
        {
            return _values.ContainsKey(key);
        }

        public void Add(string key, JsonValue value)
        {
            _values.Add(key, value);
        }

        public void Add(KeyValuePair<string, JsonValue> pair)
        {
            _values.Add(pair.Key, pair.Value);
        }

        public void Remove(string key)
        {
            if (_values.ContainsKey(key))
            {
                _values.Remove(key);
            }
        }

        public void Clear()
        {
            _values.Clear();
        }

        public JsonValue GetValue(string key)
        {
            JsonValue value;
            if (_values.TryGetValue(key, out value))
                return value;
            else return null;
        }

        public int Count
        {
            get { return _values.Count; }
        }

        #endregion

        #region parsing

        internal static JsonObject Parse(JsonReader reader)
        {
            reader.ReadEmpty();
            reader.ReadObjectStart(reader.BaseStream.Position + ": wrong token. ArrayEnd expected");
            reader.ReadEmpty();
            var result = new JsonObject();
            if (reader.CheckNext() != JsonReaderToken.ObjectEnd)
                while (true)
                {
                    reader.ReadLimiter(reader.BaseStream.Position + ": wrong token. Limiter expected");
                    var key = reader.ReadText();
                    reader.ReadLimiter(reader.BaseStream.Position + ": wrong token. Limiter expected");

                    reader.ReadEmpty();

                    reader.ReadColon(reader.BaseStream.Position + ": wrong token. Colon expected");

                    result.Add(key, JsonValue.Parse(reader));
                    reader.ReadEmpty();
                    if (reader.CheckNext() == JsonReaderToken.Separator)
                    {
                        reader.ReadSeparator();
                        reader.ReadEmpty();
                    }
                    else
                        break;
                }
            if (reader.ReadObjectEnd(reader.BaseStream.Position + ": wrong token. ObjectEnd expected"))
                return result;
            return null;
        }

        #endregion

        #region composing

        internal void Compose(JsonWriter writer)
        {
            writer.WriteObjectStart();
            if (Count > 0)
            {
                writer.IndentIncrease();
                var i = 0;
                foreach (var pair in _values)
                {
                    writer.WriteNewLine();
                    writer.WriteString(pair.Key);
                    writer.WriteColon();
                    pair.Value.Compose(writer);
                    i++;
                    if (i < Count) writer.WriteSeparator();
                }
                writer.IndentDecrease();
                writer.WriteNewLine();
            }
            writer.WriteObjectEnd();
        }

        #endregion

        public JsonObject Clone()
        {
            var result = new JsonObject();
            foreach (var keyValuePair in _values)
            {
                result.Add(keyValuePair.Key, keyValuePair.Value.Clone());
            }
            return result;
        }

        #region misc

        IEnumerator<KeyValuePair<string, JsonValue>> IEnumerable<KeyValuePair<string, JsonValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, JsonValue>>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_values).GetEnumerator();
        }

        public string DebugString
        {
            get
            {
                return "OBJECT";
            }
        }

        public bool Equals(JsonObject other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(_values, other._values);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((JsonObject)obj);
        }

        public override int GetHashCode()
        {
            return _values != null ? _values.GetHashCode() : 0;
        }

        #endregion
    }
}
