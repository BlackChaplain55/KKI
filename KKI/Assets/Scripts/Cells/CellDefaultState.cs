using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это изначальное состояние ячейки по умолчанию

public class CellDefaultState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Cell _cell;

    public CellDefaultState(StateMachine stateMachine, Cell cell)
    {
        _stateMachine = stateMachine;
        _cell = cell;
    }
    public void Enter()
    {
        _cell.PointerClick += Select;
    }

    public void Exit()
    {
        _cell.PointerClick -= Select;
    }

    public void Update()
    {
    
    }

    private void Select(PointerEventData eventData)
    {
        _stateMachine.ChangeState(_cell.SelectState);
    }
}
