using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Configuration;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField] private Unit _Unit;

    [SerializeField] private List<Transform> _SpawnPoints;

    private List<Unit> _CreatedUnits = new List<Unit>();

    private List<UnitInfo> _UnitData;

    private void Awake()
    {
        EventManager.RoundStartAction += SpawnUnits;
        EventManager.ConfigLoaded += SetUnitData;
    }

    private void SetUnitData()
    {
        _UnitData = GameManager.Instance.Config.UnitConfig.UnitInformation;
    }

    public void SpawnUnits()
    {
        ClearOlderUnits();
        for (int i = 0; i < _UnitData.Count; i++)
        {
            Unit unit = Instantiate(_Unit, _SpawnPoints[i].position, Quaternion.identity);
            unit.transform.LookAt(Vector3.zero);

            UnitClassInfo classInfo = GameManager.Instance.Config.UnitConfig.GetUnitClassInfo(_UnitData[i].ClassID);
            unit.Setup(_UnitData[i], classInfo);
            _CreatedUnits.Add(unit);
        }

        EventManager.SpawnCompleted?.Invoke(_CreatedUnits);
    }

    private void ClearOlderUnits()
    {
        _CreatedUnits.RemoveAll(o => o == null);

        for(int i = _CreatedUnits.Count -1;i >= 0;i--)
        {
            Destroy(_CreatedUnits[i].gameObject);
        }

        _CreatedUnits.Clear();
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawSphere(Vector3.zero, GameplayConstants.SPAWN_RADIUS);
    }

    private void OnDestroy()
    {
        EventManager.RoundStartAction -= SpawnUnits;
        EventManager.ConfigLoaded -= SetUnitData;
    }
}
