using System.Collections;
using System.Collections.Generic;

namespace SicarioPatch.Components
{
    public class SafeDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public SafeDictionary(TValue defaultValue)
        {
            _defaultValue = defaultValue;
        }
        private readonly TValue _defaultValue;
        public new TValue this[TKey key]
        {
            get
            {
                if (key == null) return _defaultValue ?? default(TValue);
                if (!ContainsKey(key))
                {
                    this[key] = _defaultValue ?? default(TValue);
                }
                return base[key];
            }
            set => base[key] = value;
        }
    }
}