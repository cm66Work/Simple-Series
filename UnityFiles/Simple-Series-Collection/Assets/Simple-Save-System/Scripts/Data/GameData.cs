using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.SaveSystem
{
    [System.Serializable]
    public class GameData 
    {
        public List<FloatReference> DefaultSaveDataVariables;

        public int exampleInt;
        public float exampleFloat;

        /// <summary>
        /// the values defined in this constructor will be default values
        /// the game starts with when there's no data to load.
        /// </summary>
        public GameData()
        {
            this.exampleInt = 1;
            this.exampleFloat = 1.0f;
        }
    }
}
