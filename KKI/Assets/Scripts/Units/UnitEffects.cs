using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitEffects: MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private List<CardEffect> _temporaryEffects;

    public void AddEffect(CardEffect effect)
    {
        effect.CurrentMovesCount = effect.MovesCount;
        _temporaryEffects.Add(effect);
    }

    public void CheckEffects()
    {
        foreach(CardEffect effect in _temporaryEffects)
        {
            
        }
    }

}
