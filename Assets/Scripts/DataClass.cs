using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace Data
{
    [Serializable]
    public class DataClass
    {
        public int TotalRounds;
        public List<RoundData> RoundDataList;

        //Tallys up the winner from each round
        public List<WinInfo> GetTotalWinCountAndInfo()
        {
            List<WinInfo> WinList = new List<WinInfo>();
            foreach (RoundData rd in RoundDataList)
            {
                UnitRoundData urd = rd.UnitRoundData[rd.UnitRoundData.Count - 1];
                string winnerID = urd.ID;
                
                WinInfo tempWinInfo = WinList.Find(o => o.ID == winnerID);
                if (string.IsNullOrEmpty(tempWinInfo.ID))
                {
                    string unitName = GameManager.Instance.Config.UnitConfig.GetName(winnerID);
                    WinList.Add(new WinInfo() { ID = winnerID, Name = unitName,Kills = urd.Kills, Count = 1 });
                }
                else
                {
                    tempWinInfo.Count++;
                    tempWinInfo.Kills += urd.Kills;

                    //int index = WinList.IndexOf(tempWinInfo);

                    //WinList[index] = new WinInfo() { ID = winnerID, Count = WinList[index].Count + 1 };
                }
                
            }

            WinList.Sort((x,y) => x.Count.CompareTo(y.Count));
            return WinList;
        }
    }

    [Serializable]
    public class RoundData
    {
        public string RoundID;
        public List<UnitRoundData> UnitRoundData;
    }

    [Serializable]
    public class UnitRoundData
    {
        public string ID;
        public int Place;
        public int Kills;
    }
}

public struct WinInfo
{
    public string ID;
    public string Name;
    public int Count;
    public int Kills;
}

