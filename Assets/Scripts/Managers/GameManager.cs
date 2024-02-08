using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.Events;
using Configuration;
using Data;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int _RoundAliveCount;
    private List<string> _KillLedger;
    private Coroutine _GameCo;
    private int _GameCount = 0;

    private Data.DataManager _DataManager;
    private ConfigManager _ConfigManager;
    private bool _DataLoaded;

    public ConfigClass Config => _ConfigManager.ConfigClass;

    private void Awake()
    {
        Instance = this;
        EventManager.UnitKilledAction += UnitKilled;
        _KillLedger = new List<string>();

        EventManager.DataLoaded += DataCompleted;
        _DataManager = new Data.DataManager();
        _ConfigManager = new ConfigManager();
    }

    private void Start()
    {

        _GameCo = StartCoroutine(GameCoroutine());
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void DataCompleted()
    {
        _DataLoaded = true;
    }

    //Leaderboards are visual, okay for names
    //Ledgers are data, need IDs
    public void UnitKilled(UnitInfo victim,UnitInfo attacker)
    {
        //Visual
        if(_KillLedger.Contains(victim.ID))
        {
            return;
        }

        _KillLedger.Add(victim.ID);
        UIManager.Instance.AddLeaderboardItem(victim.Name, _RoundAliveCount);
        if(_RoundAliveCount - 1 == 1)
        {
            UIManager.Instance.AddLeaderboardItem(attacker.Name, 1);
            _KillLedger.Add(attacker.ID);
        }

        //Data
        _DataManager.AddDeathAndKillData(victim.ID, _RoundAliveCount, attacker.ID);

        _RoundAliveCount--;
    }

    IEnumerator GameCoroutine()
    {
        //Config Load
        yield return StartCoroutine(_ConfigManager.LoadConfig());


        //Data Load
        _DataManager.ReadData();
        while(!_DataLoaded)
        {
            yield return null;
        }

        _RoundAliveCount = 0;
        
        while (true)
        {
            //Edge case where units can kill each other at the same time
            if(_RoundAliveCount <= 1)
            {
                if(_GameCount > 0)
                {
                    _DataManager.WriteData();
                }
                
                yield return new WaitForSeconds(1.0f);
                _GameCount++;
                _KillLedger.Clear();
                _RoundAliveCount = GameplayConstants.SPAWN_COUNT;
                EventManager.RoundStartAction?.Invoke();
                _DataManager.InitializeDataForNewRound();
            }
            yield return null;
        }
    }

    private void OnDestroy()
    {
        EventManager.UnitKilledAction -= UnitKilled;
        EventManager.DataLoaded -= DataCompleted;
    }
}
