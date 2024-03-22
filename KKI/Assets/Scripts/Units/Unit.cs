using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

//Этот класс описывает Юнита
[RequireComponent(typeof(UnitView))]
[RequireComponent(typeof(UnitEffects))]
public class Unit : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Unit stats")]
    [SerializeField] private string _name;
    [SerializeField] private float _health;
    [SerializeField] private float _initiative;
    [SerializeField] private float _damage;
    [SerializeField] private float _defence;
    [SerializeField] private float _magicPower;
    [SerializeField] private float _magicResist;
    [SerializeField] private float _archer;
    [SerializeField] private float _closeCombat;

    [Space]
    [Header("Unit Data")]
    [SerializeField] private UnitEffects _effects;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private Transform _transform;
    [SerializeField] private Animator _anim;
    [SerializeField] private UnitView _view;
    [SerializeField] private SimpleAI _AI;

    [Space]
    [Header("Runtime stats")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _currentInitiative;
    [SerializeField] private StatsBonus _bonus;
    [SerializeField] private PlayerUnit _AItarget;
    [SerializeField] private Unit _playerTarget;

    private CombatManager _combatManager;

    public StateMachine _stateMachine;
    private UnitDefaultState _defaultState;
    private UnitSelectState _selectState;
    private UnitHighlightState _highlightState;

    public event PointerStateHandler PointerChanged;
    public event StateHandler StateChanged;
    public event PointerHandler PointerClick;

    public delegate void PointerStateHandler(bool pointerEnter, Unit sender);
    public delegate void PointerHandler(PointerEventData eventData);
    public delegate void StateHandler(IState state, IState oldState, Unit sender);

    public UnitDefaultState DefaultState => _defaultState;
    public UnitSelectState SelectState => _selectState;
    public UnitHighlightState HighlightState => _highlightState;
    public Transform Transform => _transform;

    public CombatManager Combat { get => _combatManager; }
    public UnitView View { get => _view; }
    public string Name { get => _name; }
    public float Damage { get => _damage; }
    public float Defence { get => _defence; }
    public float MaxHealth { get => _health+_bonus.Health;}
    public float MaxInitiative { get => _initiative-_bonus.Initiative; }
    public float CurrentHealth { get => _currentHealth; }
    public float CurrentInitiative { get => _currentInitiative; }
    // Start is called before the first frame update

    private void Awake()
    {
        Init(FindObjectOfType<CombatManager>());
    }

    private void OnValidate()
    {
        if (!_anim) _anim = GetComponent<Animator>();
        if(!_transform) _transform = transform;
        if (!_view) _view = GetComponent<UnitView>();
        if (!_AI) _AI = GetComponent<SimpleAI>();
        if (_effects==null) _effects = GetComponent<UnitEffects>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_combatManager.ActiveCard != null)
        {
            _combatManager.ActiveUnit.SetUnitAnimation(_combatManager.ActiveCard.AnimationName,true,true);
            _combatManager.CurrentTarget = this;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_stateMachine.CurrentState == DefaultState)
        {
            _stateMachine.ChangeState(HighlightState);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_stateMachine.CurrentState == HighlightState)
        {
            _stateMachine.ChangeState(DefaultState);
        }
    }

    public void Activate()
    {
        _effects.CheckEffects();
        _bonus = _effects.SetBonus();
        _view.UpdateUI();
        if (_AI)
        {
            Debug.Log("Enemy unit " + _name + " is activated!");
            _AItarget = _AI.PickTarget();
            SetUnitAnimation("Slash", true, isTrigger: true);
        }
        else
        {
            Debug.Log("Hero unit " + _name + " is activated!");
            EventBus.Instance.ActivateUnit?.Invoke(this);
            _stateMachine.ChangeState(_selectState);
        }
    }

    public void AnimationFinished()
    {
        if (_AI)
        {
            _AItarget.DealInstantEffect(-_damage, 0);
            Debug.Log("Enemy unit " + _name + " hit " + _AItarget + " with " + _damage);
            _AItarget = null;
            EventBus.Instance.UnitActivationFinished?.Invoke();
        }
        else
        {      
            if(_combatManager.ActiveCard.Type == CardTypes.attackSingle||
                _combatManager.ActiveCard.Type == CardTypes.bonusSingle||
                _combatManager.ActiveCard.Type == CardTypes.malusSingle)
            {
                List < Unit > targets = new List<Unit>();
                targets.Add(_combatManager.CurrentTarget);
                _combatManager.ActiveCard.ApplyCardEffects(this, targets);
            }
            else
            {
                _combatManager.ActiveCard.ApplyCardEffects(this);
            }
            _combatManager.CurrentTarget = null;
            EventBus.Instance.UnitActivationFinished?.Invoke();
        }
    }

    public void SetUnitAnimation(string animParameter, bool value, bool isTrigger = false)
    {
        if (!isTrigger)
        {
            _anim.SetBool(animParameter,value);
        }
        else
        {
            _anim.SetTrigger(animParameter);
        }
    }

    public void Tick()
    {
        _currentInitiative++;
        if (_currentInitiative >= _initiative - _bonus.Initiative)
        {
            EventBus.Instance.ActivateUnit?.Invoke(this);
            Activate();
            _currentInitiative = 0;
        }
        _view.UpdateUI();
    }

    public float DealInstantEffect(float healthEffect, float initiativeEffect)
    {
        float appliedEffect = 0;

        if (healthEffect > 0) //Heal
        {
            _view.Indicators(0, 0, healthEffect);
        }

        if (healthEffect < 0) //Damage
        { 
            healthEffect -= healthEffect * (_defence + _bonus.Defence) / 100;
            appliedEffect = healthEffect;
            _view.Indicators(healthEffect, 0, 0);
            SetUnitAnimation("Impact", true, isTrigger: true);
        }
            
        _currentHealth += healthEffect;       
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _health + _bonus.Health);
        if (_currentHealth <= 0) Death();
        
        if (initiativeEffect != 0)
        {
            _currentInitiative += initiativeEffect;
            _currentInitiative = Mathf.Clamp(_currentInitiative, 0, _initiative - _bonus.Initiative);
            _view.Indicators(0, initiativeEffect, 0);
        }

        return appliedEffect;
        _view.UpdateUI();
    }

    public void Death()
    {
        EventBus.Instance.UnitDeath?.Invoke(this,null);
        SetUnitAnimation(AnimationConstants.Death.ToString(), true);
    }

    public void AddEffect(CardEffect effect)
    {
        effect.CurrentMovesCount = effect.MovesCount;
        _effects.AddEffect(effect);
        _bonus = _effects.SetBonus();
        _view.UpdateUI();
    }

    private void Init(CombatManager combatManager)
    {
        _currentHealth = _health;
        _currentInitiative = 0;
        _bonus = new StatsBonus();
        _bonus.Initiative = 0;
        _bonus.Health = 0;
        _bonus.Damage = 0;
        _bonus.Defence = 0;
        _view.Init(this, _name);

        _stateMachine = new StateMachine();
        _stateMachine.OnStateChanged += (state, oldState) => StateChanged?.Invoke(state, oldState, this);

        _defaultState = new(_stateMachine, this, _view);
        _selectState = new(_stateMachine, this, _view);
        _highlightState = new(_stateMachine, this, _view);

        _stateMachine.Initialize(_defaultState);
        _combatManager = combatManager;
        if (_AI) _AI.Init(_combatManager);
        _effects.Init(_combatManager.GetGame.CardCollection);
    }
}

public enum AnimationConstants
{
    Slash,
    Death,
    Kick,
    Run,
    Cast,
    Impact,
    ReceiveHeal
}

[Serializable]
public struct StatsBonus
{
    public float Initiative;
    public float Health;
    public float Defence;
    public float Damage;
}