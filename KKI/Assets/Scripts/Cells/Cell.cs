using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector2Int _position;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private Game _game;

    public event PointerStateHandler PointerChanged;
    public event StateHandler StateChanged;
    public event PointerHandler PointerClick;

    public delegate void PointerStateHandler(bool pointerEnter, Cell sender);
    public delegate void PointerHandler(PointerEventData eventData);
    public delegate void StateHandler(IState state, IState oldState, Cell sender);

    private bool _pointerEnter;

    [SerializeField] private StateMachine _stateMachine;

    private CellDefaultState _defaultState;
    private CellSelectState _selectState;
    private CellHighlightState _highlightState;

    private Unit _unit;
    private bool _haveUnit;
    //private Transform _transform;

    public CellDefaultState DefaultState => _defaultState;
    public CellSelectState SelectState => _selectState;
    public IState CurrentState => _stateMachine.CurrentState;
    Vector2Int Position => _position;
    public bool PointerEnter => _pointerEnter;

    public StateMachine StateMachine => _stateMachine;
    public GameObject GameObject => _gameObject;

    public bool HaveUnit => _haveUnit;

    public Unit Unit => _unit;


    private void Start()
    {

    }

    private void OnValidate()
    {
        if (!_gameObject) _gameObject = gameObject;
        if (!_game) _game = FindObjectOfType<Game>();
    }

    private void Awake()
    {
        _stateMachine = new StateMachine();
        _stateMachine.OnStateChanged += (state, oldState) => StateChanged?.Invoke(state, oldState, this);

        _defaultState = new(_stateMachine,this);
        _selectState = new(_stateMachine, this);
        _highlightState = new(_stateMachine, this);

        _stateMachine.Initialize(_defaultState);
    }

    private void OnDestroy()
    {
        _stateMachine.OnStateChanged -= (state, oldState) => StateChanged?.Invoke(state, oldState, this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClick?.Invoke(eventData);
        EventBus.Instance.OnSelectCell?.Invoke(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _pointerEnter = true;
        //HighlightUnit(true);
        PointerChanged?.Invoke(_pointerEnter, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pointerEnter = false;
        //HighlightUnit(false);

        PointerChanged?.Invoke(_pointerEnter, this);
    }

    public void Deselect()
    {
        //if(_haveUnit) ShowMoves(false);
        if (CurrentState is CellSelectState|| CurrentState is CellHighlightState)
        {
            _stateMachine.ChangeState(_defaultState);
        }
    }

    public void Initialize(Vector2Int position)
    {
        _position = position;
    }

    public void SetUnit(Unit unit = null)
    {
        if (!unit)
        {
            _unit = null;
            _haveUnit = false;
        }
        else
        {
            unit.Transform.SetPositionAndRotation(transform.position, Quaternion.Euler(0,180,0));
            _unit = unit;
            _haveUnit = true;
        }
    }

    public void HighlightUnit(bool state)
    {
        if (_haveUnit)
        {
            _unit.SetHighlight(state);
        }
    }

    public void SetMoving()
    {
        _stateMachine.ChangeState(_highlightState);
        //StateChanged?.Invoke(_highlightState, _defaultState, this);
    }

    public void ShowMoves(bool state)
    {
        int moveRadius = 0;

        if (_unit)
        {
            HighlightUnit(true);
            moveRadius = _unit.MoveRadius;
        }
        else
        {
            return;
        }


        Collider[] colliders = Physics.OverlapSphere(transform.position, moveRadius * _game.CellStep, 1 << LayerMask.NameToLayer("Cell"));

        foreach (Collider collider in colliders)
        {
            Cell currentCell = collider.GetComponent<Cell>();
            if (state)
            {
                currentCell.SetMoving();
            }
            else
            {
                HighlightUnit(false);
                currentCell.Deselect();
            }
        }
    }
}