using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Этот класс описывает Юнита
[RequireComponent(typeof(UnitView))]
public class PlayerUnit : Unit, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    new public void OnPointerClick(PointerEventData eventData)
    {
        if (_stateMachine.CurrentState == DefaultState)
        {
            _stateMachine.ChangeState(HighlightState);
        }

        if (Combat.ActiveCard != null)
        {
            Combat.ActiveUnit.SetUnitAnimation(Combat.ActiveCard.AnimationName, true, true);
            Combat.CurrentTarget = this;
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

    override public void Death()
    {
        EventBus.Instance.UnitDeath?.Invoke(null, this);
        EventBus.Instance.SFXPlay?.Invoke(SFXClipsTypes.Death);
        SetUnitAnimation(AnimationConstants.Death.ToString(), true);
    }
}
