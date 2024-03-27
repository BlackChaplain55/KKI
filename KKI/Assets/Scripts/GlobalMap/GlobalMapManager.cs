using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalMapManager : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private MapCharacter _player;
    [SerializeField] private List<Encounter> _encounters;
    [SerializeField] private ProgressData _progress;

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
        LoadProgress();
        List<string> encounterNames = new();
        encounterNames.AddRange(_progress.CompleteEncounters.Split(','));
        if (_game.Encounter.Name != "") {
            encounterNames.Add(_game.Encounter.Name);
            SaveProgress();
            EncounterData enc = new();
            _game.Encounter = enc;
        }
        foreach (Encounter enc in _encounters)
        {
            if (encounterNames.Contains(enc.Name))
            {
                enc.Init(this, true);
            }
            else
            {
                enc.Init(this, false);
            }
            
        }
    }

    public void SaveProgress()
    {
        _progress.PlayerPosition = _player.transform.position;
        List<string> completeEncounters =new();
        foreach (Encounter enc in _encounters)
        {
            if (enc.IsComplete) completeEncounters.Add(enc.Name);
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
