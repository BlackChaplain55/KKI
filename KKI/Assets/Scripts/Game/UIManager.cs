using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] Game _game;
    [SerializeField] GameObject _selectCellText;

    private void OnValidate()
    {
        if (!_game) _game = GetComponent<Game>();
        if (!_selectCellText) _selectCellText = GameObject.Find("SelectCellText");
    }

    private void Start()
    {
        EventBus.Instance.OnSelectCell?.AddListener(OnSelectCell);
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
