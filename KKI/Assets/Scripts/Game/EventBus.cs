using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//���� ������� ��� ���������� �������
public class EventBus : MonoBehaviour
{
    public UnityEvent<Unit> OnSelectUnit;
    public UnityEvent DeselectUnits;
    public UnityEvent<Unit> ActivateUnit;
    public UnityEvent<Unit,PlayerUnit> UnitDeath;
    public UnityEvent UnitActivationFinished;
    public UnityEvent<Card> ActivateCard;
    public UnityEvent<Card> DiscardCard;
    public UnityEvent OnMapTrigger;
    public UnityEvent Tick;

    public static EventBus Instance;

    void Awake()
    {
        Instance = this;
    }
}
