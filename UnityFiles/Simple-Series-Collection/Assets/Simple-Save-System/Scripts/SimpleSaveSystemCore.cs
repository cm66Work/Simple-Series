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

            if (m_disableDataPersistence)
                Debug.LogWarning("WARNING::SimpleSaveSystemCore::Data Persistence is disabled");

            this.m_dataHandler = new FileDataHandler(Application.persistentDataPath, m_fileName);

            InitializeSelectedProfileID();
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }
        #endregion

        #region Scene Loading
        public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Gets triggered before Start().
            this.m_dataPersistenceObjects = FindAllDataPersistenceObjects();
            LoadGame();
        }
        #endregion

        [Header("Debugging")]
        [SerializeField] private bool m_initializeDataIfNull = false;
        [SerializeField] private bool m_disableDataPersistence = false;
        [SerializeField] private bool m_overrideSelectedProfileID = false;
        [SerializeField] private string m_overrideProfileID = "testID";

        [Header("File Store Config")]
        [SerializeField] private string m_fileName;

        private GameData m_gameData;
        private List<IDataPersistence> m_dataPersistenceObjects;
        private FileDataHandler m_dataHandler;

        private string m_selectedProfileID = "";

        public void DeleteProfileData(string profileID)
        {
            // delete the data from this profile ID
            m_dataHandler.Delete(profileID);
            // initialize the selected profile id in case we just deleted it.
            InitializeSelectedProfileID();
            // reload the game so that our data matches the newly selected profile ID.
            LoadGame();
        }

        private void InitializeSelectedProfileID()
        {
            this.m_selectedProfileID = m_dataHandler.GetMostRecentlyUpdatedProfileID();
            if (m_overrideSelectedProfileID)
            {
                this.m_selectedProfileID = m_overrideProfileID;
                Debug.LogWarning("WARNING::SimpleSaveSystemCore:: Overriding selected profile ID with: "
                    + m_selectedProfileID);
            }
        }

        #region New Game
        public void NewGame()
        {
            this.m_gameData = new GameData();
        }
        #endregion

        #region LoadGame
        public void LoadGame()
        {
            if (m_disableDataPersistence)
                return;

            // load any saved data from file using the data handler.
            this.m_gameData = m_dataHandler.Load(m_selectedProfileID);

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
            if (m_disableDataPersistence)
                return;

            // if we do not have may data to save, log a warning here
            if(null == this.m_gameData)
            {
                Debug.LogWarning("WARNING::SimpleSaveSystemCore:: No data was found, a New Game" +
                    "needs to be started before data can be saved.");
                return;
            }

            // pass the data to other scripts so they can update it.
            foreach (IDataPersistence dataPersistenceObject in m_dataPersistenceObjects)
            {
                dataPersistenceObject.SaveData(m_gameData);
            }

            // timestamp the data so we know when it was last saved.
            m_gameData.lastUpdated = System.DateTime.Now.ToBinary();

            // save that data to a file using the data handler.
            this.m_dataHandler.Save(m_gameData, m_selectedProfileID);
        }
        private List<IDataPersistence> FindAllDataPersistenceObjects()
        {
            // FindObjectsOfType takes in an optional boolean to include inactive gameObjects.
            IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true)
                .OfType<IDataPersistence>(); 

            return new List<IDataPersistence>(dataPersistenceObjects);
            
        }
        #endregion

        public bool HasGameData()
        {
            return m_gameData != null;
        }

        public Dictionary<string, GameData> GetAllProfilesGameData()
        {
            return m_dataHandler.LoadAllProfiles();
        }

        public void ChangeSelectedProfileID(string newProfileID)
        {
            // update the profile to use for saving and loading.
            this.m_selectedProfileID = newProfileID;
            // load the game, which will use that profile, updating our game data accordingly.
            LoadGame();
        }
    }
}
