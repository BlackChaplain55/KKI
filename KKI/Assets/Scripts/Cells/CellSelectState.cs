using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellSelectState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Cell _cell;

    public CellSelectState(StateMachine stateMachine, Cell cell)
    {
        _stateMachine = stateMachine;
        _cell = cell;
    }
    public void Enter()
    {
        _cell.PointerClick += Deselect;
    }

    public void Exit()
    {
        _cell.PointerClick -= Deselect;
    }

    public void Update()
    {

    }

    private void Deselect(PointerEventData eventData)
    {
        _stateMachine.ChangeState(_cell.DefaultState);
    }
}