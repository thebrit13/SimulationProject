using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TriggerAssistant : MonoBehaviour
{
    [SerializeField] private UnityEvent<Unit> _OnEnter;
    [SerializeField] private UnityEvent<Unit> _OnExit;
    [SerializeField] private bool _ChildCollider = true;

    private void OnTriggerEnter(Collider other)
    {
        Unit unit = _ChildCollider ? other.GetComponentInParent<Unit>() : other.GetComponent<Unit>();
        if(unit)
        {
            _OnEnter?.Invoke(unit);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Unit unit = _ChildCollider ? other.GetComponentInParent<Unit>() : other.GetComponent<Unit>();
        if (unit)
        {
            _OnExit?.Invoke(unit);
        }
    }
}
