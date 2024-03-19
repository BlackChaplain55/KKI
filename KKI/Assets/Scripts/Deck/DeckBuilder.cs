using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//Этот класс отвечает за сборку колоды
public class DeckBuilder : MonoBehaviour
{
    [SerializeField] private CardCollection _cardCollection;
    [SerializeField] private Transform _collectionContainer;
    [SerializeField] private TMP_Text _deckCounter;
    [SerializeField] private int _collectionSize;
    [SerializeField] private Transform _playerDeck;
    [SerializeField] private AutoLayout3D.GridLayoutGroup3D _layoutGroup3D;
    [SerializeField] private Game _game;
    [SerializeField] private int _deckSize = 25;
    [SerializeField] private bool _haveActiveCard=false;

    public bool HaveActiveCard => _haveActiveCard;

    private void OnValidate()
    {
        if (!_playerDeck) _playerDeck = transform.Find("PlayerDeckBuild").transform;
        if (!_layoutGroup3D) _layoutGroup3D = _playerDeck.GetComponent<AutoLayout3D.GridLayoutGroup3D>();
    }

    //private void Awake()
    public void Init(Game game)
    {
        _game = game;
        //_game = FindObjectOfType<Game>();
        InitializeCollection();
        InitializeDeck();
        EventBus.Instance.ActivateCard?.AddListener(OnSelectCard);
    }

    public void ActivateCard(bool state)
    {
        _haveActiveCard = state;
    }

    public void AddCardToDeck(string cardName) {
        if (_playerDeck.childCount == _deckSize || cardName.Contains(Constants.UnknownCard)) return;
        
        Card card = _cardCollection.FindCard(cardName);
        GameObject newCardGO = GameObject.Instantiate(card.gameObject, _playerDeck);
        newCardGO.transform.localRotation = Quaternion.Euler(0, -100, 70);
        Card newCard = newCardGO.GetComponent<Card>();
        newCard.IsInDeck = true;
        SetLayoutSpacing();
        _game.CurrentDeck.AddToDeck(card);
        _game.CurrentDeck.SaveDeck();  // Сохраняем колоду с добавленной картой
        _deckCounter.text = _playerDeck.childCount + "/" + _deckSize;
    }

    private void InitializeCollection() //Заполняем коллекцию карт из SO-библиотеки
    {
        int cardCount=0;

        foreach (GameObject cardObject in _cardCollection.Cards) //Заполняем известными картами
        {
            Card card = cardObject.GetComponent<Card>();
            GameObject newCard = Instantiate(cardObject,_collectionContainer);
            newCard.transform.localRotation = Quaternion.Euler(0, -90, 70);
            card.Initialize(_game);
            cardCount++;
        }

        for (int i = cardCount; i < _collectionSize; i++) //Показываем неисследованные карты
        {
            GameObject newCardGO = Instantiate(_cardCollection.UnknownCard, _collectionContainer);            
            newCardGO.transform.localRotation = Quaternion.Euler(0, -90, 70);
            Card newCard = newCardGO.GetComponent<Card>();
            newCard.Initialize(_game);
        }
    }

    private void InitializeDeck() //Загружаем колоду игрока
    {
        _game.CurrentDeck.LoadDeck();

        for (int i = _playerDeck.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(_playerDeck.GetChild(i));
        }

        foreach (string cardName in _game.CurrentDeck.PlayerDeck)
        {
            Card card = _cardCollection.FindCard(cardName);
            if (card != null)
            {
                GameObject newCardGO = Instantiate(card.GameObject, _playerDeck);
                newCardGO.transform.localRotation = Quaternion.Euler(0, -100, 70);
                Card newCard = newCardGO.GetComponent<Card>();
                newCard.IsInDeck = true;
                newCard.Initialize(_game);
            }
        }
        SetLayoutSpacing();
        _deckCounter.text = _playerDeck.childCount + "/" + _deckSize;
    }

    private void OnSelectCard(Card card) //При нажатии на карту создаем её копию в колоде игрока или удаляем оттуда
    {
        if (card.IsInDeck)
        { 
            _game.CurrentDeck.RemoveFromDeck(card); // Удаление карты
            _game.CurrentDeck.SaveDeck();
            DestroyImmediate(card.gameObject);
            _deckCounter.text = _playerDeck.childCount + "/" + _deckSize;
        }
    }

    private void SetLayoutSpacing() //Настраиваем плотность карт на панели колоды
    {
        if (_playerDeck.childCount <= 11)
            _layoutGroup3D.spacing.x = 5;
        else if (_playerDeck.childCount <= 13)
            _layoutGroup3D.spacing.x = 4;
        else if (_playerDeck.childCount <= 16)
            _layoutGroup3D.spacing.x = 3;
        else if (_playerDeck.childCount <= 19)
            _layoutGroup3D.spacing.x = 2.5f;
        else if (_playerDeck.childCount <= 22)
            _layoutGroup3D.spacing.x = 2;
        else 
            _layoutGroup3D.spacing.x = 1.5f;
    }
}
