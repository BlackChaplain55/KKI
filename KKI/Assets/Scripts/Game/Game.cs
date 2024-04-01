using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using AutoLayout3D;

//Это основная точка входа игры
public class Game : MonoBehaviour
{

    [Space]
    [Header("Game components")]
    [SerializeField] private Color _availableMovesColor;
    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private CardCollection _cardCollection;
    [SerializeField] private Deck _deck;
    [SerializeField] private DeckBuilder _deckBuilder;
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private Transform _playerDeckContainer;
    [SerializeField] private Transform _playerDeckDefaultPosition;
    [SerializeField] private GridLayoutGroup3D _layoutGroup3D;
    [SerializeField] private Scene _currentScene;
    [SerializeField] private CardCollection _collection;
    [SerializeField] private Settings _settings;
    [SerializeField] private PuzzleController _puzzleController;
    [SerializeField] private GlobalMapManager _globalMapManager;

    private Transform _playerGMPosition;

    //private float _cellStep;
    private bool _isCombat;
    [SerializeField] private EncounterData _currentEncounter;
    [SerializeField] private ProgressData  _progress;
    [SerializeField] private CardCollection _defaultCollection;

    private GameStartState _startState;
    private GameDeckBuildState _deckbuildState;
    private GameMainMenuState _mainMenuState;
    private GamePlayState _playState;
    private GameGlobalMapState _globalMapState;

    private static Game GameInstance;
    private bool _inputBlocked;

    public EncounterData Encounter { get => _currentEncounter; set => _currentEncounter = value; }
    public ProgressData Progress { get => _progress; set => _progress = value; }
    public CardCollection DefaultCollection { get => _defaultCollection;}
    public Transform PlayerGMPosition { get => _playerGMPosition; set => _playerGMPosition.SetPositionAndRotation(value.position,value.rotation); }
    public StateMachine StateMachine => _stateMachine;
    public IState CurrentState => _stateMachine.CurrentState;
    public MainMenu MainMenu => _mainMenu;
    public Settings GameSettings => _settings;
    public AudioMixer GameAudioMixer => _audioMixer;

    public Scene CurrentScene => _currentScene;
    public GameStartState StartState => _startState;
    public GameDeckBuildState DeckBuildState => _deckbuildState;
    public GameMainMenuState MainMenuState => _mainMenuState;
    public GamePlayState PlayState => _playState;
    public GameGlobalMapState GlobalMapState => _globalMapState;
    public CardCollection CardCollection => _cardCollection;
    public Deck CurrentDeck => _deck;
    public DeckBuilder DeckBuider => _deckBuilder;
    public bool IsCombat { get => _isCombat; set => _isCombat = value; }
    public CombatManager Combat => _combatManager;

    public float ActionPoints { get => _combatManager.ActionPoints; }
    public bool InputBlocked { get => _inputBlocked; set => _inputBlocked = value; }

    private void OnValidate()
    {
        if (!_mainMenu) _mainMenu = transform.Find("UI").GetComponent<MainMenu>();
        if (!_deck) _deck = GetComponent<Deck>();
        if (!_playerDeckContainer) _playerDeckContainer = transform.Find("PlayerDeck");
        if (!_layoutGroup3D) _layoutGroup3D = _playerDeckContainer.transform.GetComponent<GridLayoutGroup3D>();
    }

    void Start()
    {
        //DontDestroyOnLoad(gameObject);

        //if (GameInstance == null)
        //{
        //    GameInstance = this;
        //    Initialize();
        //}
        //else Destroy(gameObject);    
    }

