using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMapManager : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private MapCharacter _player;
    [SerializeField] private List<Encounter> _encounters;
    [SerializeField] private ProgressData _progress;


    public ProgressData Progress { get => _progress; }
    public MapCharacter Player { get => _player; }

    private void OnValidate()
    {
        if (!_player) _player = GameObject.FindObjectOfType<MapCharacter>();
        _encounters = new List<Encounter>();
        _encounters.AddRange(FindObjectsByType<Encounter>(FindObjectsSortMode.None));
    }
    public void Init(Game game)
    {
        if (!_game) _game = game;
        if (!_player) _player = GameObject.FindObjectOfType<MapCharacter>();
        _encounters = new List<Encounter>();
        _encounters.AddRange(FindObjectsByType<Encounter>(FindObjectsSortMode.None));
        
        if (_game.Encounter.Name != "") {
            if (_game.Encounter.IsComplete)
            {
                if (_game.Encounter.GiveBastet) _progress.Bastet = true;
                if (_game.Encounter.GiveThoth) _progress.Thoth = true;
                if (_game.Encounter.GiveGeb) _progress.Geb = true;
                if (_game.Encounter.GiveMeritseger) _progress.Meritseger = true;
                _progress.InitialDeckBonus += _game.Encounter.InitialDeckBonus;
                _progress.TurnCardBonus += _game.Encounter.TurnCardBonus;
                _progress.InitialAP += _game.Encounter.InitialAP;
                _progress.TurnAPBonus += _game.Encounter.TurnAPBonus;
                //_progress.PlayerPosition = _game.Encounter.SavePoint;
                SaveProgress(_game.Encounter.Name, _game.Encounter.SavePoint);
                EncounterData enc = new();
                _game.Encounter = enc;
            }
            //CompleteEncounterNames.Add(_game.Encounter.Name);
        }
        LoadProgress();
        List<string> CompleteEncounterNames = new();
        CompleteEncounterNames.AddRange(_progress.CompleteEncounters.Split(','));
        foreach (Encounter enc in _encounters)
        {            
            if (CompleteEncounterNames.Contains(enc.Name))
            {
                enc.Init(this, true,_game);
            }
            else
            {
                enc.Init(this, false, _game);
            }
            
        }
    }

    public void SaveProgress(string addCompleteEncounter, Vector3 savePosition)
    {
        if (savePosition != Vector3.zero)
        {
            _progress.PlayerPosition = savePosition;
        }
        else
        {
            _progress.PlayerPosition = _player.transform.position;
        }
        List<string> completeEncounters =new();
        foreach (Encounter enc in _encounters)
        {
            if (enc.IsComplete) completeEncounters.Add(enc.Name);
        }
        if (addCompleteEncounter != "")
        {
            completeEncounters.Add(addCompleteEncounter);
        }
        _progress.CompleteEncounters = string.Join(',', completeEncounters);
        SaveLoadManager.SaveProgresData(_progress);
    }

    public void LoadProgress()
    {
        ProgressData progress = SaveLoadManager.LoadProgres();
        _player.transform.position = progress.PlayerPosition;
        _progress = progress;
    }

    [ContextMenu("Drop progress")]
    public void DropProgress()
    {
        _progress.PlayerPosition = new Vector3(15.16f, 4.201f, 175.8f);
        _progress.CompleteEncounters = "";
        SaveLoadManager.SaveProgresData(_progress);
    }
}
