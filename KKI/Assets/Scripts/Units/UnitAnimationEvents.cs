using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationEvents : MonoBehaviour
{
    [SerializeField] private Unit _unit;

    private void OnValidate()
    {
        if (!_unit) _unit = transform.parent.GetComponent<Unit>();
    }

    public void OnAnimationFinished()
    {
        _unit.AnimationFinished();
    }
}
