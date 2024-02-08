using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameplayConstants
{
    public static readonly int SPAWN_RADIUS = 25;

    public static readonly float STOPPING_DISTANCE_ADJ = 1.5f;

    #region Unit
    public static readonly int SPAWN_COUNT = 12;

    public static readonly int DEFAULT_HEALTH = 5;
    public static readonly int DEFAULT_SPEED = 5;
    public static readonly int DEFAULT_DAMAGE = 1;


    public static readonly float VISION_RADIUS = 50.0f;
    public static readonly float VISION_ANGLE_MAX = 80.0f;
    public static readonly float ATTACK_RANGE = 5.0f;

    public static readonly float PURSUE_MULTIPLIER = 1.25f;
    #endregion

    #region TaskManager
    public static readonly float MIN_WAIT_TIME = 1.0f;
    public static readonly float MAX_WAIT_TIME = 4.0f;

    public static readonly int MAX_VALID_LOC_ATTEMPTS = 5;
    #endregion

    #region Camera
    public static readonly float GOAL_UPDATE_TICK = .25f;
    public static readonly float MIN_CAMERA_HEIGHT = 25;
    public static readonly float MAX_CAMERA_HEIGHT = 200;
    //public static readonly float MIN_UNIT_DISTANCE = 10;
    public static readonly float MAX_UNIT_DISTANCE = 250;


    //public static readonly float MAX_MOVE_PER_UPDATE = 5;
    //public static readonly float MIN_CAMERA_LERP_DISTANCE = 10;
    //public static readonly float MAX_CAMERA_LERP_DISTANCE = 100;

    //public static readonly float MAX_Y_GOAL = 5.0f;
    #endregion

    //public static Vector3 GetRandomPointWithinRadius()
    //{
    //    Vector2 randLoc = Random.insideUnitCircle * GameplayConstants.SPAWN_RADIUS;
    //    Vector3 loc = new Vector3(randLoc.x, 0, randLoc.y);
    //    return loc;
    //}

    public static Vector3 GetValidPointOnMap(Vector3 topLeftBounds, Vector3 bottomRightBounds)
    {
        int attempts = 0;
        while (attempts < MAX_VALID_LOC_ATTEMPTS)
        {
            //First get some random points
            float randX = Random.Range(topLeftBounds.x, bottomRightBounds.x);
            float randY = Random.Range(topLeftBounds.z, bottomRightBounds.z);

            Vector3 potentialLoc = new Vector3(randX, 0, randY);
            int mask = LayerMask.GetMask("Map");

            //Then verify they are valid
            if (Physics.Raycast(potentialLoc + new Vector3(0, 90, 0), Vector3.down, 100.0f, mask))
            {
                return potentialLoc;
            }
            attempts++;
        }
        return Vector3.zero;

    }

    //Dont think this works
    public static Vector3 GetMiddlePoint(List<Unit> aliveUnits)
    {
        Vector3 middle = Vector3.zero;
        foreach (Unit unit in aliveUnits)
        {
            middle += unit.transform.position;
        }
        return middle / GameplayConstants.SPAWN_COUNT;
    }

    public static Vector3 GetTopLeftForAllUnits(List<Unit> aliveUnits)
    {
        Vector3 topLeft = Vector3.zero;
        foreach(Unit unit in aliveUnits)
        {
            if(unit.transform.position.x < topLeft.x)
            {
                topLeft.x = unit.transform.position.x;
            }

            if(unit.transform.position.z > topLeft.z)
            {
                topLeft.z = unit.transform.position.z;
            }
        }
        return topLeft;
    }

    public static Vector3 GetBottomRightForAllUnits(List<Unit> aliveUnits)
    {
        Vector3 bottomRight = Vector3.zero;
        foreach (Unit unit in aliveUnits)
        {
            if (unit.transform.position.x > bottomRight.x)
            {
                bottomRight.x = unit.transform.position.x;
            }

            if(unit.transform.position.z < bottomRight.z)
            {
                bottomRight.z = unit.transform.position.z;
            }
        }
        return bottomRight;
    }
}
