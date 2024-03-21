using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _activeState;
    [SerializeField] private GameObject _passiveState;
    [SerializeField] private TriggerCollectionPuzzle _puzzleParent;
    private bool _active;

    public void Init(TriggerCollectionPuzzle puzzleParent)
    {
        _puzzleParent = puzzleParent;
        _active = false;
    }

    public void SetActive(bool state)
    {
        _active = state;
        if (state)
        {
            _activeState.SetActive(true);
            _passiveState.SetActive(false);
        }
        else
        {
            _activeState.SetActive(false);
            _passiveState.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!_active) _puzzleParent.ChildTriggered(this);
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
