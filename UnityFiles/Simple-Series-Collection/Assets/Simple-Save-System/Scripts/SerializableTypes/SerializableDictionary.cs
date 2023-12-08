using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.SaveSystem
{
    [System.Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        // I am not using the convention of m_ here to make the serialized output more human readable.
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        //  save the dictionary to list.
        public void OnAfterDeserialize()
        {
            keys.Clear();
            values.Clear();
            foreach (KeyValuePair<TKey, TValue> pare in this)
            {
                keys.Add(pare.Key);
                values.Add(pare.Value);
            }
        }

        // load the dictionary from list.
        public void OnBeforeSerialize()
        {
            this.Clear();

            if(keys.Count != values.Count) 
            {
                Debug.LogError("Tried to deserialize a SerializableDictionary, but the amount of keys ("
                    + keys.Count + ") does not match the number of values ("
                    + values.Count + ") which indicates that something went wrong!"); 
            }

            for (int i = 0; i < Keys.Count; i++)
            {
                this.Add(keys[i], values[i]);
            }
        }
    }
}
