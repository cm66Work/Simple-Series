using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.ES.SimpleSystems.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public FloatReference test;


        private void Awake()
        {
            SimpleSaveSystemCore.instance.LoadGame();
        }

        private void OnApplicationQuit()
        {
            SimpleSaveSystemCore.instance.SaveGame();
        }
    }
}
