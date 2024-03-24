using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitEffects: MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private CardCollection _collection;
    [SerializeField] private List<CardEffect> _temporaryEffects;
    [SerializeField] private Transform _effectsContainer;
    [SerializeField] private GameObject _effectPrefab;

    private void OnValidate()
    {
        if (!_effectsContainer) _effectsContainer = transform.Find("UnitCanvas")?.Find("Panel")?.Find("EffectsContainer");
    }

    public void Init(CardCollection collection)
    {
        _collection = collection;
    }

    public void AddEffect(CardEffect effect)
    {
        effect.CurrentMovesCount = effect.MovesCount;
        _temporaryEffects.Add(effect);
        UpdateEffectsIcons();
    }

    public StatsBonus SetBonus()
    {
        StatsBonus bonus = new StatsBonus();
        bonus.Damage = 0;
        bonus.Initiative = 0;
        bonus.Health = 0;
        bonus.Defence = 0;
        bonus.MDamage = 0;
        bonus.MResistance = 0;

        for (int i = 0; i < _temporaryEffects.Count; i++)
        {
            CardEffect effect = _temporaryEffects[i];

            bonus.Damage += effect.DamageBonus;
            bonus.Initiative += effect.MaxInitiativeBonus;
            bonus.Health += effect.MaxHealthBonus;
            bonus.Defence += effect.DefenceBonus;
            bonus.MDamage += effect.MDamage;
            bonus.MResistance += effect.MResistBonus;
        }
        return bonus;
    }

    public void CheckEffects()
    {
        List<CardEffect> expiredEffects = new List<CardEffect>();
        for (int i = 0; i < _temporaryEffects.Count; i++)
        {
            CardEffect effect = _temporaryEffects[i];
            effect.CurrentMovesCount--;
            if (effect.CurrentMovesCount == 0) expiredEffects.Add(effect);
            _temporaryEffects[i] = effect;
        }

        foreach (var effect in expiredEffects)
        {
            _temporaryEffects.Remove(effect);
        }

        UpdateEffectsIcons();
    }

    private void UpdateEffectsIcons()
    {
        for (int i = _effectsContainer.childCount-1; i >= 0; i--)
        {
            _effectsContainer.GetChild(i).gameObject.SetActive(false);
        }

        int index = 0;

        foreach (var effect in _temporaryEffects)
        {
            GameObject effectIndicator;
            

            if (_effectsContainer.childCount < index + 1)
            {
                effectIndicator = Instantiate(_effectPrefab, _effectsContainer);
            }
            else
            {
                effectIndicator = _effectsContainer.GetChild(index).gameObject;
                effectIndicator.SetActive(true);
            }

            Image effectSprite = effectIndicator.transform.Find("Icon")?.GetComponent<Image>();
            effectSprite.sprite = _collection.GetEffectSprite(effect.type);

            GameObject counterGO = effectIndicator.transform.Find("Counter").gameObject;
            if (effect.CurrentMovesCount > 0)
            {
                counterGO.SetActive(true);
                TMP_Text counter = counterGO?.GetComponent<TMP_Text>();
                if (effect.CurrentMovesCount > 10) counter.text = "\u221E";
                else counter.text = effect.CurrentMovesCount.ToString();
            }
            else
            {
                counterGO.SetActive(false);
            }
                

            index++;
        }
    }
}
