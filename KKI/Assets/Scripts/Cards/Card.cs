using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(CardView))]

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Vector2Int _position;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private GameObject _cardModel;
    [SerializeField] private Game _game;
    [SerializeField] private CardView _cardView;

    public event PointerStateHandler PointerChanged;
    public event StateHandler StateChanged;
    public event PointerHandler PointerClick;

    public delegate void PointerStateHandler(bool pointerEnter, Card sender);
    public delegate void PointerHandler(PointerEventData eventData);
    public delegate void StateHandler(IState state, IState oldState, Card sender);

    private bool _pointerEnter;

    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private TMP_Text _cardHeader;
    [SerializeField] private TMP_Text _cardDescription;

    private CardDefaultState _defaultState;
    private CardSelectState _selectState;
    private CardHighlightState _highlightState;

    private Unit _unit;
    private bool _haveUnit;
    private bool _isInDeck;
    private Vector3 _modelDefaultPosition;
    private Quaternion _modelDefaultRotation;

    public CardDefaultState DefaultState => _defaultState;
    public CardSelectState SelectState => _selectState;
    public CardHighlightState HightLightState => _highlightState;
    public IState CurrentState => _stateMachine.CurrentState;
    Vector2Int Position => _position;
    public bool PointerEnter => _pointerEnter;

    public StateMachine StateMachine => _stateMachine;
    public GameObject GameObject => _gameObject;
    public GameObject CardModel => _cardModel;
    public Vector3 ModelDefaultPosition => _modelDefaultPosition;
    public Quaternion ModelDefaultRotation => _modelDefaultRotation;


    public Game CurrentGame => _game;

    public bool HaveUnit => _haveUnit;
    public bool IsInDeck { get => _isInDeck; set => _isInDeck = value; } 

    public Unit Unit => _unit;

    private void OnValidate()
    {
        if (!_gameObject) _gameObject = gameObject;
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_cardView) _cardView = GetComponent<CardView>();
        if (!_cardModel) _cardModel = transform.Find("CardModel").gameObject;
    }

    private void Awake()
    {
        _stateMachine = new StateMachine();
        _stateMachine.OnStateChanged += (state, oldState) => StateChanged?.Invoke(state, oldState, this);

        _defaultState = new(_stateMachine,this, _cardView);
        _selectState = new(_stateMachine, this, _cardView);
        _highlightState = new(_stateMachine, this, _cardView);
        _modelDefaultPosition = _cardModel.transform.localPosition;
        _modelDefaultRotation = _cardModel.transform.localRotation;

        _stateMachine.Initialize(_defaultState);
    }

    private void OnDestroy()
    {
        _stateMachine.OnStateChanged -= (state, oldState) => StateChanged?.Invoke(state, oldState, this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClick?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _pointerEnter = true;
        if (_stateMachine.CurrentState is CardDefaultState) _stateMachine.ChangeState(_highlightState);
        PointerChanged?.Invoke(_pointerEnter, this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _pointerEnter = false;
        if(_stateMachine.CurrentState is CardHighlightState) _stateMachine.ChangeState(_defaultState);
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

    public void ShowEffectArea(bool state)
    {
        int moveRadius = 0;

        if (_unit)
        {
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
                currentCell.Deselect();
            }
        }
    }
}