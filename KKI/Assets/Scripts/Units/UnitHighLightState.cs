using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitHighlightState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Unit _unit;
    private readonly UnitView _unitView;

    //Это изначальное состояние карты

    public UnitHighlightState(StateMachine stateMachine, Unit unit, UnitView view)
    {
        _stateMachine = stateMachine;
        _unit = unit;
        _unitView = view;
    }
    public void Enter()
    {
        _unitView.SetHighlight(true);
    }

    public void Exit()
    {
        _unitView.SetHighlight(false);
    }

    public void Update()
    {

    }
}
