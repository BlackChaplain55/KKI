using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Это класс отвечает за интерфес в основном игровом режиме

public class CombatUIManager : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private GameObject _selectCellText;
    [SerializeField] private GameObject _SpawnButton;

    private void OnValidate()
    {
        if (!_game) _game = GetComponent<Game>();
        if (!_selectCellText) _selectCellText = GameObject.Find("SelectCellText");
    }

    private void Start()
    {
        EventBus.Instance.OnSelectCell?.AddListener(OnSelectCell);
    }

    public void Initialize()
    {
        _SpawnButton.SetActive(true);
    }

    public void Exit()
    {
        _SpawnButton.SetActive(false);
    }

    public void SpawnUnit(Unit unitPrefab)
    {
        _selectCellText.SetActive(true);
        _game.BeginSpawnUnit(unitPrefab);
    }

    private void OnSelectCell(Cell cell)
    {
        _selectCellText.SetActive(false);
    }
}
