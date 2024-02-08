using Data;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
    const string DATA_PATH = "Results";
    const string FILE_NAME = "GameData.json";

    [MenuItem("Debug/Clear Data")]
    static void ClearData()
    {
        string dataPath = Application.persistentDataPath + "/" + DATA_PATH + "/" + FILE_NAME;
        if (File.Exists(dataPath))
        {
            File.Delete(dataPath);
        }
        Debug.Log("Data Cleared");
    }

    [MenuItem("Debug/Show Data")]
    static void OutputData()
    {
        string dataPath = Application.persistentDataPath + "/" + DATA_PATH + "/" + FILE_NAME;
        if (File.Exists(dataPath))
        {
            StreamReader sr = new StreamReader(dataPath);
            Debug.Log(sr.ReadToEnd());
            sr.Close();
        }
        else
        {
            Debug.Log("No Data");
        }
    }
}
