using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Configuration;

public class ConfigManager:MonoBehaviour
{
    private const string CONFIG_LOCATION = "Assets/Configs/Config.json";

    public ConfigClass ConfigClass;

    public ConfigManager()
    {

    }

    public IEnumerator LoadConfig()
    {
        StreamReader sr = new StreamReader(CONFIG_LOCATION);
        ConfigClass = JsonUtility.FromJson<ConfigClass>(sr.ReadToEnd());
        while(ConfigClass == null)
        {
            yield return null;
        }

        sr.Close();

        EventManager.ConfigLoaded?.Invoke();
    }
}
