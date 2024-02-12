using UnityEditor;
using UnityEngine;
using TMPro;

//Этот класс отвечает за визуал ячейки тактического поля
public class CellView : MonoBehaviour
{
    [SerializeField] private Color _defaultColor = Color.green;
    [SerializeField] private Color _enterColor = Color.yellow;
    [SerializeField] private Color _selectColor = Color.yellow;
    [SerializeField] private Color _selectEnterColor = Color.red;
    [SerializeField] private Color _highlightColor = Color.green;
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Cell _cell;
    [SerializeField] private Game _game;
    [SerializeField] private TMP_Text _debugText;

    private void OnEnable()
    {
        _cell.StateChanged += (state, oldState, sender) => StateChanged(state, sender.PointerEnter);
        _cell.PointerChanged += (pointerEnter, sender) => StateChanged(sender.CurrentState, pointerEnter);
        _meshRenderer.material.color = _defaultColor;
    }

    private void OnDisable()
    {
        _cell.StateChanged -= (state, oldState, sender) => StateChanged(state, sender.PointerEnter);
        _cell.PointerChanged -= (pointerEnter, sender) => StateChanged(sender.CurrentState, pointerEnter);
    }

    private void OnValidate()
    {
        if (!_meshRenderer) _meshRenderer = GetComponent<MeshRenderer>();
        if (!_cell) _cell = GetComponent<Cell>();
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_debugText) _debugText = transform.GetComponentInChildren<TMP_Text>();
    }

    private void SetColorByState(IState cellState, bool pointerEnter) //Перекраска ячейки при смене состояний
    {
        _debugText.text = cellState.ToString();
        var haveSelectedUnit = _game.HaveSelectedUnit;
        _meshRenderer.material.color = (cellState, pointerEnter) switch
        {
            (CellDefaultState, true) => _enterColor,
            (CellDefaultState, _) => _defaultColor,
            (CellSelectState, true) => _selectEnterColor,
            (CellSelectState, _) => _selectColor,
            (CellHighlightState, true) => _selectEnterColor,
            (CellHighlightState, _) => _highlightColor,
        };
    }

    private void StateChanged(IState cellState, bool pointerEnter)
    {
        SetColorByState(cellState, pointerEnter);
    }


}