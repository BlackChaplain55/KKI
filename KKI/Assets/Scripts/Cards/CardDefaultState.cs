using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardDefaultState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Card _card;
    private readonly CardView _cardView;

    //Это изначальное состояние карты

    public CardDefaultState(StateMachine stateMachine, Card card, CardView view)
    {
        _stateMachine = stateMachine;
        _card = card;
        _cardView = view;
    }
    public void Enter()
    {
        _card.PointerClick += Select;
        _cardView.Anim.SetBool("Highlight", false);  // Делаем карту неактивной, если она таковой была
    }

    public void Exit()
    {
        _card.PointerClick -= Select;
    }

    public void Update()
    {
    
    }

    private void Select(PointerEventData eventData)
    {
        _stateMachine.ChangeState(_card.SelectState);
    }
}
