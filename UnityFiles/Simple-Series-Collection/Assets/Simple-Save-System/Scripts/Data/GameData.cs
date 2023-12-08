using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.SaveSystem
{
    [System.Serializable]
    public class GameData 
    {
        public int exampleInt;
        public float exampleFloat;

        public SerializableDictionary<string, bool> exampleDictionary;

        /// <summary>
        /// the values defined in this constructor will be default values
        /// the game starts with when there's no data to load.
        /// </summary>
        public GameData()
        {
            this.exampleInt = 1;
            this.exampleFloat = 1.0f;
            this.exampleDictionary = new SerializableDictionary<string, bool>();
            this.exampleDictionary.Add("test", false);
        }
    }
}
