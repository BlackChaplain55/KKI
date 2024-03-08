using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Этот класс описывает Юнита
[RequireComponent(typeof(UnitView))]
public class PlayerUnit : Unit, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        EventBus.Instance.DeselectUnits?.Invoke();
        _stateMachine.ChangeState(SelectState);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_stateMachine.CurrentState == DefaultState)
        {
            _stateMachine.ChangeState(HighlightState);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_stateMachine.CurrentState == HighlightState)
        {
            _stateMachine.ChangeState(DefaultState);
        }
    }

    public void SelectUnit()
    {
        EventBus.Instance.DeselectUnits?.Invoke();
        _stateMachine.ChangeState(HighlightState);
    }

    public void DeselectUnit()
    {
        _stateMachine.ChangeState(DefaultState);
    }

    new public void Activate()
    {
        Debug.Log("Hero unit " + Name + " is activated!");
        SetUnitAnimation("Slash", true, isTrigger: true);
    }
}
