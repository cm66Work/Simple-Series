using UnityEngine;
using System;
using System.IO;

namespace com.ES.SimpleSystems.SaveSystem
{
    public class FileDataHandler 
    {
        private string m_dataDirPath = "";
        private string dataFileName = "";

        public FileDataHandler(string dataDirPath, string dataFileName)
        {
            m_dataDirPath = dataDirPath;
            this.dataFileName = dataFileName;
        }

        public GameData Load()
        {
            // Use Path.Combine to account for different OS's having different path separators.
            string fullPath = Path.Combine(m_dataDirPath, dataFileName);
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

        public void Save(GameData data) 
        {
            // Use Path.Combine to account for different OS's having different path separators.
            string fullPath = Path.Combine(m_dataDirPath, dataFileName);
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
    }
}
