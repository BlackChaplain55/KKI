using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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
        if (_card.CurrentGame.CurrentState is GamePlayState)
        {
            if (_card.Type == CardTypes.attackMulti || _card.Type == CardTypes.bonusMulti || _card.Type == CardTypes.malusMulti)
            {
                EventBus.Instance.ActivateCard?.Invoke(_card);
            }
            else
            {
                _stateMachine.ChangeState(_card.SelectState);
            }
        }
    }
}