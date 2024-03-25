using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    [SerializeField] CombatManager _combatManager;
    [SerializeField] Unit _unit;

    public void Init(CombatManager combatManager, Unit unit)
    {
        if (combatManager != null) _combatManager = combatManager;
        else _combatManager = FindObjectOfType<CombatManager>();
        _unit = unit;
    }

    public PlayerUnit PickTarget()
    {
        foreach (PlayerUnit pUnit in _combatManager.PlayerUnits)
        {
            if (pUnit.Effects.CheckEffectExist(EffectTypes.provoke)) return pUnit;
        }
        int rnd = Random.Range(0, _combatManager.PlayerUnits.Count);
        return _combatManager.PlayerUnits[rnd];
    }
}
