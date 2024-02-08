using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LeadboardItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _UnitName;
    [SerializeField] private TextMeshProUGUI _UnitPlace;
    [SerializeField] private TextMeshProUGUI _CountText;

    public void Set(string unitID,int place)
    {
        _UnitName.text = unitID;
        _UnitPlace.text = place.ToString();
        _CountText.enabled = false;
    }

    public void Set(string unitID, int place,int count)
    {
        _UnitName.text = unitID;
        _UnitPlace.text = place.ToString();
        _CountText.text = count.ToString();
        _CountText.enabled = true;
    }
}
