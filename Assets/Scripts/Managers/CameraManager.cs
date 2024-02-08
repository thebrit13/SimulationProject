using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private Transform _CameraTransform;
    //[SerializeField] private Camera _MainCamera;

    private Coroutine _CameraMovementCo = null;
    private Coroutine _CameraGoalUpdateCo = null;
    private Vector3 _MiddlePointOfUnits = Vector3.zero;
    private List<Unit> _SpawnedUnits = null;
    private Vector3 _CameraGoal;

    private void Awake()
    {
        _CameraTransform.transform.position = new Vector3(0, GameplayConstants.MAX_CAMERA_HEIGHT, 0);
        EventManager.SpawnCompleted += UpdateUnits;
    }

    private void UpdateUnits(List<Unit> spawnedUnits)
    {
        _SpawnedUnits = spawnedUnits;
        ResetCamera();
    }

    void ResetCamera()
    {
        _CameraTransform.transform.position = new Vector3(0, GameplayConstants.MAX_CAMERA_HEIGHT, 0);
        _CameraGoal = _CameraTransform.transform.position;
        if (_CameraGoalUpdateCo != null)
        {
            StopCoroutine(_CameraGoalUpdateCo);
        }
        if (_CameraMovementCo != null)
        {
            StopCoroutine(_CameraMovementCo);
        }
        _CameraGoalUpdateCo = StartCoroutine(UpdateCameraGoal());
        _CameraMovementCo = StartCoroutine(UpdateCameraPosition());
    }

    IEnumerator UpdateCameraPosition()
    {
        while(true)
        {
            _CameraTransform.transform.position = Vector3.Lerp(_CameraTransform.transform.position,_CameraGoal, Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator UpdateCameraGoal()
    {
        while(true)
        {
            _SpawnedUnits.RemoveAll(o => o == null);
            Vector3 topLeftUnit = GameplayConstants.GetTopLeftForAllUnits(_SpawnedUnits);
            Vector3 bottomRightUnit = GameplayConstants.GetBottomRightForAllUnits(_SpawnedUnits);

            _CameraGoal = GetCameraZoomAndLocationGoal(topLeftUnit, bottomRightUnit);

            yield return new WaitForSeconds(GameplayConstants.GOAL_UPDATE_TICK);
        }
    }

    //Distance at both topLeft and bottomRight points should be the same
    private Vector3 GetCameraZoomAndLocationGoal(Vector3 unitsTopLeft, Vector3 unitsBottomRight)
    {
        Vector3 unitsMidPoint = Vector3.Lerp(unitsTopLeft, unitsBottomRight,.5f);
        _MiddlePointOfUnits = unitsMidPoint;

        float cameraY = Mathf.Lerp(GameplayConstants.MIN_CAMERA_HEIGHT, GameplayConstants.MAX_CAMERA_HEIGHT,
            Vector3.Distance(unitsTopLeft, unitsBottomRight) / GameplayConstants.MAX_UNIT_DISTANCE);

        return new Vector3(unitsMidPoint.x,cameraY,unitsMidPoint.z);
    }

    private void OnDestroy()
    {
        EventManager.SpawnCompleted -= UpdateUnits;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(_MiddlePointOfUnits, 1);
        //Gizmos.DrawSphere(_MainCamera.ScreenToWorldPoint(new Vector3(0, Screen.height, _MainCamera.transform.position.y)),1);
        //Gizmos.DrawSphere(_MainCamera.ScreenToWorldPoint(new Vector3(Screen.width, 0, _MainCamera.transform.position.y)),1);
    }
}
