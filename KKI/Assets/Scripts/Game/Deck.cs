using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    [SerializeField] private List<string> _playerDeck = new();
    [SerializeField] private List<string> _enemyDeck = new();
    [SerializeField] private Game _game;
    private Card _selectedCard;
    private bool _haveSelectedCard;

    public List<string> PlayerDeck => _playerDeck;

    private void OnValidate()
    {
        if (!_game) _game = GetComponent<Game>();
    }

    public void AddToDeck(Card card)
    {
        _playerDeck.Add(card.name.Replace("(Clone)",""));
    }

    public void RemoveFromDeck(Card card)
    {
        _playerDeck.Remove(card.name.Replace("(Clone)", ""));
    }

    public void SaveDeck()
    {
        string deck = "";
        foreach (string card in _playerDeck)
        {
            deck += card;
            deck += ",";
        }
        deck = deck.TrimEnd(',');
        PlayerPrefs.SetString("PlayerDeck",deck);
    }

    public void LoadDeck()
    {
        string deck = PlayerPrefs.GetString("PlayerDeck", "");
        string[] cards = deck.Split(',');
        foreach (string card in cards)
        {
            if(card!="") _playerDeck.Add(card);
        }
    }

    [ContextMenu("Drop saved deck")]

    public void DropDeck()
    {
        PlayerPrefs.SetString("PlayerDeck", "");
    }
}
