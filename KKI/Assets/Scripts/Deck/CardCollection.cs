using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

[CreateAssetMenu(menuName = "ScriptableObjects/Card collection")]

// ��� SO ��� ��������� ��������� ����

public class CardCollection : ScriptableObject
{
    [SerializeField] public List<GameObject> Cards;

    [SerializeField] public SerializedDictionary<EffectTypes, Sprite> EffectIcons;
    [SerializeField] public SerializedDictionary<EffectTypes, string> EffectDescriptions;
    [SerializeField] public SerializedDictionary<CardTypes, string> CardTypesDescriptions;
    [SerializeField] public SerializedDictionary<CardColors, Material> CardMaterials;

    [SerializeField] public GameObject UnknownCard;
    [SerializeField] public Sprite _defaultSprite;

    public Card FindCard(string name)
    {
        foreach (GameObject cardGO in Cards)
        {
            if (cardGO.name == name.Replace("(Clone)", "")) return cardGO.GetComponent<Card>();
        }
        return null;
    }

    public Sprite GetEffectSprite(EffectTypes effectType)
    {
        Sprite sprite = EffectIcons.GetValueOrDefault(effectType, _defaultSprite);
        return sprite;
    }
}
