using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using Random = UnityEngine.Random;

public enum TaskType
{
    IDLE,
    MOVE,
    WAIT,
    ATTACK,
    PURSUE
}

public class TaskManager : MonoBehaviour
{
    //public static TaskManager Instance;

    [SerializeField] private Transform _TopLeftBounds;
    [SerializeField] private Transform _BottomRightBounds;

    private const float TASK_UPDATE_TICK = .2f;

    private List<TaskInfo> _CreatedUnits;

    private Coroutine _TaskCoroutine;

    private List<Unit> _UnitsInDangerArea;

    private void Awake()
    {
        //Instance = this;
        _CreatedUnits = new List<TaskInfo>();
        _UnitsInDangerArea = new List<Unit>();
        EventManager.SpawnCompleted += SetupRound;
    }

    private void SetupRound(List<Unit> units)
    {
        _CreatedUnits.Clear();
        foreach(Unit unit in units)
        {
            _CreatedUnits.Add(new TaskInfo()
            {
                UnitRef = unit
            });
        }
        _UnitsInDangerArea.Clear();
        StartTaskManager();
    }

    public void StartTaskManager()
    {
        if(_TaskCoroutine != null)
        {
            StopCoroutine(_TaskCoroutine);
        }
        _TaskCoroutine = StartCoroutine(TaskUpdate());
    }

    IEnumerator TaskUpdate()
    {
        while(true) 
        {
            for(int i = 0; i < _CreatedUnits.Count;i++)
            {
                TaskInfo taskInfo = _CreatedUnits[i];
                TaskType startingTaskType = taskInfo.CurrentTask;
                bool canAttack = IsInDangerArea(taskInfo.UnitRef);

                //For now, just skip killed enemies
                if (taskInfo.UnitRef == null|| taskInfo.UnitRef.gameObject == null)
                {
                    continue;
                }

                if(!canAttack)
                {
                    taskInfo.UnitRef.Enemy = null;
                }

                //First keep attacking if you already have a target
                if (taskInfo.UnitRef.Enemy)
                {
                    taskInfo.CurrentTask = TaskType.ATTACK;
                    taskInfo.UnitRef.TryAttack();
                }
                else
                {
                    //Second begin attacking enemy if within range
                    if(canAttack && taskInfo.UnitRef.AttemptToSetEnemy())
                    {
                        taskInfo.CurrentTask = TaskType.ATTACK;
                        taskInfo.UnitRef.TryAttack();
                    }
                    //Third get close to a potential enemy
                    else if(canAttack && taskInfo.UnitRef.AttemptToPursue())
                    {
                        taskInfo.CurrentTask = TaskType.PURSUE;
                    }
                    //Fourth just keep exploring
                    else
                    {
                        if(taskInfo.CurrentTask == TaskType.ATTACK 
                            || taskInfo.CurrentTask == TaskType.PURSUE)
                        {
                            taskInfo.CurrentTask = TaskType.IDLE;
                        }

                        switch (taskInfo.CurrentTask)
                        {
                            case TaskType.IDLE:
                                DetermineNextTaskFromIdle(ref taskInfo);
                                break;
                            case TaskType.MOVE:
                                if (_CreatedUnits[i].UnitRef.IsUnitAtDestination())
                                {
                                    taskInfo.CurrentTask = TaskType.IDLE;
                                }
                                break;
                            case TaskType.WAIT:
                                if (taskInfo.EndWaitTime < DateTime.Now)
                                {
                                    taskInfo.CurrentTask = TaskType.IDLE;
                                }
                                break;

                        }
                    }
                }

                if(startingTaskType != taskInfo.CurrentTask)
                {
                    _CreatedUnits[i] = taskInfo;
                    _CreatedUnits[i].UnitRef.SetDebugText(taskInfo.CurrentTask);
                }
            }
            yield return new WaitForSeconds(TASK_UPDATE_TICK);
        }
    }

    private void DetermineNextTaskFromIdle(ref TaskInfo taskInfo)
    {
        if (Random.Range(0.0f, 1.0f) < .5f)
        {
            Vector3 loc = GameplayConstants.GetValidPointOnMap(_TopLeftBounds.position, _BottomRightBounds.position);
            if(loc != Vector3.zero)
            {
                taskInfo.UnitRef.MoveTo(loc,false);
                taskInfo.CurrentTask = TaskType.MOVE;
                return;
            }

        }

        taskInfo.EndWaitTime = DateTime.Now.AddSeconds(Random.Range(GameplayConstants.MIN_WAIT_TIME, GameplayConstants.MAX_WAIT_TIME));
        taskInfo.CurrentTask = TaskType.WAIT;
    }

    public void OnUnitEnterDangerArea(Unit unit)
    {
        if(!_UnitsInDangerArea.Contains(unit))
        {
            _UnitsInDangerArea.Add(unit);
        }
    }

    public bool IsInDangerArea(Unit unit)
    {
        return _UnitsInDangerArea.Contains(unit);
    }

    public void OnUnitExitDangerArea(Unit unit)
    {
        if (_UnitsInDangerArea.Contains(unit))
        {
            _UnitsInDangerArea.Remove(unit);
        }
    }

    private void OnDestroy()
    {
        EventManager.SpawnCompleted -= SetupRound;
    }
}

public struct TaskInfo
{
    public Unit UnitRef;
    public TaskType CurrentTask;
    public DateTime EndWaitTime;
}
