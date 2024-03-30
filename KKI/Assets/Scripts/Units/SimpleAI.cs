using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private Unit _unit;
    [SerializeField] private List<AIAction> _actions = new();
    [SerializeField] private List<AIAction> _attacks = new();

    public void Init(CombatManager combatManager, Unit unit)
    {
        if (combatManager != null) _combatManager = combatManager;
        else _combatManager = FindObjectOfType<CombatManager>();
        _unit = unit;
    }

    public PlayerUnit PickTarget(AIActionTypes type = AIActionTypes.RandomSingleDamage)
    {
        foreach (PlayerUnit pUnit in _combatManager.PlayerUnits)
        {
            if (pUnit.Effects.CheckEffectExist(EffectTypes.provoke)) return pUnit;
        }

        if (type == AIActionTypes.StrongestSingle)
        {
            PlayerUnit targetUnit = _combatManager.PlayerUnits[0];
            foreach (PlayerUnit pUnit in _combatManager.PlayerUnits)
            {
                if (pUnit.CurrentHealth > targetUnit.CurrentHealth) targetUnit = pUnit;
            }
        }
        else if (type == AIActionTypes.WeakestSingle)
        {
            PlayerUnit targetUnit = _combatManager.PlayerUnits[0];
            foreach (PlayerUnit pUnit in _combatManager.PlayerUnits)
            {
                if (pUnit.CurrentHealth < targetUnit.CurrentHealth) targetUnit = pUnit;
            }
        }
        else
        {
            int rnd = Random.Range(0, _combatManager.PlayerUnits.Count);
            return _combatManager.PlayerUnits[rnd];
        }
        return null;
    }

    public AIDecision PickDecision()
    {
        AIDecision decision = new();
        decision.targets = new();

        if (_actions.Count == 0&& _attacks.Count == 0)
        {
            AIAction action = new();
            action.Type = AIActionTypes.RandomSingleDamage;
            action.PValue = _unit.Damage+ _unit.Bonus.Damage;
            action.MValue = _unit.MDamage+ _unit.Bonus.Damage;
            decision.action = action;
            decision.targets.Add(PickTarget());
            return decision;
        }

        foreach(AIAction action in _actions)
        {
            if(action.Type == AIActionTypes.Heal)
            {
                Unit woundedUnit = CheckWounded();
                if (woundedUnit != null)
                {
                    decision.targets.Add(woundedUnit);
                    decision.action = action;
                    return decision;
                }
            }
        }

        foreach (AIAction action in _actions)
        {
            if (action.Type == AIActionTypes.AOEHeal)
            {
                if (CheckTeamHealth())
                {
                    decision.targets.AddRange(_combatManager.EnemyUnits);
                    decision.action = action;
                    return decision;
                }
            }
        }

        int rnd = Random.Range(0, _attacks.Count);
        AIAction attack = new();
        attack = _attacks[rnd];
        if (attack.PValue == 0 && attack.MValue == 0)
        {
            attack.PValue = _unit.Damage + _unit.Bonus.Damage;
            attack.MValue = _unit.MDamage + _unit.Bonus.MDamage;
        }
        if (attack.Type == AIActionTypes.RandomSingleDamage)
        {
            decision.targets.Add(PickTarget());
        }
        else if(attack.Type == AIActionTypes.AOEDamage)
        {
            decision.targets.AddRange(_combatManager.PlayerUnits);
        }
        else 
        {
            decision.targets.Add(PickTarget(attack.Type));
        }

        return decision;
    }

    private Unit CheckWounded()
    {
        foreach(Unit enemyAlly in _combatManager.EnemyUnits)
        {
            if (enemyAlly.CurrentHealth < enemyAlly.MaxHealth * 0.5f)
            {
                return enemyAlly;
            }
        }
        return null;
    }

    private bool CheckTeamHealth()
    {
        float teamHealth = 0f;
        foreach (Unit enemyAlly in _combatManager.EnemyUnits)
        {
            teamHealth += enemyAlly.CurrentHealth / enemyAlly.MaxHealth;
        }

        if (teamHealth < 0.5f*_combatManager.EnemyUnits.Count)
        {
            return true;
        }
        return false;
    }
}

public struct AIDecision
{
    public AIAction action;
    public List<Unit> targets;
}

[System.Serializable]

public struct AIAction
{
    public string Name;
    public float PValue;
    public float MValue;
    public float Heal;
    public AIActionTypes Type;
}

[System.Serializable]

public enum AIActionTypes
{
    AOEDamage,
    AOEHeal,
    Heal,
    RandomSingleDamage,
    StrongestSingle,
    WeakestSingle
}
