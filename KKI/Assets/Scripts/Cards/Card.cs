using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using AYellowpaper.SerializedCollections;

[RequireComponent(typeof(CardView))]

//��� ������� ����� ��� ���� ����

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{

    [Space]
    [Header("Card game data")]

    [SerializeField] private CardTypes _type;
    [SerializeField] private CardColors _color;
    [SerializeField] private int _actionPointCost = 0;
    [SerializeField] private int _giveCardsCount;
    [SerializeField] private CardEffect _effect;
    [SerializeField] private AnimationConstants _animationName;
    [SerializeField] private bool _isDark = false;
    [SerializeField] private SerializedDictionary<Unit,List<CardEffect>> _personalEffects;

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
    private CardDescriptionState _descriptionState;

    private bool _isInDeck;
    private Vector3 _modelDefaultPosition;
    private Quaternion _modelDefaultRotation;

    public string AnimationName { get => _animationName.ToString(); }
    public int GiveCardsCount { get => _giveCardsCount;  }
    public bool IsDark { get => _isDark; }
    public CardEffect Effect { get => _effect; }
    public CardDefaultState DefaultState => _defaultState;
    public CardSelectState SelectState => _selectState;
    public CardHighlightState HightLightState => _highlightState;
    public CardDescriptionState DescriptionState => _descriptionState;
    public IState CurrentState => _stateMachine.CurrentState;
    public bool PointerEnter => _pointerEnter;
    public int APCost => _actionPointCost;
    public CardColors Color => _color;

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
        _descriptionState = new(_stateMachine, this, _cardView);
        _modelDefaultPosition = _cardModel.transform.localPosition;
        _modelDefaultRotation = _cardModel.transform.localRotation;

        _stateMachine.Initialize(_defaultState);
        if (!_combatManager) _combatManager = FindObjectOfType<CombatManager>();
        if (!_game) _game = FindFirstObjectByType<Game>();
    }

    private void OnDestroy()
    {
        _stateMachine.OnStateChanged -= (state, oldState) => StateChanged?.Invoke(state, oldState, this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_game.CurrentState is GamePlayState) 
        {
            if (_combatManager.ActiveUnit == null) return;
        }
        PointerClick?.Invoke(eventData);
    }

    public List<List<CardEffect>> GetPersonatEffectsList()
    {
        List<List<CardEffect>> personalEffectsGroup = new();
        foreach(List<CardEffect> effects in _personalEffects.Values)
        {
            personalEffectsGroup.Add(effects);
        }
        return personalEffectsGroup;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _pointerEnter = true;
        if (_game.CurrentState is GameDeckBuildState && _game.DeckBuider.HaveActiveCard) return;
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
        if (game == null)
        {
            Debug.Log("CARD INITIALIZATION ERROR");
        }
        
        _game = game;
        float rnd = UnityEngine.Random.Range(0.5f, 1.2f);
        _cardView.Init(this);
        _cardView.Anim.SetFloat("IdleSpeed", rnd);
    }

    public void SetFullView()
    {
        _cardView.SetFullView();
    }

    private void ApplyEffect(CardEffect effect, Unit cardUser, List<Unit> targets=null)
    {
        if (targets == null) {
            targets = new();
            targets.AddRange(_combatManager.EnemyUnits);
        }
        float damageDone = 0;
        foreach (var unit in targets)
        {
            if (effect.MovesCount > 0)  //�������� �������
            {
                unit.AddEffect(effect);
            }
            else                        //���������� �������
            {
                if (effect.Damage > 0 || effect.InitiativeBonus > 0)
                {
                    float pDamage = _effect.Damage * (cardUser.Damage + cardUser.Bonus.Damage);
                    float mDamage = _effect.MDamage * (cardUser.MDamage + cardUser.Bonus.MDamage);
                    damageDone += unit.DealInstantEffect(pDamage, mDamage, 0, 0);
                    
                }
                if (effect.InitiativeBonus > 0)
                {
                    unit.DealInstantEffect(0, 0, 0, effect.InitiativeBonus);
                }
                if (effect.Heal > 0)
                {
                    float heal = effect.Heal * (cardUser.MDamage + cardUser.Bonus.MDamage * cardUser.MDamage);
                    unit.DealInstantEffect(0, 0, heal, 0);
                }
            }
        }
        if (_effect.Vampiric != 0)
        {
            cardUser.DealInstantEffect(0, 0, heal: damageDone * _effect.Vampiric / 100, 0);
        }
        //_stateMachine.ChangeState(_defaultState);
    }

    public void ApplyCardEffects(Unit cardUser, List<Unit> targets = null)
    {
        ApplyEffect(_effect, cardUser, targets);
        foreach(var personalEffectGroup in _personalEffects)
        {
            if(personalEffectGroup.Key.name == cardUser.name)
            {
                foreach (CardEffect personalEffect in personalEffectGroup.Value)
                {
                    if (personalEffect.isAOE)
                    {
                        ApplyEffect(personalEffect, cardUser);
                    }
                    else
                    {
                        ApplyEffect(personalEffect, cardUser, targets);
                    }
                    List<CardTypes> effectTypes = new List<CardTypes> { CardTypes.attackMulti, CardTypes.attackSingle, CardTypes.bonusMulti, CardTypes.bonusSingle };
                    if (effectTypes.Contains(personalEffect.effectType)) 
                    {     
                        cardUser.View.ShowEffectName(personalEffect.EffectName, true);
                    }
                    else
                    {
                        foreach(Unit unit in targets)
                        {
                            unit.View.ShowEffectName(personalEffect.EffectName, false);
                        }
                    }
                        
                }
            }   
        }

        EventBus.Instance.DiscardCard?.Invoke(this);
        EventBus.Instance.DeselectUnits?.Invoke();
        EventBus.Instance.UnitActivationFinished?.Invoke();
        _combatManager.ActionPoints -= _actionPointCost;
        _combatManager.UpdateUI();
        DestroyCard();
    }

    public List<CardEffect> GetPersonalEffect(string unitName)
    {
        List<CardEffect> effects = new();
        foreach (var personalEffectGroup in _personalEffects)
        {
            if (personalEffectGroup.Key.name == unitName)
            {
                return personalEffectGroup.Value;
            }
        }
        return effects;
    }

    public void ToDeck()
    {
        _game.DeckBuider.AddCardToDeck(this.name);
    }

    public void SetState(IState state)
    {
        _stateMachine.ChangeState(state);
    }

    public void DestroyCard()
    {
        if (_game.DeckBuider) _game.DeckBuider.ActivateCard(false);
        Destroy(this.gameObject);
    }
}

public enum CardTypes
{
    attackSingle,
    attackMulti,
    bonusSingle,
    malusSingle,
    bonusMulti,
    malusMulti,
    giveCard
}

public enum EffectTypes
{
    speedUP,
    slowDown,
    damage,
    fire,
    ice,
    shield,
    healOverTime,
    bleeding,
    stun,
    provoke,
    none
}

[Serializable]
public struct CardEffect
{
    public string EffectName;
    public CardTypes effectType;
    public float Damage;
    public float MDamage;
    public float Heal;
    public float InitiativeBonus;
    public float MaxHealthBonus;
    public float InitiativeBoost;
    public float DamageBonus;
    public float DefenceBonus;
    public float MDamageBonus;
    public float MResistBonus;
    public float Vampiric;
    public int MovesCount;
    public int CurrentMovesCount;
    public bool isAOE;
    public EffectTypes type;
}

public enum CardColors
{
    delault,
    red,
    green,
    blue,
    orange,
    yellow,
    violet
}