using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Этот класс описывает Юнита
public class Unit : MonoBehaviour
{
    [Header("Unit stats")]
    [SerializeField] private Vector2Int[] _moves;
    [SerializeField] private Vector2Int[] _attackMoves;
    [SerializeField] private int _moveRadius;
    [SerializeField] private int _health;
    [SerializeField] private int _currentHealth;

    [Space]
    [Header("Unit Data")]

    [SerializeField] private Cell _cell;

    [SerializeField] private Color _moveColor = Color.green;
    [SerializeField] private Color _attackColor = Color.red;
    [SerializeField] private Color _universalColor = Color.yellow;

    [SerializeField] private GameObject _gameObject;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private Transform _transform;

    public Transform Transform => _transform;
    public int MoveRadius => _moveRadius;

    public Vector2Int[] Moves => _moves;
    public Vector2Int[] AttackMoves => _attackMoves;
    // Start is called before the first frame update

    private void OnValidate()
    {
        _transform = transform;
        Transform spotTransform = transform.Find("Spot");
        if(spotTransform) _highlight = spotTransform.gameObject;
    }
 
    public void FindCells()
    {
        Collider[] colliders = Physics.OverlapBox(_transform.position, _transform.localScale, Quaternion.identity, 1<< LayerMask.NameToLayer("Cell"));

        if (colliders.Length == 0) return;

        Cell cell = colliders[0].GetComponent<Cell>();
        cell.SetUnit(this);
        _cell = cell;
    }

    public void SetCell(Cell cell)
    {
        _cell = cell;
    }

    public void SetHighlight(bool state)
    {
        if(_highlight) _highlight.SetActive(state);
    }
}
