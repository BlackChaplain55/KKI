using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это подсвеченное состояние ячейки

public class CellHighlightState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Cell _cell;

    public CellHighlightState(StateMachine stateMachine, Cell cell)
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

    }
}