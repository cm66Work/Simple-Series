using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace com.ES.SimpleSystems.SaveSystem
{
    public class FileDataHandler 
    {
        private string m_dataDirPath = "";
        private string m_dataFileName = "";

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            m_dataDirPath = Path.Combine(dataDirPath, "SaveGames");
            this.m_dataFileName = dataFileName;
        }

        public GameData Load(string profileID)
        {
            // base case - if the profileID is null, return right away
            if (null == profileID)
                return null;

            // Use Path.Combine to account for different OS's having different path separators.
            string fullPath = Path.Combine(m_dataDirPath, profileID, m_dataFileName);

            GameData loadedData = null;
            if(!File.Exists(fullPath))
            {
                return loadedData;
            }

            try
            {
                // load the serialized data from the file.
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                // deserialize the dat from Json back into the C# object.
                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }
            catch(Exception e)
            {
                Debug.LogError("Error::FileDataHandler:: Error occurred when trying to load data from file " +
                    fullPath + "\n" + e);
            }

            return loadedData;
        }

        public void Save(GameData data, string profileID) 
        {
            // base case - if the profileID is null, return right away.
            if (null == profileID)
                return;

            // Use Path.Combine to account for different OS's having different path separators.
            string fullPath = Path.Combine(m_dataDirPath, profileID, m_dataFileName);
            try
            {
                // create the directory the file will be written to if it does not exists. 
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

                // serialize the C# game data object into Json.
                string dataToStore = JsonUtility.ToJson(data, true);

                // write the serialized data to the file.
                using(FileStream stream = new FileStream(fullPath, FileMode.Create))
                {
                    using(StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.Write(dataToStore);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error::FileDataHandler:: Error occurred when trying to save sat to file " +
                    fullPath + "\n" +  e);
            }
        }

        public void Delete(string profileID)
        {
            // base case - if the profileID is null, return right away.
            if (null == profileID) return;

            string fullPath = Path.Combine(m_dataDirPath, profileID, m_dataFileName);
            try
            {
                // ensure the data file exists at this path before deleting the directory
                if(File.Exists(fullPath))
                {
                    // delete the profile folder and everything within it
                    Directory.Delete(Path.GetDirectoryName(fullPath), true);
                }
                else
                {
                    Debug.LogWarning("Tried to delete profile data, but data was not found at path: " +
                        fullPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR::FileDataHandler:: " +
                    $"Failed to delete profile data for profileID: {profileID}" +
                    $" at path: {fullPath} \n {e} "); 
            }
        }

        /// <summary>
        /// string: profile ID
        /// GameData: the GameDataObject for that save file.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, GameData> LoadAllProfiles()
        {
            Dictionary<string, GameData> profileDirectory = new Dictionary<string, GameData>();

            // loop over all directory names in the data directory path.
            IEnumerable<DirectoryInfo> directoryInfo = new DirectoryInfo(m_dataDirPath).EnumerateDirectories();
            foreach (DirectoryInfo dirInfo in directoryInfo)
            {
                string profileID = dirInfo.Name;

                // defensive programming - check if the data file exists
                // if it does not, then this folder is not a profile and should be skipped.
                string fullPath = Path.Combine(m_dataDirPath, profileID, m_dataFileName); 
                if(!File.Exists(fullPath))
                {
                    Debug.LogWarning("WARNING::FileDataHandler::Skipping directory when loading all profiles" +
                        "because it does not contain data:" + profileID);
                    continue;
                }

                // load the game data for this profile and put it in the dictionary
                GameData profileData = Load(profileID);
                // defensive programming - ensure the profile data is not null,
                // because if it is then something went wrong and we should let ourselves know!
                if(null != profileData)
                {
                    profileDirectory.Add(profileID, profileData);
                }
                else
                {
                    Debug.LogError("ERROR::FileDataHandler:: Tried to load profile but something went wrong." +
                        "ProfileID:" + profileID);
                }
            }

            return profileDirectory;
        }

        public string GetMostRecentlyUpdatedProfileID()
        {
            string mostRecentProfileID = null;

            Dictionary<string, GameData> profilesGameData = LoadAllProfiles();
            foreach (KeyValuePair<string, GameData> pair in profilesGameData)
            {
                string profileID = pair.Key;
                GameData gameData = pair.Value;

                // skip this if the GameData is null
                if(null == gameData)
                {
                    continue;
                }

                // if this is the first data we have come across that exists, it is the most recent so far.
                if(null == mostRecentProfileID)
                {
                    mostRecentProfileID = profileID;
                }
                else // otherwise, compare to see which date is the most recent.
                {
                    DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileID].lastUpdated);
                    DateTime newDatetime = DateTime.FromBinary(gameData.lastUpdated);
                    // the greatest DateTime value is the most recent
                    if(newDatetime > mostRecentDateTime)
                    {
                        mostRecentProfileID = profileID;
                    }
                }
            }
            return mostRecentProfileID;
        }
    }
}
