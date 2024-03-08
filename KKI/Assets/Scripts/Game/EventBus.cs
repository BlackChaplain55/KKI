using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Ўина событий дл€ глобальных событий
public class EventBus : MonoBehaviour
{
    public UnityEvent<Unit> OnSelectUnit;
    public UnityEvent DeselectUnits;
    public UnityEvent<Unit> ActivateUnit;
    public UnityEvent UnitActivationFinished;
    public UnityEvent<Card> OnSelectCard;
    public UnityEvent OnMapTrigger;
    public UnityEvent Tick;

    public static EventBus Instance;

    void Awake()
    {
        Instance = this;
    }
}
