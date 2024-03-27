using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

//Это подсвеченное состояние карты

public class CardHighlightState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Card _card;
    private readonly CardView _cardView;

    public CardHighlightState(StateMachine stateMachine, Card card, CardView view)
    {
        _stateMachine = stateMachine;
        _card = card;
        _cardView = view;
    }
    public void Enter()
    {
        _card.PointerClick += Select;
        _cardView.Anim.SetBool("Highlight", true); // Анимация вылета карты со своего места
    }

    public void Exit()
    {
        _card.PointerClick -= Select;
        if(_stateMachine.CurrentState is CardDefaultState)
        {
            _cardView.Anim.SetBool("Highlight", false); // Анимация возвращения карты на место
        }
    }

    public void Update()
    {

    }

    private void Select(PointerEventData eventData)
    {
        if (_card.CurrentGame.CurrentState is GamePlayState && eventData.button == PointerEventData.InputButton.Left)
        {
            if (_card.APCost > _card.CurrentGame.ActionPoints)
            {
                return;
            }
            if (_card.Type == CardTypes.giveCard)
            {
                List<string> newCards = _card.CurrentGame.CurrentDeck.GetRandomCards(_card.GiveCardsCount);
                _card.CurrentGame.CurrentDeck.AddToHand(newCards);
                _card.CurrentGame.InstantinateCardsToDeck(newCards);
            }
            if (_card.Type == CardTypes.attackMulti || _card.Type == CardTypes.bonusMulti || _card.Type == CardTypes.malusMulti)
            {
                EventBus.Instance.ActivateCard?.Invoke(_card);
            }
            else
            {
                _stateMachine.ChangeState(_card.SelectState);
            }
        }
        else if (_card.CurrentGame.CurrentState is GameDeckBuildState|| (_card.CurrentGame.CurrentState is GamePlayState&&eventData.button==PointerEventData.InputButton.Right))
        {
            if (_card.CurrentGame.CurrentState is GameDeckBuildState&&_card.IsInDeck) EventBus.Instance.ActivateCard?.Invoke(_card);
            else
            if (_card.name.Contains("Unknown")) return;
            else
            {
                Card cardPrefab = _card.CurrentGame.CardCollection.FindCard(_card.name);
                GameObject cardFullViewGO = GameObject.Instantiate(cardPrefab.gameObject, _card.transform.parent.parent);
                Card cardFullView = cardFullViewGO.GetComponent<Card>();
                cardFullView.Initialize(_card.CurrentGame);
                cardFullView.SetState(cardFullView.DescriptionState);
                cardFullViewGO.transform.SetPositionAndRotation(_card.transform.position, _card.transform.rotation);
                cardFullViewGO.transform.DOScale(3, 1);
                Vector3 fullViewPosition = new Vector3(5, 5, -5);
                bool playState = false;
                if (_card.CurrentGame.CurrentState is GamePlayState)
                {
                    fullViewPosition = _card.CurrentGame.Combat.FullViewPosition.position;
                    playState = true;
                }
                cardFullViewGO.transform.DOMove(fullViewPosition, 0.5f).OnComplete(() => {
                    cardFullView.SetFullView(playState);
                });
                if (_card.CurrentGame.CurrentState is GameDeckBuildState)
                    _card.CurrentGame.DeckBuider.ActivateCard(true);
            }  
        }
    }
}