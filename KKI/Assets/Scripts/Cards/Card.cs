using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(CardView))]

//Это базовый класс для всех карт

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    [Space]
    [Header("Card game data")]

    [SerializeField] private CardTypes _type;
    [SerializeField] private CardEffect _effect;
    [SerializeField] private AnimationConstants _animationName;
    [SerializeField] private bool _isCloseCombat;

    [Space]
    [Header("Tech data")]

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
    private CombatManager _combatManager;

    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private TMP_Text _cardHeader;
    [SerializeField] private TMP_Text _cardDescription;

    private CardDefaultState _defaultState;
    private CardSelectState _selectState;
    private CardHighlightState _highlightState;

    private bool _isInDeck;
    private Vector3 _modelDefaultPosition;
    private Quaternion _modelDefaultRotation;

    public string AnimationName { get => _animationName.ToString(); }
    public CardDefaultState DefaultState => _defaultState;
    public CardSelectState SelectState => _selectState;
    public CardHighlightState HightLightState => _highlightState;
    public IState CurrentState => _stateMachine.CurrentState;
    public bool PointerEnter => _pointerEnter;

    public StateMachine StateMachine => _stateMachine;
    public GameObject GameObject => _gameObject;
    public GameObject CardModel => _cardModel;
    public Vector3 ModelDefaultPosition => _modelDefaultPosition;
    public Quaternion ModelDefaultRotation => _modelDefaultRotation;
    public Game CurrentGame => _game;

    public bool IsInDeck { get => _isInDeck; set => _isInDeck = value; }
    public CardTypes Type => _type;

    private void OnValidate()
    {
        if (!_gameObject) _gameObject = gameObject;
        //if (!_game) _game = FindObjectOfType<Game>();0
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
        _combatManager = FindObjectOfType<CombatManager>();
    }

    private void OnDestroy()
    {
        _stateMachine.OnStateChanged -= (state, oldState) => StateChanged?.Invoke(state, oldState, this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_combatManager.ActiveUnit == null) return;
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

    public void Initialize(Game game)
    {
        _game = game;
    }

    public void ApplyEffect(List<Unit> targets=null)
    {
        if (targets == null) {
            targets = new();
            targets.AddRange(_combatManager.EnemyUnits);
        } 
        foreach (var unit in targets)
        {
            if (_effect.Damage > 0) unit.DealInstantEffect(-_effect.Damage*_combatManager.ActiveUnit.Damage, _effect.InitiativeBonus);
            if (_effect.Heal > 0) unit.DealInstantEffect(_effect.Heal, _effect.InitiativeBonus);
            if(_effect.MovesCount>0) unit.AddEffect(_effect);
        }
        EventBus.Instance.DeselectUnits?.Invoke();
        EventBus.Instance.UnitActivationFinished?.Invoke();
        _stateMachine.ChangeState(_defaultState);
    }
}

public enum CardTypes
{
    attackSingle,
    attackMulti,
    bonusSingle,
    malusSingle,
    bonusMulti,
    malusMulti
}

[Serializable]
public struct CardEffect
{
    public float Damage;
    public float Heal;
    public float InitiativeBonus;
    public float MaxHealthBonus;
    public float MaxInitiativeBonus;
    public float DamageBonus;
    public float DefenceBonus;
    public int MovesCount;
    public int CurrentMovesCount;
}