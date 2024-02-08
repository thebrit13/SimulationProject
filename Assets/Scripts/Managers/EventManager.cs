using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventManager
{
    #region Game Management
    public static System.Action RoundStartAction;
    public static System.Action<Configuration.UnitInfo,Configuration.UnitInfo> UnitKilledAction;
    public static System.Action<Data.DataClass> RoundDataUpdated;
    public static System.Action ConfigLoaded;
    public static System.Action DataLoaded;
    public static System.Action<List<Unit>> SpawnCompleted;
    #endregion
}
