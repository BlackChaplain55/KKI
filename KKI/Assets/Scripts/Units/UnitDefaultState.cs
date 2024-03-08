using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitDefaultState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Unit _unit;
    private readonly UnitView _unitView;

    //Это изначальное состояние карты

    public UnitDefaultState(StateMachine stateMachine, Unit unit, UnitView view)
    {
        _stateMachine = stateMachine;
        _unit = unit;
        _unitView = view;
    }
    public void Enter()
    {

    }

    public void Exit()
    {

    }

    public void Update()
    {
    
    }
}
