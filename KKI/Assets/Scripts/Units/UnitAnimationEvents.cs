using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimationEvents : MonoBehaviour
{
    [SerializeField] private Unit _unit;

    private void OnValidate()
    {
        if (!_unit) _unit = GetComponent<Unit>();
    }

    public void OnSlashFinished()
    {
        _unit.OnAttackOver();
    }
}
