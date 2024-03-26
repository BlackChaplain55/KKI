using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    [SerializeField] private EncounterData _encData;
    [SerializeField] private Game _game;
    private bool _isStarted = false;
    private bool _isConfirmed = false;
    private bool _isComplete;

    public bool IsComplete { get => _isComplete; }

    private void Awake()
    {
        _game = FindObjectOfType<Game>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_isStarted && !_isComplete)
            {
                _isStarted = true;
                //BeginEncounter();
                _game.MainMenu.ShowConfirmationWindow(_encData.EncounterBeginText, true);
                EventBus.Instance.Confirm.AddListener(OnConfirm);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _isStarted = false;
            _game.MainMenu.ShowConfirmationWindow("", false);
            EventBus.Instance.Confirm.RemoveListener(OnConfirm);
        }
    }

    public void Init(bool complete)
    {
        _isComplete = false;
    }

    private void OnConfirm()
    {
        BeginEncounter();
    }

    private IEnumerator WaitForConfirmation()
    {
        while (!_isComplete)
        {
            yield return null;
        }

        BeginEncounter();
    }

    private void BeginEncounter()
    {
        _game.MainMenu.GameStart(_encData);
    }
}

[Serializable]
public struct EncounterData
{
    public string Name;
    public List<GameObject> Enemies;
    public string EncounterBeginText;
    public string EncounterVictoryText;
    public GameObject VictoryCard;
}

