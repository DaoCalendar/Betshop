using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace premier.basic
{
    public class BidirectionalDictionary<T1, T2>
    {
        Dictionary<T1, T2> keys = new Dictionary<T1, T2>();
        Dictionary<T2, T1> values = new Dictionary<T2, T1>();

        public void Add(T1 key, T2 value)
        {
            keys.Add(key, value);
            values.Add(value, key);
        }

        public T2 GetValue(T1 key)
        {
            return keys[key];
        }

        public Dictionary<T2, T1>.KeyCollection Values
        {
            get { return values.Keys; }
        }

        public T1 GetKey(T2 value)
        {
            return values[value];
        }

        public Dictionary<T1, T2>.KeyCollection Keys
        {
            get { return keys.Keys; }
        }

        public bool ContainsKey(T1 key)
        {
            return keys.ContainsKey(key);
        }

        public bool ContainsValue(T2 value)
        {
            return values.ContainsKey(value);
        }

        public void RemoveKey(T1 key)
        {
            values.Remove(keys[key]);
            keys.Remove(key);
        }

        public void RemoveValue(T2 value)
        {
            keys.Remove(values[value]);
            values.Remove(value);
        }

        //public Dictionary<T1, T2>.KeyCollection FirstKeys
        //{
        //    get { return left.Keys; }
        //}

        //public Dictionary<T2, T1>.KeyCollection SecondKeys
        //{
        //    get { return right.Keys; }
        //}

        //public Dictionary<T1, T2> Keys
        //{
        //    get { return keys; }
        //}

        //public Dictionary<T2, T1> Values
        //{
        //    get { return values; }
        //}
    }
}
