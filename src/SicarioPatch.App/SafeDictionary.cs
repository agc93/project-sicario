using System.Collections;
using System.Collections.Generic;

namespace SicarioPatch.App
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
                if (!this.ContainsKey(key))
                {
                    this[key] = _defaultValue ?? default(TValue);
                }
                return base[key];
            }
            set => base[key] = value;
        }
    }
}