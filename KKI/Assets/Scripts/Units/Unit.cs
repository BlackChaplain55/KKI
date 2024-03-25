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
    [SerializeField] private bool _archer;
    [SerializeField] private bool _closeCombat;

    [Space]
    [Header("Unit Data")]
    [SerializeField] private UnitEffects _effects;
    [SerializeField] private GameObject _gameObject;
    [SerializeField] private Transform _transform;
    [SerializeField] private Transform _figure;
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
    public UnitEffects Effects { get => _effects; }
    public string Name { get => _name; }
    public float Damage { get => _damage; }
    public float MDamage { get => _magicPower; }
    public float Defence { get => _defence; }
    public float MResistance { get => _magicResist; }
    public float MaxHealth { get => _health+_bonus.Health;}
    public float MaxInitiative { get => _initiative-_bonus.Initiative; }
    public float CurrentHealth { get => _currentHealth; }
    public float CurrentInitiative { get => _currentInitiative; }
    public StatsBonus Bonus { get => _bonus; }
    // Start is called before the first frame update

    private void Awake()
    {
        Init(FindObjectOfType<CombatManager>());
    }

    private void OnValidate()
    {
        if (!_anim) _anim = transform.Find("Figure").GetComponent<Animator>();
        if(!_transform) _transform = transform;
        if (!_view) _view = GetComponent<UnitView>();
        if (!_AI) _AI = GetComponent<SimpleAI>();
        if (_effects==null) _effects = GetComponent<UnitEffects>();
        if (!_figure) _figure = transform.Find("Figure");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_combatManager.ActiveCard != null)
        {
            _combatManager.ActiveUnit.SetUnitAnimation(_combatManager.ActiveCard.AnimationName,true,true);
            _combatManager.ActiveUnit.LookAtTarget(transform);
            _combatManager.CurrentTarget = this;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _view.ShowStats(true);
        if (_stateMachine.CurrentState == DefaultState)
        {
            _stateMachine.ChangeState(HighlightState);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _view.ShowStats(false);
        if (_stateMachine.CurrentState == HighlightState)
        {
            _stateMachine.ChangeState(DefaultState);
        }
    }

    public void Activate()
    {
        //_effects.CheckEffects();
        //_bonus = _effects.SetBonus();
        //_view.UpdateUI();
        if (_effects.CheckEffectExist(EffectTypes.provoke)) {
            _view.ShowEffectName("Оглушен", false);
            return;
        };
        if (_AI)
        {
            Debug.Log("Enemy unit " + _name + " is activated!");
            _AItarget = _AI.PickTarget();
            LookAtTarget(_AItarget.transform);
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
            _AItarget.DealInstantEffect(-_damage,_magicPower,0, 0);
            Debug.Log("Enemy unit " + _name + " hit " + _AItarget + " with " + _damage);
            _AItarget = null;
            EventBus.Instance.UnitActivationFinished?.Invoke();
        }
        else
        {
            if (_combatManager.ActiveCard.Type == CardTypes.attackSingle ||
                _combatManager.ActiveCard.Type == CardTypes.bonusSingle ||
                _combatManager.ActiveCard.Type == CardTypes.malusSingle)
            {
                List<Unit> targets = new List<Unit>();
                targets.Add(_combatManager.CurrentTarget);
                _combatManager.ActiveCard.ApplyCardEffects(this, targets);
            }
            else if(_combatManager.ActiveCard.Type == CardTypes.bonusMulti)
            {
                _combatManager.ActiveCard.ApplyCardEffects(this);
            }
            else
            {
                List<Unit> targets = new List<Unit>();
                targets.AddRange(_combatManager.PlayerUnits);
                _combatManager.ActiveCard.ApplyCardEffects(this, targets);
            }
            _combatManager.CurrentTarget = null;
            EventBus.Instance.UnitActivationFinished?.Invoke();
        }
        _effects.CheckEffects();
        _bonus = _effects.SetBonus();
        _view.UpdateUI();
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
        _currentInitiative += _initiative + _bonus.Initiative;
        if (_currentInitiative >= _combatManager.UnitActivationLimit)
        {
            EventBus.Instance.ActivateUnit?.Invoke(this);
            Activate();
            _currentInitiative = 0;
        }
        _view.UpdateUI();
    }

    public float DealInstantEffect(float pDamage, float mDamage, float heal, float initiativeBoost)
    {
        float appliedDamage = 0;

        if (heal > 0) //Heal
        {
            _view.Indicators(0, 0, heal);
        }

        if (pDamage > 0|| mDamage > 0) //Damage
        {
            pDamage -= pDamage * (_defence + _defence*_bonus.Defence) / 100;
            mDamage -= mDamage * (_magicResist + _magicResist*_bonus.MResistance) / 100;
            appliedDamage = -pDamage- mDamage;
            _view.Indicators(appliedDamage, 0, 0);
            SetUnitAnimation("Impact", true, isTrigger: true);
        }
            
        _currentHealth += appliedDamage + heal; ;       
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _health + _bonus.Health);
        if (_currentHealth <= 0) Death();
        
        if (initiativeBoost != 0)
        {
            _currentInitiative += initiativeBoost;
            _view.Indicators(0, initiativeBoost, 0);
        }

        return appliedDamage;
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
        if (_AI) _AI.Init(_combatManager, this);
        _effects.Init(_combatManager.GetGame.CardCollection);
        float rnd = UnityEngine.Random.Range(0.8f, 1.2f);
        _anim.SetFloat("IdleSpeed", rnd);
    }

    public void LookAtTarget(Transform target)
    {
        _figure.LookAt(target);
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
    public float MResistance;
    public float MDamage;
}