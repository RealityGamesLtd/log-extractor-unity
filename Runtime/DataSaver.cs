using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace LogExtraction
{
    public class DataSaver
    {
        public static string GetFilePath(string dataFileName)
        {
            string tempPath = Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");
            return tempPath;
        }

        //Save Data
        public static void SaveData(string dataToSave, string dataFileName)
        {
            string tempPath = Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");

            byte[] stringByte = Encoding.ASCII.GetBytes(dataToSave);

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }

            try
            {
                File.WriteAllBytes(tempPath, stringByte);
                Debug.Log("Saved Data to: " + tempPath.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To PlayerInfo Data to: " + tempPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }
        }

        //Load Data
        public static string LoadData(string dataFileName)
        {
            string tempPath = Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");

            //Exit if Directory or File does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Debug.LogWarning("Directory does not exist");
                return "";
            }

            if (!File.Exists(tempPath))
            {
                Debug.Log("File does not exist");
                return default;
            }

            //Load saved logs
            byte[] stringByte = null;
            try
            {
                stringByte = File.ReadAllBytes(tempPath);
                Debug.Log("Loaded Data from: " + tempPath.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To Load Data from: " + tempPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }

            //Convert to json string
            return Encoding.ASCII.GetString(stringByte);
        }

        //Save Data
        public static void SaveData<T>(T dataToSave, string dataFileName)
        {
            string tempPath = Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");

            //Convert To Json then to bytes
            string jsonData = JsonUtility.ToJson(dataToSave, true);
            byte[] jsonByte = Encoding.ASCII.GetBytes(jsonData);

            //Create Directory if it does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
            }
            //Debug.Log(path);

            try
            {
                File.WriteAllBytes(tempPath, jsonByte);
                Debug.Log("Saved Data to: " + tempPath.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To PlayerInfo Data to: " + tempPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }
        }

        //Load Data
        public static T LoadData<T>(string dataFileName)
        {
            string tempPath = Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");

            //Exit if Directory or File does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Debug.LogWarning("Directory does not exist");
                return default(T);
            }

            if (!File.Exists(tempPath))
            {
                Debug.Log("File does not exist");
                return default;
            }

            //Load saved Json
            byte[] jsonByte = null;
            try
            {
                jsonByte = File.ReadAllBytes(tempPath);
                Debug.Log("Loaded Data from: " + tempPath.Replace("/", "\\"));
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To Load Data from: " + tempPath.Replace("/", "\\"));
                Debug.LogWarning("Error: " + e.Message);
            }

            //Convert to json string
            string jsonData = Encoding.ASCII.GetString(jsonByte);

            //Convert to Object
            object resultValue = JsonUtility.FromJson<T>(jsonData);
            return (T)Convert.ChangeType(resultValue, typeof(T));
        }

        public static bool DeleteData(string dataFileName)
        {
            bool success = false;

            //Load Data
            string tempPath = Path.Combine(Application.persistentDataPath, "data");
            tempPath = Path.Combine(tempPath, dataFileName + ".txt");

            //Exit if Directory or File does not exist
            if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
            {
                Debug.LogWarning("Directory does not exist");
                return false;
            }

            if (!File.Exists(tempPath))
            {
                Debug.Log("File does not exist");
                return false;
            }

            try
            {
                File.Delete(tempPath);
                Debug.Log("Data deleted from: " + tempPath.Replace("/", "\\"));
                success = true;
            }
            catch (Exception e)
            {
                Debug.LogWarning("Failed To Delete Data: " + e.Message);
            }

            return success;
        }
    }
}