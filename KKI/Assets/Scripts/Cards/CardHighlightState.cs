using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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
        _cardView.Anim.SetBool("Highlight", true);
    }

    public void Exit()
    {
        _card.PointerClick -= Select;
        if(_stateMachine.CurrentState is CardDefaultState)
        {
            _cardView.Anim.SetBool("Highlight", false);
        }
    }

    public void Update()
    {

    }

    private void Select(PointerEventData eventData)
    {
        EventBus.Instance.OnSelectCard?.Invoke(_card);
        if (_card.CurrentGame.CurrentState is GamePlayState)
        {
            _stateMachine.ChangeState(_card.SelectState);
        }
    }
}