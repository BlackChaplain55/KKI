using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollectionPuzzle : MonoBehaviour
{
    [SerializeField] private List<ChildTrigger> _triggers;
    private int _index = 0;
    private bool _solved;

    private void Awake()
    {
        foreach (ChildTrigger trigger in _triggers)
        {
            trigger.Init(this);
        }
        _solved = false;
        DropPuzzle();
    }

    public void ChildTriggered(ChildTrigger trigger)
    {
        if (_solved) return;

        int foundIndex = _triggers.IndexOf(trigger);
        if (foundIndex == _index)
        {
            _triggers[_index].SetActive(true);
            _index++;
            if (_index == _triggers.Count)
            {
                _solved = true;
            }
        }
        else
        {
            DropPuzzle();
        }

    }

    private void DropPuzzle()
    {
        foreach (ChildTrigger trigger in _triggers)
        {
            trigger.SetActive(false);
        }
        _index = 0;
        _solved = false;
    }
}
