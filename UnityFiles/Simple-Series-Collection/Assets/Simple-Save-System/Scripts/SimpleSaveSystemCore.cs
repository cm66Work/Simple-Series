using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;

namespace com.ES.SimpleSystems.SaveSystem
{
    public class SimpleSaveSystemCore : MonoBehaviour
    {
        #region life cycle
        public static SimpleSaveSystemCore instance { get; private set; }

        public void Awake()
        {
            if (instance != null)
                Debug.LogError("More then one instance of SimpleSaveSystemCore already exists in this scene");
            instance = this;
            DontDestroyOnLoad(this.gameObject);

            this.m_dataHandler = new FileDataHandler(Application.persistentDataPath, m_fileName);
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Gets triggered before Start().
            this.m_dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
        public void OnSceneUnloaded(Scene scene)
        {
            SaveGame();
        }
        private void OnApplicationQuit()
        {
            SaveGame();
        }
        #endregion

        [Header("Debugging")]
        [SerializeField] private bool m_initializeDataIfNull = false;

        [Header("File Store Config")]
        [SerializeField] private string m_fileName;

        private GameData m_gameData;
        private List<IDataPersistence> m_dataPersistenceObjects;
        private FileDataHandler m_dataHandler;

        #region New Game
        public void NewGame()
        {
            this.m_gameData = new GameData();
        }
        #endregion

        #region LoadGame
        public void LoadGame()
        {
            // load any saved data from file using the data handler.
            this.m_gameData = m_dataHandler.Load();

            // start a new game if the data is null AND we are configured to initialize data for debugging purposes.
            if(null == this.m_gameData && m_initializeDataIfNull)
            {
                NewGame();
            }

            // if no data can be loaded, initialize to a new game.
            if(this.m_gameData == null)
            {
                Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
                return;
            }
            // push the loaded data to all other scripts that need it.
            foreach (IDataPersistence dataPersistenceObjects in m_dataPersistenceObjects)
            {
                dataPersistenceObjects.LoadData(m_gameData);
            }
        }
        #endregion

        #region SaveGame
        public void SaveGame()
        {
            // pass the data to other scripts so they can update it.
            foreach (IDataPersistence dataPersistenceObject in m_dataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(ref m_gameData);
            }

            // save that data to a file using the data handler.
            this.m_dataHandler.Save(m_gameData);
        }
        #endregion

        public bool HasGameData()
        {
            return m_gameData != null;
        }

        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>()
                .OfType<IDataPersistence>(); 

            return new List<IDataPersistence>(dataPersistenceObjects);
            
        }
    }
}
