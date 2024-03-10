using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField] private float _healthBonus;
    [SerializeField] private float _initiativeBonus;
    [SerializeField] private float _damageBonus;
    [SerializeField] private float _defenceBonus;
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
    public string Name { get => _name; }
    public float Damage { get => _damage; }
    public float Defence { get => _defence; }
    public float MaxHealth { get => _health;}
    public float MaxInitiative { get => _initiative; }
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
                _combatManager.ActiveCard.ApplyEffect(targets);
            }
            else
            {
                _combatManager.ActiveCard.ApplyEffect();
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
        if (_currentInitiative >= _initiative - _initiativeBonus)
        {
            EventBus.Instance.ActivateUnit?.Invoke(this);
            Activate();
            _currentInitiative = 0;
        }
        _view.UpdateUI();
    }

    public void DealInstantEffect(float health, float initiative)
    {
        if (health > 0) //Heal
        {
            _view.Indicators(0, 0, health);
        }

        if (health < 0) //Damage
        {
            health -= health * (_defence + _defenceBonus) / 100;
            _view.Indicators(health, 0, 0);
            SetUnitAnimation("Impact", true, isTrigger: true);
        }
            
        _currentHealth += health;       
        _currentHealth = Mathf.Clamp(_currentHealth, 0, _health + _healthBonus);
        if (_currentHealth <= 0) Death();
        
        _currentInitiative += initiative;
        _currentInitiative = Mathf.Clamp(_currentInitiative, 0, _initiative + _initiativeBonus);

        if (initiative != 0) _view.Indicators(0, initiative, 0);

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
    }

    private void Init(CombatManager combatManager)
    {
        _currentHealth = _health;
        _currentInitiative = 0;
        _healthBonus = 0;
        _initiativeBonus = 0;
        _damageBonus = 0;
        _defenceBonus = 0;
        _view.Init(this, _name);

        _stateMachine = new StateMachine();
        _stateMachine.OnStateChanged += (state, oldState) => StateChanged?.Invoke(state, oldState, this);

        _defaultState = new(_stateMachine, this, _view);
        _selectState = new(_stateMachine, this, _view);
        _highlightState = new(_stateMachine, this, _view);

        _stateMachine.Initialize(_defaultState);
        _combatManager = FindObjectOfType<CombatManager>();
        if (_AI) _AI.Init(_combatManager);
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

public struct StatsBonus
{
    public float Initiative;
    public float Health;
    public float Defence;
    public float Damage;
}