using Data;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private Transform _LeaderboardParent;
    [SerializeField] private Transform _LeaderboardParentOverall;
    [SerializeField] private LeadboardItem _LeaderboardItem;
    [SerializeField] private TextMeshProUGUI _DebugText;

    private void Awake()
    {
        Instance = this;
        EventManager.RoundStartAction += ClearLeaderboard;
        EventManager.RoundDataUpdated += UpdateOverallLeaderboard;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateOverallLeaderboard(Data.DataClass dc)
    {
        _LeaderboardParentOverall.RemoveAllChildrenUI();

        List<WinInfo> winList = dc.GetTotalWinCountAndInfo();

        int place = 1;
        foreach(WinInfo wi in winList)
        {
            AddOverallLeaderboardItem(string.Format("{0}({1})",wi.Name,wi.Kills), winList.Count - place + 1, wi.Count);
            place++;
        }
       
    }

    public void AddOverallLeaderboardItem(string unitID, int place,int count)
    {
        LeadboardItem li = Instantiate(_LeaderboardItem, _LeaderboardParentOverall);
        li.Set(unitID, place,count);
        li.transform.SetAsFirstSibling();
    }

    public void AddLeaderboardItem(string unitID,int place)
    {
        LeadboardItem li = Instantiate(_LeaderboardItem, _LeaderboardParent);
        li.Set(unitID, place);
        li.transform.SetAsFirstSibling();
    }

    public void ClearLeaderboard()
    {
        _LeaderboardParent.RemoveAllChildrenUI();
    }

    public void AddDebugText(string debugText)
    {
        _DebugText.text = _DebugText.text.ToString() + "\n " + debugText;
    }

    private void OnDestroy()
    {
        EventManager.RoundStartAction -= ClearLeaderboard;
        EventManager.RoundDataUpdated -= UpdateOverallLeaderboard;
    }

}
