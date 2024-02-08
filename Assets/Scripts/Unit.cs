using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using System;
using Configuration;

public class Unit : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _NMA;
    [SerializeField] private SphereCollider _VisionCollider;
    [SerializeField] private MeshRenderer _Renderer;

    [SerializeField] private TextMeshPro _DebugText;

    [SerializeField] private TextMeshProUGUI _HealthText;

    public Unit Enemy;

    private int _Health;
    private int _Speed;
    private int _Damage;

    private List<Unit> _SpottedUnits;
    private List<Unit> _AttackOptions;
    private List<Unit> _PursueOptions;

    private DateTime _LastAttackTime;
    private UnitClassInfo _ClassInfo;
    private UnitInfo _UnitInfo;

    public void Setup(UnitInfo unitInfo,UnitClassInfo classInfo)
    {
        this.gameObject.name = unitInfo.ID;
        _Health = classInfo.Health;
        _Speed = classInfo.Speed;
        _Damage = classInfo.Damage;
        _ClassInfo = classInfo;
        _UnitInfo = unitInfo;

        _NMA.speed = _Speed;
        _VisionCollider.radius = GameplayConstants.VISION_RADIUS;

        _SpottedUnits = new List<Unit>();
        _AttackOptions = new List<Unit>();
        _PursueOptions = new List<Unit>();

        UpdateUnitUI();

        DebugUnitColors();
    }

    private void DebugUnitColors()
    {
        switch(_ClassInfo.ID)
        {
            case "Light":
                _Renderer.material.color = Color.blue;
                break;
            case "Medium":
                _Renderer.material.color = Color.yellow;
                break;
            case "Heavy":
                _Renderer.material.color = Color.red;
                break;
        }
    }

    public void MoveTo(Vector3 loc,bool pursue)
    {
        if(!_NMA.isActiveAndEnabled)
        {
            return;
        }

        _NMA.speed = pursue ? GameplayConstants.DEFAULT_SPEED * GameplayConstants.PURSUE_MULTIPLIER : GameplayConstants.DEFAULT_SPEED;

        _NMA.SetDestination(loc);
    }

    public bool IsUnitAtDestination()
    {
        return Vector3.Distance(this.transform.position, _NMA.destination) <= _NMA.stoppingDistance * GameplayConstants.STOPPING_DISTANCE_ADJ;
    }

    public bool AttemptToSetEnemy()
    {
        if (_SpottedUnits.Count == 0)
        {
            return false;
        }

        CleanupAndSortUnitsInRange();

        _AttackOptions.Clear();

        _AttackOptions = _SpottedUnits.FindAll(o => IsLocationValid(o.transform.position,GameplayConstants.ATTACK_RANGE));

        Enemy = _AttackOptions.Count > 0 ? _AttackOptions[0] : null;
        return Enemy != null;
    }

    public bool AttemptToPursue()
    {
        if (_SpottedUnits.Count == 0)
        {
            return false;
        }

        CleanupAndSortUnitsInRange();

        _PursueOptions.Clear();

        _PursueOptions = _SpottedUnits.FindAll(o => IsLocationValid(o.transform.position, GameplayConstants.VISION_RADIUS));

        if(_PursueOptions.Count > 0)
        {
            MoveTo(_PursueOptions[0].transform.position,true);
            return true;
        }

        return false;

    }

    //Remmoves dead enemies and sorts by closest
    //and removes options if they are out of vision cone? (maybe add this later)
    private void CleanupAndSortUnitsInRange()
    {
        _SpottedUnits.RemoveAll(o => o == null);

        _SpottedUnits.Sort((a, b) => Vector3.Distance(a.gameObject.transform.position, this.transform.position)
        .CompareTo(Vector3.Distance(b.gameObject.transform.position, this.transform.position)));
    }

    public void SetDebugText(TaskType tt)
    {
        string taskTypeAb = "";
        switch(tt)
        {
            case TaskType.IDLE:
                taskTypeAb = "I";
                break;
            case TaskType.MOVE:
                taskTypeAb = "M";
                break;
            case TaskType.WAIT:
                taskTypeAb = "W";
                break;
            case TaskType.ATTACK:
                taskTypeAb = "A";
                break;
            case TaskType.PURSUE:
                taskTypeAb = "P";
                break;
        }
        _DebugText.SetText(taskTypeAb);
    }

    public void OnEnterRange(Unit unit)
    {
        if(!_SpottedUnits.Contains(unit))
        {
            _SpottedUnits.Add(unit);
        }

    }

    public void OnExitRange(Unit unit)
    {
        if (_SpottedUnits.Contains(unit))
        {
            _SpottedUnits.Remove(unit);
        }
    }

    //Checks to make sure loc is close enough and within vision range
    private bool IsLocationValid(Vector3 loc,float distance)
    {
        bool isValid = true;
        isValid = isValid && Vector3.Distance(this.transform.position, loc) <= distance;

        //Vision
        Vector3 direction = loc - this.transform.position;
        isValid = isValid && Mathf.Abs(Vector3.Angle(direction,this.transform.forward)) <= GameplayConstants.VISION_ANGLE_MAX;
        return isValid;

    }

    public void TryAttack()
    {
        //MoveTo(Enemy.transform.position,true);
        MoveTo(this.transform.position,false);
        //this.transform.LookAt(Enemy.transform.position);
        //Can attack time wise
        if((DateTime.Now - _LastAttackTime).TotalSeconds >= 1.0f)
        {
            if(IsLocationValid(Enemy.transform.position,GameplayConstants.ATTACK_RANGE))
            {
                if (Enemy.TakeDamage(_Damage,this))
                {
                    EventManager.UnitKilledAction?.Invoke(Enemy.GetUnitInfo(),_UnitInfo);
                    Enemy = null;
                }
                _LastAttackTime = DateTime.Now;
            }
            else
            {
                Enemy = null;
            }
        }
    }

    public UnitInfo GetUnitInfo()
    {
        return _UnitInfo;
    }

    public bool TakeDamage(int amt, Unit attacker)
    {
        Enemy = attacker;
        _Health -= amt;
        UpdateUnitUI();
        if (_Health <= 0)
        {
            Destroy(this.gameObject);
            return true;
        }

        return false;
    }

    public void UpdateUnitUI()
    {
        _HealthText.text = string.Format("{0}/{1}", _Health, _ClassInfo.Health);
    }
}
