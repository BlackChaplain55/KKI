using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//Этот класс описывает Юнита
[RequireComponent(typeof(UnitView))]
public class Unit : MonoBehaviour
{
    [Header("Unit stats")]
    [SerializeField] private string _name;
    [SerializeField] private float _health;
    [SerializeField] private float _initiative;
    [SerializeField] private float _damage;
    [SerializeField] private float _defence;

    [Space]
    [Header("Unit Data")]

    [SerializeField] private GameObject _gameObject;
    [SerializeField] private Transform _transform;
    [SerializeField] private Animator _anim;
    [SerializeField] private UnitView _view;

    [Space]
    [Header("Runtime stats")]
    [SerializeField] private float _currentHealth;
    [SerializeField] private float _currentInitiative;
    [SerializeField] private float _damageBonus;
    [SerializeField] private float _defenceBonus;

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

    public string Name { get => _name; }
    public float MaxHealth { get => _health;}
    public float MaxInitiative { get => _initiative; }
    public float CurrentHealth { get => _currentHealth; }
    public float CurrentInitiative { get => _currentInitiative; }
    // Start is called before the first frame update

    private void Awake()
    {
        Init();
    }

    private void OnValidate()
    {
        if (!_anim) _anim = GetComponent<Animator>();
        if(!_transform) _transform = transform;
        if (!_view) _view = GetComponent<UnitView>();
    }

    public void Activate()
    {
        Debug.Log("Enemy unit " + _name + " is activated!");
        SetUnitAnimation("Slash",true,isTrigger:true);
    }

    public void OnAttackOver()
    {
        EventBus.Instance.UnitActivationFinished?.Invoke();
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

    private void Init()
    {
        _currentHealth = _health;
        _currentInitiative = 0;
        _view.Init(this, _name);

        _stateMachine = new StateMachine();
        _stateMachine.OnStateChanged += (state, oldState) => StateChanged?.Invoke(state, oldState, this);

        _defaultState = new(_stateMachine, this, _view);
        _selectState = new(_stateMachine, this, _view);
        _highlightState = new(_stateMachine, this, _view);

        _stateMachine.Initialize(_defaultState);
    }

    public void Tick()
    {
        _currentInitiative++;
        if (_currentInitiative >= _initiative)
        {
            EventBus.Instance.ActivateUnit?.Invoke(this);
            _currentInitiative = 0;
        }
        _view.UpdateUI();
    }
}
