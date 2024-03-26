using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Это класс отвечает за интерфес в основном игровом режиме
[RequireComponent(typeof(CombatUI))]

public class CombatManager : MonoBehaviour
{
    [Header("Game settings")]
    [SerializeField] private int _initialHandSize = 6;
    [SerializeField] private int _cardsPerTurn = 3;
    [SerializeField] private int _initialAP = 2;
    [SerializeField] private int _initialAPPerTurn = 3;
    [SerializeField] private float _turnLength;
    [SerializeField] private float _unitActvationLimit = 100;
    [SerializeField] private int _actionPoints;
    [SerializeField] private float _tickInterval;
    [SerializeField] private Color _bonusColor = Color.green;
    [SerializeField] private Color _malusColor = Color.red;
    [SerializeField] private List<Transform> _enemyPositions;
    [SerializeField] private List<GameObject> _defaultEnemies;
    [Space]
    [Header("Game components")]
    [SerializeField] private Game _game;
    [SerializeField] private CombatUI _combatUI;
    [SerializeField] private Transform _unitsContainer;
    [SerializeField] private Transform _enemiesContainer;
    [SerializeField] private List<PlayerUnit> _playerUnits;
    [SerializeField] private List<Unit> _enemyUnits;
    [SerializeField] private Unit _currentTarget;

    private Unit _activeUnit;
    private Card _activeCard;
    private float _tickTimer;
    private float _currentTurnLength;
    private int _bonusAPPerTurn;

    public Unit CurrentTarget { get => _currentTarget; set => _currentTarget = value; }
    public int ActionPoints { get => _actionPoints; set => _actionPoints = value; }
    public int InitialHandSize { get => _initialHandSize; }

    public Game GetGame => _game;
    public Unit ActiveUnit => _activeUnit;
    public Card ActiveCard => _activeCard;
    public List<PlayerUnit> PlayerUnits => _playerUnits;
    public List<Unit> EnemyUnits => _enemyUnits;
    public float CurrentTurnLength { get => _currentTurnLength; }
    public float TurnLength { get => _turnLength; }
    public float UnitActivationLimit { get => _unitActvationLimit; }

    public Color BonusColor { get => _bonusColor; }
    public Color MalusColor { get => _malusColor; }

    private void OnValidate()
    {
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_unitsContainer) _unitsContainer = transform.Find("PlayerUnits");
        if (!_enemiesContainer) _enemiesContainer = transform.Find("EnemyUnits");
        if (!_combatUI) _combatUI = GetComponent<CombatUI>();
    }

    private void Start()
    {
        //Initialize();
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
        EventBus.Instance.UpdateCards?.Invoke();
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
        _currentTurnLength++;
        if (_currentTurnLength >= _turnLength)
        {
            _currentTurnLength = 0;
            Turn();
        }
        _combatUI.UpdateUI();
    }

    public void Initialize(List<GameObject> enemiesList)
    {
        _tickTimer = 0;
        _actionPoints = _initialAP;
        EventBus.Instance.ActivateUnit.AddListener(ActivateUnit);
        EventBus.Instance.ActivateCard.AddListener(ActivateCard);
        EventBus.Instance.DeselectUnits.AddListener(DeselectUnits);
        EventBus.Instance.UnitActivationFinished.AddListener(UnitActivationFinished);
        EventBus.Instance.UnitDeath.AddListener(UnitDeath);

        string[] heroes = PlayerPrefs.GetString("Heroes").Split(",");
        if (heroes.Length != 0)
        {
            for (int i = 0; i < _unitsContainer.childCount; i++)
            {
                _playerUnits.Add(_unitsContainer.GetChild(i).GetComponent<PlayerUnit>());
            }
        }
        //for (int i = 0; i < _enemiesContainer.childCount; i++)
        //{
        //    _enemyUnits.Add(_enemiesContainer.GetChild(i).GetComponent<Unit>());
        //}
        if (enemiesList.Count == 0) enemiesList = _defaultEnemies;

        for (int i = 0; i < enemiesList.Count; i++)
        {
            if (i == 5) break;
            var enemy = enemiesList[i];
            GameObject newEnemy = Instantiate(enemy, _enemyPositions[i].position, _enemyPositions[i].rotation, _enemiesContainer);
            _enemyUnits.Add(newEnemy.GetComponent<Unit>());
        }
        _combatUI.Init(this);
    }

    public void Exit()
    {

    }

    private void Turn()
    {
        _actionPoints += _initialAPPerTurn+ _bonusAPPerTurn;
        List<string> newCards = _game.CurrentDeck.GetRandomCards(_cardsPerTurn);
        _game.CurrentDeck.AddToHand(newCards);
        _game.InstantinateCardsToDeck(newCards);
    }

    private void UnitActivationFinished()
    {
        _activeUnit = null;
    }

    private void UnitDeath(Unit unit, PlayerUnit playerUnit)
    {
        _playerUnits.Remove(playerUnit);
        _enemyUnits.Remove(unit);

        if (_enemyUnits.Count==0)
        {

        }
    }

    private void CombatFinished()
    {

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

    public void UpdateUI()
    {
        _combatUI.UpdateUI();
    }
}
