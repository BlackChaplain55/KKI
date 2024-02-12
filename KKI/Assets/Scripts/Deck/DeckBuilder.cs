using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private CardCollection _cardCollection;
    [SerializeField] private Transform _collectionContainer;
    [SerializeField] private TMP_Text _deckCounter;
    [SerializeField] private int _collectionSize;
    [SerializeField] private Transform _playerDeck;
    [SerializeField] private AutoLayout3D.GridLayoutGroup3D _layoutGroup3D;
    [SerializeField] private Game _game;
    [SerializeField] private int _deckSize = 20;

    private void OnValidate()
    {
        if (!_playerDeck) _playerDeck = transform.Find("PlayerDeckBuild").transform;
        if (!_layoutGroup3D) _layoutGroup3D = _playerDeck.GetComponent<AutoLayout3D.GridLayoutGroup3D>();
    }

    private void Awake()
    {
        _game = FindObjectOfType<Game>();
        InitializeCollection();
        InitializeDeck();
        EventBus.Instance.OnSelectCard?.AddListener(OnSelectCard);
    }

    private void InitializeCollection()
    {
        int cardCount=0;

        foreach (GameObject cardObject in _cardCollection.Cards)
        {
            Card card = cardObject.GetComponent<Card>();
            GameObject newCard = Instantiate(cardObject,_collectionContainer);
            newCard.transform.localRotation = Quaternion.Euler(0, -90, 70);
            cardCount++;
        }

        for (int i = cardCount; i < _collectionSize; i++)
        {
            GameObject newCard = Instantiate(_cardCollection.UnknownCard, _collectionContainer);
            newCard.transform.localRotation = Quaternion.Euler(0, -90, 70);
        }
    }

    private void InitializeDeck()
    {
        _game.CurrentDeck.LoadDeck();
        foreach (string cardName in _game.CurrentDeck.PlayerDeck)
        {
            Card card = _cardCollection.FindCard(cardName);
            if (card != null)
            {
                GameObject newCard = Instantiate(card.GameObject, _playerDeck);
                newCard.transform.localRotation = Quaternion.Euler(0, -100, 70);
                newCard.GetComponent<Card>().IsInDeck = true;
            }
        }
        SetLayoutSpacing();
        _deckCounter.text = _playerDeck.childCount + "/" + _deckSize;
    }

    private void OnSelectCard(Card card)
    {
        if (_playerDeck.childCount == _deckSize || card.name.Contains(Constants.UnknownCard)) return;
        if (!card.IsInDeck)
        {
            Vector3 position = card.CardModel.transform.position;
            Quaternion rotation = card.CardModel.transform.rotation;
            card.CardModel.transform.localPosition = card.ModelDefaultPosition;
            card.CardModel.transform.localRotation = card.ModelDefaultRotation;
            GameObject newCard = Instantiate(card.GameObject, _playerDeck);
            card.CardModel.transform.localPosition = position;
            card.CardModel.transform.localRotation = rotation;
            newCard.GetComponent<Card>().IsInDeck = true;
            newCard.transform.localRotation = Quaternion.Euler(0, -100, 70);
            SetLayoutSpacing();
            _game.CurrentDeck.AddToDeck(card);
            _game.CurrentDeck.SaveDeck();
            _deckCounter.text = _playerDeck.childCount+ "/"+ _deckSize;
        }
        else
        {
            _game.CurrentDeck.RemoveFromDeck(card);
            DestroyImmediate(card.gameObject);
            _deckCounter.text = _playerDeck.childCount + "/" + _deckSize;
        }
    }

    private void SetLayoutSpacing()
    {
        if (_playerDeck.childCount <= 11)
            _layoutGroup3D.spacing.x = 5;
        else if (_playerDeck.childCount <= 13)
            _layoutGroup3D.spacing.x = 4;
        else
            _layoutGroup3D.spacing.x = 3;
    }
}