    void Awake()
    {
        
        Application.targetFrameRate = 60;
        DontDestroyOnLoad(gameObject);
        
        if (GameInstance == null)
        {
            GameInstance = this;
            Initialize();
        }
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Initialize()
    {
        _currentScene = SceneManager.GetActiveScene();
        _stateMachine = new StateMachine();
        _startState = new(_stateMachine, this);
        _playState = new(_stateMachine, this);
        _deckbuildState = new(_stateMachine, this);
        _mainMenuState = new(_stateMachine, this);
        _globalMapState = new(_stateMachine, this);
        _mainMenu.Initialize();

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.DeckBuildSceneName))
        {
            _stateMachine.Initialize(_deckbuildState);
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.CombatSceneName))
        {
            _stateMachine.Initialize(_playState);
            //InitializeCombatScene();
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.GlobalMapSceneName))
        {
            _stateMachine.Initialize(_globalMapState);
            //InitializeGlobalMapScene();
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
        // Действия при начале боя
        _isCombat = true;
        _combatManager = FindObjectOfType<CombatManager>();
        List<GameObject> enemiesList = new();
        if (_currentEncounter.Name != "")
        {
            enemiesList = _currentEncounter.Enemies;
        }
        ClearPlayerDeckContainer();
        InitializePlayerDeck();
        _combatManager.Initialize(this, enemiesList,_progress);  
    }

    public void InitializeGlobalMapScene()
    {
        _puzzleController = FindObjectOfType<PuzzleController>();
        _globalMapManager = FindObjectOfType<GlobalMapManager>();
        _puzzleController.Init(this,_globalMapManager);
        _globalMapManager.Init(this);
    }

    public void ExitCombatScene()
    {
        ClearPlayerDeckContainer();
        _deck.Clear();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {     
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.DeckBuildSceneName))
        {
            _deckBuilder = FindAnyObjectByType<DeckBuilder>();
            _deckBuilder.Init(this);
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.CombatSceneName))
        {
            InitializeCombatScene();
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.GlobalMapSceneName))
        {
            InitializeGlobalMapScene();
        }
    }

    private void InitializePlayerDeck() // Загружаем карты в руку игрока
    {
        _deck.Init(this);
        _deck.LoadDeck();
        _deck.AddToHand(_deck.GetRandomCards(_combatManager.InitialHandSize+_progress.InitialDeckBonus));
        InstantinateCardsToDeck(_deck.PlayerHand);
        
    }

    public void InstantinateCardsToDeck(List<string> cards)
    {
        foreach (string cardName in cards)
        {
            Card card = _cardCollection.FindCard(cardName);
            if (card != null)
            {
                GameObject newCard = Instantiate(card.GameObject, _playerDeckContainer);
                newCard.GetComponent<Card>().IsInDeck = true;
                newCard.GetComponent<Card>().Initialize(this);
            }
        }
        SetLayoutSpacing();
    }

    private void ClearPlayerDeckContainer()
    {
        for (int i = _playerDeckContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(_playerDeckContainer.GetChild(i).gameObject);
        }
        Debug.Log("Deck container clear - " + _playerDeckContainer.childCount.ToString());
    }

    public void SetLayoutSpacing() //Настраиваем плотность расположения карт в руке игрока
    {
        if (_playerDeckContainer.childCount <= 8)
            _layoutGroup3D.spacing.z = 1f;
        else if (_playerDeckContainer.childCount <= 12)
            _layoutGroup3D.spacing.z = 0.6f;
        else
            _layoutGroup3D.spacing.z = 0.3f;
        _playerDeckContainer.transform.position = new Vector3(_playerDeckContainer.transform.position.x, _playerDeckContainer.transform.position.y,
            _playerDeckDefaultPosition.transform.position.z + (_layoutGroup3D.cellSize.z / 2 + _layoutGroup3D.spacing.z/2) * (_playerDeckContainer.childCount - 1)-_layoutGroup3D.cellSize.z / 2);
        _layoutGroup3D.UpdateLayout();
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
        return Mathf.Log10(Mathf.Clamp(value,0.0001f,1)) * 20;
    }

    public void ChangeState(IState state)
    {
        _stateMachine.ChangeState(state);
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    public void DropCollection()
    {
        _cardCollection.Cards.Clear();
        _cardCollection.Cards.AddRange(_defaultCollection.Cards);
    }
}

public struct Settings
{
    public bool SoundEnabled;
    public float EffectsVol;
    public float MusicVol;
    public float AmbientVol;
}

[Serializable]

public struct ProgressData
{
    public Vector3 PlayerPosition;
    public string CompleteEncounters;
    public int InitialDeckBonus;
    public int TurnCardBonus;
    public int InitialAP;
    public int TurnAPBonus;
    public bool Bastet;
    public bool Geb;
    public bool Meritseger;
    public bool Thoth;
    public int DarkCardsUsed;
}
