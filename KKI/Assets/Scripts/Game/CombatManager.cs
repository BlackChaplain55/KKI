using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Это класс отвечает за интерфес в основном игровом режиме

public class CombatManager : MonoBehaviour
{
    [SerializeField] private Game _game;
    [SerializeField] private Transform _unitsContainer;
    [SerializeField] private Transform _enemiesContainer;
    [SerializeField] private float _tickInterval;
    [SerializeField] private List<PlayerUnit> _playerUnits;
    [SerializeField] private List<Unit> _enemyUnits;
    [SerializeField] private Unit _currentTarget;

    private Unit _activeUnit;
    private Card _activeCard;
    private float _tickTimer;

    public Unit CurrentTarget { get => _currentTarget; set => _currentTarget = value; }
    
    public Unit ActiveUnit  => _activeUnit;
    public Card ActiveCard => _activeCard;
    public List<PlayerUnit> PlayerUnits => _playerUnits;
    public List<Unit> EnemyUnits => _enemyUnits;

    private void OnValidate()
    {
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_unitsContainer) _unitsContainer = transform.Find("PlayerUnits");
        if (!_enemiesContainer) _enemiesContainer = transform.Find("EnemyUnits");
    }

    private void Start()
    {
        Initialize();
    }

    private void FixedUpdate()
    {
        if (_activeUnit == null)
        {
            _tickTimer += Time.fixedDeltaTime;
        }

        if (_tickTimer >= _tickInterval)
        {
            if (EventBus.Instance != null) EventBus.Instance.Tick?.Invoke();
            _tickTimer = 0;
            Tick();
        }
    }

    private void ActivateUnit(Unit unit)
    {
        _activeUnit = unit;
    }

    private void Tick()
    {
        foreach (Unit unit in _playerUnits)
        {
            if (_activeUnit == null) unit.Tick();
        }
        foreach (Unit unit in _enemyUnits)
        {
            if (_activeUnit == null) unit.Tick();
        }
    }

    public void Initialize()
    {
        _tickTimer = 0;
        EventBus.Instance.ActivateUnit.AddListener(ActivateUnit);
        EventBus.Instance.ActivateCard.AddListener(ActivateCard);
        EventBus.Instance.DeselectUnits.AddListener(DeselectUnits);
        EventBus.Instance.UnitActivationFinished.AddListener(UnitActivationFinished);
        EventBus.Instance.UnitDeath.AddListener(UnitDeath);
        for (int i = 0; i < _unitsContainer.childCount; i++)
        {
            _playerUnits.Add(_unitsContainer.GetChild(i).GetComponent<PlayerUnit>());
        }
        for (int i = 0; i < _enemiesContainer.childCount; i++)
        {
            _enemyUnits.Add(_enemiesContainer.GetChild(i).GetComponent<Unit>());
        }
    }

    public void Exit()
    {
       
    }

    private void UnitActivationFinished()
    {
        _activeUnit = null;
    }

    private void UnitDeath(Unit unit, PlayerUnit playerUnit)
    {
        _playerUnits.Remove(playerUnit);
        _enemyUnits.Remove(unit);
    }

    private void DeselectUnits()
    {
        foreach(PlayerUnit unit in _playerUnits)
        {
            unit.DeselectUnit();
        }
    }

    private void ActivateCard(Card card)
    {
        _activeCard = card;
        if(card.Type==CardTypes.attackMulti|| card.Type == CardTypes.bonusMulti|| card.Type == CardTypes.malusMulti)
        {
            ActiveUnit.SetUnitAnimation(card.AnimationName, true, true);
        }
    }
}
