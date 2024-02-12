using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Card collection")]

public class CardCollection : ScriptableObject
{
    [SerializeField] public List<GameObject> Cards;

    [SerializeField] public GameObject UnknownCard;

    public Card FindCard(string name)
    {
        foreach (GameObject cardGO in Cards)
        {
            if (cardGO.name == name.Replace("(Clone)", "")) return cardGO.GetComponent<Card>();
        }
        return null;
    }
}
