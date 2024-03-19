using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDescriptionState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Card _card;
    private readonly CardView _cardView;

    //Это выделенное состояние карты

    public CardDescriptionState(StateMachine stateMachine, Card card, CardView view)
    {
        _stateMachine = stateMachine;
        _card = card;
        _cardView = view;
    }
    public void Enter()
    {
        _card.PointerClick += Deselect;
    }

    public void Exit()
    {
        _card.PointerClick -= Deselect;
    }

    public void Update()
    {

    }

    private void Deselect(PointerEventData eventData)
    {
        _stateMachine.ChangeState(_card.DefaultState);
    }
}