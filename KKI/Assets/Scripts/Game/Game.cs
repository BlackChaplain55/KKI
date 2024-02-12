using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//Это основная точка входа игры
public class Game : MonoBehaviour
{
    [SerializeField] private Color _availableMovesColor;
    [SerializeField] private Grid _grid;
    [SerializeField] private Unit _selectedUnit;
    [SerializeField] private UnitActions _unitActions;
    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private CombatUIManager _combatUI;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private CardCollection _cardCollection;
    [SerializeField] private Deck _deck;
    [SerializeField] private Transform _playerDeckContainer;
    [SerializeField] private AutoLayout3D.GridLayoutGroup3D _layoutGroup3D;

    private List<Cell> _cells = new List<Cell>();
    private List<Cell> _movingCells = new List<Cell>();
    private Cell _selectedCell;
    private bool _haveSelectedCell;
    private bool _haveSelectedUnit;
    private Transform _unitsContainer;
    private float _cellStep;
    private Unit _unitToSpawn;
    private Settings _settings;
    
    private GameStartState _startState;
    private GameDeckBuildState _deckbuildState;
    private GameMainMenuState _mainMenuState;
    private GamePlayState _playState;
    

    private static GameObject GameInstance;
    public StateMachine StateMachine => _stateMachine;
    public IState CurrentState => _stateMachine.CurrentState;
    public Cell SelectedCell => _selectedCell;
    public bool HaveSelectedCell => _haveSelectedCell;
    public bool HaveSelectedUnit => _haveSelectedUnit;
    public Transform UnitsContainer => _unitsContainer;
    public float CellStep => _cellStep;
    public MainMenu MainMenu => _mainMenu;
    public Settings GameSettings => _settings;
    public AudioMixer GameAudioMixer => _audioMixer;

    public GameStartState StartState => _startState;
    public GameDeckBuildState DeckBuildState => _deckbuildState;
    public GameMainMenuState MainMenuState => _mainMenuState;
    public GamePlayState PlayState => _playState;
    public CardCollection CardCollection => _cardCollection;
    public Deck CurrentDeck => _deck;

    private void OnValidate()
    {
        if (!_unitsContainer) transform.Find("Units");
        if (!_grid) _grid = FindObjectOfType<Grid>();
        if (!_unitActions) _unitActions = GetComponent<UnitActions>();
        if (!_mainMenu) _mainMenu = FindObjectOfType<MainMenu>();
        if (!_combatUI) _combatUI = FindObjectOfType<CombatUIManager>();
        if (!_deck) _deck = GetComponent<Deck>();
        if (!_playerDeckContainer) _playerDeckContainer = transform.Find("PlayerDeck");
        if (!_layoutGroup3D) _layoutGroup3D = _playerDeckContainer.transform.GetComponent<AutoLayout3D.GridLayoutGroup3D>();
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (GameInstance == null)
        {
            GameInstance = gameObject;
            Initialize();
        }
        else Destroy(gameObject);    
    }

    void Awake()
    {
        Application.targetFrameRate = 60;
        EventBus.Instance.OnSelectCell?.AddListener(OnSelectCell);  
    }

