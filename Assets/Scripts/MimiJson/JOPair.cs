using System.Collections.Generic;

namespace MimiJson
{
    public struct JOPair
    {
        private string _key;
        private JsonValue _value;
        public string Key { get { return _key; } }
        public JsonValue Value { get { return _value; } }

        public JOPair(string key, JsonValue value)
        {
            _key = key;
            _value = value;
        }

        public static implicit operator KeyValuePair<string, JsonValue>(JOPair pair)
        {
            return new KeyValuePair<string, JsonValue>(pair.Key, pair.Value);
        }

        public static implicit operator JOPair(KeyValuePair<string, JsonValue> pair)
        {
            return new JOPair(pair.Key, pair.Value);
        }

    }


}
