using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace com.ES.SimpleSystems.SaveSystem
{
    public class SimpleSaveSystemCore
    {
        public static SimpleSaveSystemCore instance { get; private set; }

        public void Awake()
        {
            if (instance != null)
                Debug.LogError("More then one instance of SimpleSaveSystemCore already exists in this scene");
            instance = this;
        }


        private GameData m_gameData;

        #region New Game
        public void NewGame()
        {
            this.m_gameData = new GameData();
        }
        #endregion

        #region LoadGame
        public void LoadGame()
        {
            // TODO - load any saved data from file using the data handler.
            // if no data can be loaded, initialize to a new game.
            if(this.m_gameData == null)
            {
                Debug.Log("No data was found. Initializing data to defaults.");
                NewGame();
            }
            // TODO - push the loaded data to all other scripts that need it.
        }
        #endregion

        #region SaveGame
        public void SaveGame()
        {
            // TODO - pass the data to other scripts so they can update it.

            // TODO - save that data to a file using the data handler.
        }
        #endregion




    }
}
