using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

namespace Data
{
    public class DataManager: MonoBehaviour
    {
        const string DATA_PATH = "Results";
        const string FILE_NAME = "GameData.json";

        private DataClass _DataClass;

        private RoundData _CurrentRoundData;

        public void ReadData()
        {
            string dataPath = Application.persistentDataPath + "/" + DATA_PATH + "/" + FILE_NAME;
            if (File.Exists(dataPath))
            {
                StreamReader sr = new StreamReader(Path.Combine(Application.persistentDataPath, DATA_PATH, FILE_NAME));
                _DataClass = JsonUtility.FromJson<DataClass>(sr.ReadToEnd());
                sr.Close();
            }
            else
            {
                _DataClass = new DataClass();
                _DataClass.RoundDataList = new List<RoundData>();
            }

            EventManager.DataLoaded?.Invoke();
            EventManager.RoundDataUpdated?.Invoke(_DataClass);
        }

        public void InitializeDataForNewRound()
        {
            _CurrentRoundData = new RoundData()
            {
                UnitRoundData = new List<UnitRoundData>()
            };
        }

        public void AddDeathAndKillData(string killedID, int killedPlace, string attackerID)
        {
            UnitRoundData urdKilled = _CurrentRoundData.UnitRoundData.Find(o => o.ID == killedID);
            //Killed
            if (urdKilled != null)
            {
                urdKilled.Place = killedPlace;
            }
            else
            {
                UnitRoundData urdKilledNew = new UnitRoundData()
                {
                    ID = killedID,
                    Place = killedPlace,
                    Kills = 0
                };
                _CurrentRoundData.UnitRoundData.Add(urdKilledNew);
            }

            //Attacker
            UnitRoundData urdAttacker = _CurrentRoundData.UnitRoundData.Find(o => o.ID == attackerID);
            if (urdAttacker != null)
            {
                urdAttacker.Kills++;
            }
            else
            {
                UnitRoundData urdAttackerNew = new UnitRoundData()
                {
                    ID = attackerID,
                    Place = 1,
                    Kills = 1
                };
                _CurrentRoundData.UnitRoundData.Add(urdAttackerNew);
            }
        }

        public void WriteData()
        {
            //RoundData rd = new RoundData()
            //{
            //    RoundID = roundID.ToString(),
            //    UnitRoundData = new List<UnitRoundData>()
            //};

            StreamWriter sw = new StreamWriter(Path.Combine(Application.persistentDataPath, DATA_PATH, FILE_NAME));
            //for (int i = 0; i < units.Count; i++)
            //{
            //    UnitRoundData urd = new UnitRoundData()
            //    {
            //        ID = units[i],
            //        Place = GameplayConstants.SPAWN_COUNT - i
            //    };
            //    rd.UnitRoundData.Add(urd);
            //}

            
            _DataClass.TotalRounds++;
            _CurrentRoundData.RoundID = _DataClass.TotalRounds.ToString();
            _DataClass.RoundDataList.Add(_CurrentRoundData);

            sw.Write(JsonUtility.ToJson(_DataClass));
            sw.Close();

            EventManager.RoundDataUpdated(_DataClass);
        }
    }

}

