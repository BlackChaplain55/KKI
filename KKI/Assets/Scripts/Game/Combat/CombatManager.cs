using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Это класс отвечает за интерфес в основном игровом режиме
[RequireComponent(typeof(CombatUI))]

public class CombatManager : MonoBehaviour
{
    [Space]
    [Header("Heroes")]
    [SerializeField] private PlayerUnit _Jabari;
    [SerializeField] private PlayerUnit _Geb;
    [SerializeField] private PlayerUnit _Bastet;
    [SerializeField] private PlayerUnit _Meritseger;
    [SerializeField] private PlayerUnit _Thoth;
    [Header("Game settings")]
    [SerializeField] private int _maxHandSize = 12;
    [SerializeField] private int _initialHandSize = 6;
    [SerializeField] private int _cardsPerTurn = 3;
    [SerializeField] private int _initialAP = 2;
    [SerializeField] private int _initialAPPerTurn = 3;
    [SerializeField] private float _turnLength;
    [SerializeField] private float _unitActvationLimit = 100;
    [SerializeField] private int _actionPoints;
    [SerializeField] private float _tickInterval;
    [SerializeField] private float _effectDelayInterval;
    [SerializeField] private Color _bonusColor = Color.green;
    [SerializeField] private Color _malusColor = Color.red;
    [SerializeField] public Color InfoBonusColor = Color.green;
    [SerializeField] public Color InfoMalusColor = Color.red;
    [SerializeField] private List<Transform> _enemyPositions;
    [SerializeField] private List<GameObject> _defaultEnemies;
    [SerializeField] private Transform _fullViewPosition;
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
    private bool _finished = false;
    private bool _victory = false;

    public Unit CurrentTarget { get => _currentTarget; set => _currentTarget = value; }
    public int ActionPoints { get => _actionPoints; set => _actionPoints = value; }
    public int InitialHandSize { get => _initialHandSize; }
    public float EffectDelayInterval { get => _effectDelayInterval; }
    public Transform FullViewPosition { get => _fullViewPosition; }

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
       //if (!_game) _game = FindObjectOfType<Game>();
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
        if (_finished) return;
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

    public void SkipTurn()
    {
        _activeUnit.FinishActivation();
    }

    public void Initialize(Game game,List<GameObject> enemiesList, ProgressData progress)
    {
        _game = game;
        _tickTimer = 0;
        _actionPoints = _initialAP;
        EventBus.Instance.ActivateUnit.AddListener(ActivateUnit);
        EventBus.Instance.ActivateCard.AddListener(ActivateCard);
        EventBus.Instance.DeselectUnits.AddListener(DeselectUnits);
        EventBus.Instance.UnitActivationFinished.AddListener(UnitActivationFinished);
        EventBus.Instance.UnitDeath.AddListener(UnitDeath);

        if (progress.PlayerPosition == Vector3.zero)
        {
            for (int i = 0; i < _unitsContainer.childCount; i++)
            {
                _playerUnits.Add(_unitsContainer.GetChild(i).GetComponent<PlayerUnit>());
            }
        }
        else
        {
            _playerUnits.Add(_Jabari);
            EnableHero(progress.Bastet, _Bastet);
            EnableHero(progress.Geb, _Geb);
            EnableHero(progress.Thoth, _Thoth);
            EnableHero(progress.Meritseger, _Meritseger);
        }

        if (enemiesList.Count == 0) enemiesList = _defaultEnemies;

        for (int i = 0; i < enemiesList.Count; i++)
        {
            if (i == 5) break;
            var enemy = enemiesList[i];
            GameObject newEnemy = Instantiate(enemy, _enemyPositions[i].position, _enemyPositions[i].rotation, _enemiesContainer);
            _enemyUnits.Add(newEnemy.GetComponent<Unit>());
        }
        _finished = false;
        _combatUI.Init(this);
        //_game.CurrentDeck.LoadDeck();
    }

    private void EnableHero(bool enabled, PlayerUnit hero)
    {
        if (enabled) _playerUnits.Add(hero); else hero.gameObject.SetActive(false);
    }
    public void Exit()
    {

    }

    private void Turn()
    {
        _actionPoints += _initialAPPerTurn + _game.Progress.TurnAPBonus;
        if (_game.CurrentDeck.PlayerHand.Count > _maxHandSize) return;
        List<string> newCards = _game.CurrentDeck.GetRandomCards(_cardsPerTurn +_game.Progress.TurnCardBonus);
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

        if (_enemyUnits.Count==0) //Победа в битве
        {
            _victory = true;
            CombatFinishedConfirmation();
        }

        if (_playerUnits.Count == 0) //Поражение в битве
        {
            _victory = false;
            CombatFinishedConfirmation();
        }
    }

    private void CombatFinishedConfirmation()
    {
        _finished = true;
        if (_victory)
        {
            _game.MainMenu.ShowConfirmationWindow(_game.Encounter.EncounterVictoryText, true);
        }
        else
        {
            _game.MainMenu.ShowConfirmationWindow("Поражение", true);
        }
        EventBus.Instance.Confirm.AddListener(OnConfirmation);
    }

    private void OnConfirmation()
    {
        if (_victory)
        {
            EncounterData enc = _game.Encounter;
            if (enc.VictoryCard)
            {
                if (_game.CardCollection.FindCard(enc.VictoryCard.name) == null)
                {
                    _game.CardCollection.Cards.Add(enc.VictoryCard);
                }
            }
            enc.IsComplete = true;
            _game.Encounter = enc;
            _game.ChangeState(_game.GlobalMapState);
        }
        else
        {
            EncounterData enc = _game.Encounter;
            enc.IsComplete = false;
            _game.Encounter = enc;
            _game.ChangeState(_game.GlobalMapState);
        }
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