    private void Initialize()
    {
        _stateMachine = new StateMachine();

        _startState = new(_stateMachine, this);
        _playState = new(_stateMachine, this);
        _deckbuildState = new(_stateMachine, this);
        _mainMenuState = new(_stateMachine, this);

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.DeckBuildSceneName))
        {
            _stateMachine.Initialize(_deckbuildState);
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.CombatSceneName))
        {
            _stateMachine.Initialize(_playState);
        }
        else
        {
            _stateMachine.Initialize(_startState);
        }

        _settings = SaveLoadManager.SettingsLoad();
        _mainMenu.SetSettingsUI(_settings);
        SetVolumes();
    }

    public void SetSettings(Settings newSettings)
    {
        _settings = newSettings;
        SetVolumes();
    }

    public void InitializeCombatScene()
    {
        _cells.AddRange(FindObjectsOfType<Cell>());
        _cellStep = _grid.CellSize.x + _grid.OffsetSize;
        _deck.LoadDeck();
        InitializePlayerDeck();
        _combatUI.Initialize();
    }

    public void ExitCombatScene()
    {
        _combatUI.Exit();
    }

    private void InitializePlayerDeck() // Загружаем карты в руку игрока
    {
        foreach (string cardName in _deck.PlayerDeck)
        {
            Card card = _cardCollection.FindCard(cardName);
            if (card != null)
            {
                GameObject newCard = Instantiate(card.GameObject, _playerDeckContainer);
                //newCard.transform.localRotation = Quaternion.Euler(0, -100, 70);
                newCard.GetComponent<Card>().IsInDeck = true;
            }
        }
        SetLayoutSpacing();
    }

    private void SetLayoutSpacing() //Настраиваем плотность расположения карт в руке игрока
    {
        if (_playerDeckContainer.childCount <= 12)
            _layoutGroup3D.spacing.x = 5;
        else if (_playerDeckContainer.childCount <= 14)
            _layoutGroup3D.spacing.x = 4;
        else
            _layoutGroup3D.spacing.x = 3;
    }

    private void TrySetSelected(Cell cell) //Обрабатываем нажатие на ячейку
    {
        if (cell != _selectedCell)
        {
            if (_haveSelectedUnit)
            {
                if(cell.CurrentState is CellHighlightState)
                {
                    _unitActions.MoveUnit(_selectedCell,cell);
                    return;
                }
            }

            if (_haveSelectedCell)
            {
                _selectedCell.Deselect();
                if (_selectedCell.HaveUnit)
                {
                    _selectedCell.ShowMoves(false); ;
                }
            }

            _selectedCell = cell;
            _haveSelectedCell = true;
            if (_selectedCell.HaveUnit)
            {
                _haveSelectedUnit = true;
                _selectedUnit = _selectedCell.Unit;
                _selectedCell.ShowMoves(true);
            }
        }
        else if(_selectedCell)
        {
            _selectedCell.Deselect();
            _selectedCell = null;
            _haveSelectedCell = false;
            _selectedUnit = null;
            _haveSelectedUnit = false;
        }
    }

    private void OnSelectCell(Cell cell)
    {
        TrySetSelected(cell);
        if (!cell.HaveUnit && _unitToSpawn)
        {
            SpawnUnit();
        }
    }

    public void BeginSpawnUnit(Unit unit)
    {
        _unitToSpawn = unit;
    }

    public void SpawnUnit() //Выставляем юнит на поле
    {
        Unit newUnit = Instantiate(_unitToSpawn, _unitsContainer);
        _selectedCell.SetUnit(newUnit);
        newUnit.SetCell(_selectedCell);
        _haveSelectedUnit = true;
        _haveSelectedUnit = true;
        _selectedUnit = newUnit;
        _unitToSpawn = null;
        _selectedCell.ShowMoves(true);
        //Debug.Log("Unit Spawn at "+_selectedCell.name);
    }

    private void SetVolumes() //установка громкости из насттроек в микшер
    {
        if (_settings.SoundEnabled)
        {
            _audioMixer.SetFloat("Master", 0);
            _audioMixer.SetFloat("MusicVol", ValueToVolume(_settings.MusicVol));
            _audioMixer.SetFloat("AmbientVol", ValueToVolume(_settings.AmbientVol));
            _audioMixer.SetFloat("EffectsVol", ValueToVolume(_settings.EffectsVol));
        }
        else
        {
            _audioMixer.SetFloat("Master", -80);
        }
    }

    private float ValueToVolume(float value) //делаем регулировку гроскости более линейной
    {
        return Mathf.Log10(Mathf.Clamp(value,0.001f,1)) * 40;
    }

    public void ChangeState(IState state)
    {
        _stateMachine.ChangeState(state);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}

public struct Settings
{
    public bool SoundEnabled;
    public float EffectsVol;
    public float MusicVol;
    public float AmbientVol;
}
