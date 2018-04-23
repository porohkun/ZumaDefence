using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace MimiJson
{
    public class JsonArray : IEnumerable<JsonValue>, IJsonLineInfo, IEquatable<JsonArray>
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

        private readonly List<JsonValue> _values = new List<JsonValue>();

        public int Length
        {
            get { return _values.Count; }
        }

        public JsonValue this[int index]
        {
            get { return _values[index]; }
            set { _values[index] = value; }
        }

        #region constructors

        public JsonArray()
        {
        }

        public JsonArray(IEnumerable<JsonValue> args) : this()
        {
            foreach (var arg in args)
                Add(arg);
        }

        public JsonArray(params JsonValue[] args) : this((IEnumerable<JsonValue>)args)
        {
        }

        #endregion

        #region values manipulating

        public void Add(JsonValue value)
        {
            _values.Add(value);
        }

        public void Remove(int index)
        {
            _values.RemoveAt(index);
        }

        public void Clear()
        {
            _values.Clear();
        }

        public static JsonArray operator +(JsonArray lhs, JsonArray rhs)
        {
            var result = lhs.Clone();
            foreach (var value in rhs._values)
            {
                result.Add(value.Clone());
            }
            return result;
        }

        #endregion

        #region parsing

        internal static JsonArray Parse(JsonReader reader)
        {
            reader.ReadEmpty();
            reader.ReadArrayStart(reader.BaseStream.Position + ": wrong token. ArrayStart expected");
            reader.ReadEmpty();
            var result = new JsonArray();
            if (reader.CheckNext() != JsonReaderToken.ArrayEnd)
                while (true)
                {
                    result.Add(JsonValue.Parse(reader));
                    reader.ReadEmpty();
                    if (reader.CheckNext() == JsonReaderToken.Separator)
                    {
                        reader.ReadSeparator();
                        reader.ReadEmpty();
                    }
                    else
                        break;
                }
            if (reader.ReadArrayEnd(reader.BaseStream.Position + ": wrong token. ArrayEnd expected"))
                return result;
            return null;
        }

        #endregion

        #region composing

        internal void Compose(JsonWriter writer)
        {
            writer.WriteArrayStart();
            if (Length > 0)
            {
                writer.IndentIncrease();
                var i = 0;
                foreach (var value in _values)
                {
                    writer.WriteNewLine();
                    value.Compose(writer);
                    i++;
                    if (i < Length) writer.WriteSeparator();
                }
                writer.IndentDecrease();
                writer.WriteNewLine();
            }
            writer.WriteArrayEnd();
        }

        #endregion

        public JsonArray Clone()
        {
            var result = new JsonArray();
            foreach (var v in _values)
            {
                result.Add(v.Clone());
            }
            return result;
        }

        #region misc

        IEnumerator<JsonValue> IEnumerable<JsonValue>.GetEnumerator()
        {
            return ((IEnumerable<JsonValue>)_values).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_values).GetEnumerator();
        }

        public string DebugString
        {
            get
            {
                return "ARRAY";
            }
        }

        public bool Equals(JsonArray other)
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
            return Equals((JsonArray)obj);
        }

        public override int GetHashCode()
        {
            return (_values != null ? _values.GetHashCode() : 0);
        }

        #endregion
    }
}
