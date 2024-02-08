using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Configuration
{
    [Serializable]
    public class ConfigClass
    {
        public UnitConfig UnitConfig;
    }

    [Serializable]
    public class UnitConfig
    {
        public List<UnitInfo> UnitInformation;
        public List<UnitClassInfo> UnitClassInformation;

        public UnitClassInfo GetUnitClassInfo(string id)
        {
            return UnitClassInformation.Find(o => o.ID == id);
        }

        public string GetName(string id)
        {
            return UnitInformation.Find(o => o.ID == id).Name;
        }
    }

    [Serializable]
    public class UnitInfo
    {
        public string ID;
        public string Name;
        public string ClassID;
    }

    [Serializable]
    public class UnitClassInfo
    {
        public string ID;
        public int Speed;
        public int Health;
        public int Damage;
    }
}
