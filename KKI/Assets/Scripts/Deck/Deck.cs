using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DeckUI))]
//Этот класс отвечает за хранение колоды
public class Deck : MonoBehaviour
{
    [SerializeField] private List<string> _playerDeck = new();
    [SerializeField] private List<string> _playerDiscard = new();
    [SerializeField] private List<string> _playerCurrentDeck = new();
    [SerializeField] private List<string> _playerHand = new();
    [SerializeField] private DeckUI _deckUI;
    [SerializeField] private Game _game;

    public List<string> PlayerDeck => _playerDeck;
    public List<string> PlayerHand => _playerHand;
    public List<string> PlayerCurrentDeck => _playerCurrentDeck;
    public List<string> PlayerDiscard => _playerDiscard;

    private void OnValidate()
    {
        //if (!_game) _game = GetComponent<Game>();
        if (!_deckUI) _deckUI = GetComponent<DeckUI>();
    }

    public void Init(Game game)
    {
        _game = game;
        EventBus.Instance.DiscardCard.AddListener(DiscardCard);
        _playerDiscard.Clear();
        ResetCurrentDeck();
        _deckUI.Init(this);
    }

    public void AddToDeck(Card card)
    {
        _playerDeck.Add(card.name.Replace("(Clone)",""));
    }


    public void AddToHand(List<string> cards)
    {
        _playerHand.AddRange(cards);
        _deckUI.UpdateUI();
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
        _playerDeck.Clear();
        foreach (string card in cards)
        {
            if(card!="") _playerDeck.Add(card);
        }
        ResetCurrentDeck();
    }

    [ContextMenu("Drop saved deck")]

    public void DropDeck()
    {
        PlayerPrefs.SetString("PlayerDeck", "");
    }

    public List<string> GetRandomCards(int num)
    {
        List<string> cards = new();

        for (int i = 0; i < num; i++)
        {
            int rnd = Random.Range(0, _playerCurrentDeck.Count);
            cards.Add(_playerCurrentDeck[rnd]);
            _playerCurrentDeck.RemoveAt(rnd);
            if (_playerCurrentDeck.Count == 0)
            {
                ResetDiscard();
            }
        }
        _deckUI.UpdateUI();
        return cards;
    }

    private void ResetCurrentDeck()
    {
        _playerCurrentDeck.Clear();
        _playerCurrentDeck.AddRange(_playerDeck);
    }

    private void ResetDiscard()
    {
        _playerCurrentDeck.AddRange(_playerDiscard);
    }

    private void DiscardCard(Card card)
    {
        string cardName = card.name.Replace("(Clone)", "");
        _playerHand.Remove(cardName);
        _playerDiscard.Add(cardName);
        _deckUI.UpdateUI();
    }
}
