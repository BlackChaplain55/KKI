using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter : MonoBehaviour
{
    [SerializeField] private EncounterData _encData;
    [SerializeField] private Game _game;
    [SerializeField] private GlobalMapManager _globalMapManager;
    [SerializeField] private GameObject _blocker;
    private bool _isStarted = false;
    private bool _isConfirmed = false;

    public bool IsComplete { get => _encData.IsComplete; }
    public string Name { get => _encData.Name; }


    private void OnValidate()
    {
        if (!_game) _game = FindObjectOfType<Game>();
    }
    private void Awake()
    {
        if(!_game) _game = FindObjectOfType<Game>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!_isStarted && !_encData.IsComplete)
            {
                _isStarted = true;
                //BeginEncounter();
                _game.MainMenu.ShowConfirmationWindow(_encData.EncounterBeginText, true);
                _globalMapManager.Player.SetInputBlocked(true);
                EventBus.Instance.Confirm.AddListener(OnConfirm);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(DelayedTriggerExit());
        }
    }

    private IEnumerator DelayedTriggerExit()
    {
        yield return new WaitForSeconds(0.5f);

        _isStarted = false;
        _game.MainMenu.ShowConfirmationWindow("", false);
        _globalMapManager.Player.SetInputBlocked(false);
        EventBus.Instance.Confirm.RemoveListener(OnConfirm);
    }

    public void Init(GlobalMapManager gmManager, bool complete)
    {
        //_isComplete = false;
        _globalMapManager = gmManager;
        _encData.IsComplete = complete;

        if (complete)
        {
            gameObject.SetActive(false);
        }
    }

    public void CompleteEncounter()
    {
        //_isComplete = true;
        _encData.IsComplete = true;
        _blocker.SetActive(false);
    }

    private void OnConfirm()
    {
        _globalMapManager.SaveProgress();
        BeginEncounter();
    }

    private void BeginEncounter()
    {
        _game.MainMenu.GameStart(_encData);
        _game.Progress = _globalMapManager.Progress;
    }
}

[Serializable]
public struct EncounterData
{
    public string Name;
    public List<GameObject> Enemies;
    public string EncounterBeginText;
    public Sprite InitialScreen;
    public Sprite VictoryScreen;
    public AudioClip InitialClip;
    public AudioClip VictoryClip;
    public string EncounterVictoryText;
    public GameObject VictoryCard;
    public bool IsComplete;
}

