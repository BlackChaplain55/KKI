using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelectState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Card _card;
    private readonly CardView _cardView;

    //Это выделенное состояние карты

    public CardSelectState(StateMachine stateMachine, Card card, CardView view)
    {
        _stateMachine = stateMachine;
        _card = card;
        _cardView = view;
    }
    public void Enter()
    {
        if(_card.CurrentGame.CurrentState is GameDeckBuildState)
        {
            
        }
        else
        {
            _cardView.SelectedVFX.gameObject.SetActive(true);
            _cardView.SelectedVFX.Play(); //Эффект "горения" карты
        }
        EventBus.Instance.ActivateCard?.Invoke(_card);
        _card.PointerClick += Deselect;
        
    }

    public void Exit()
    {
        _cardView.SelectedVFX.Stop();
        _card.PointerClick -= Deselect;
    }

    public void Update()
    {

    }

    private void Deselect(PointerEventData eventData)
    {
        _stateMachine.ChangeState(_card.HightLightState);
    }
}