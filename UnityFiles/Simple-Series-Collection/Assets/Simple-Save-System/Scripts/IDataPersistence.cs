using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.SaveSystem
{
    public interface IDataPersistence 
    {
        void LoadData(GameData data);
        
        // using ref to allow for data modification on not creating of duplicate data.
        void SaveData(GameData data);
    }
}
