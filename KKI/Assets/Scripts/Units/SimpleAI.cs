using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    [SerializeField] CombatManager _combatManager;

    public void Init(CombatManager combatManager)
    {
        if (combatManager != null) _combatManager = combatManager;
        else _combatManager = FindObjectOfType<CombatManager>();
    }

    public PlayerUnit PickTarget()
    {
        int rnd = Random.Range(0, _combatManager.PlayerUnits.Count);
        return _combatManager.PlayerUnits[rnd];
    }
}
