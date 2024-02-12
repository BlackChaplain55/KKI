using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventBus : MonoBehaviour
{
    public UnityEvent<Unit> OnSelectUnit;
    public UnityEvent<Cell> OnSelectCell;
    public UnityEvent<Card> OnSelectCard;

    public static EventBus Instance;

    void Awake()
    {
        Instance = this;
    }
}
