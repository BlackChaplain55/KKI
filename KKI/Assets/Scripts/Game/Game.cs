using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

//Это основная точка входа игры
public class Game : MonoBehaviour
{
    [SerializeField] private Color _availableMovesColor;

    [SerializeField] private StateMachine _stateMachine;
    [SerializeField] private MainMenu _mainMenu;
    [SerializeField] private AudioMixer _audioMixer;
    [SerializeField] private CardCollection _cardCollection;
    [SerializeField] private Deck _deck;
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private Transform _playerDeckContainer;
    [SerializeField] private AutoLayout3D.GridLayoutGroup3D _layoutGroup3D;
    [SerializeField] private Scene _currentScene;
    private float _cellStep;
    private bool _isCombat;
    private Settings _settings;
    private PuzzleController _puzzleController;
    
    private GameStartState _startState;
    private GameDeckBuildState _deckbuildState;
    private GameMainMenuState _mainMenuState;
    private GamePlayState _playState;
    private GameGlobalMapState _globalMapState;

    private static Game GameInstance;

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
    public bool IsCombat { get => _isCombat; set => _isCombat = value; }
    public CombatManager Combat => _combatManager;
    public float CellStep { get => _cellStep; set => _cellStep = value; }

private void OnValidate()
    {
        if (!_mainMenu) _mainMenu = FindObjectOfType<MainMenu>();
        if (!_deck) _deck = GetComponent<Deck>();
        if (!_playerDeckContainer) _playerDeckContainer = transform.Find("PlayerDeck");
        if (!_layoutGroup3D) _layoutGroup3D = _playerDeckContainer.transform.GetComponent<AutoLayout3D.GridLayoutGroup3D>();
    }

    void Start()
    {
        DontDestroyOnLoad(gameObject);

        if (GameInstance == null)
        {
            GameInstance = this;
            Initialize();
        }
        else Destroy(gameObject);    
    }

    void Awake()
    {
        Application.targetFrameRate = 60;     
    }

    private void Initialize()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

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
            InitializeCombatScene();
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.GlobalMapSceneName))
        {
            _stateMachine.Initialize(_globalMapState);
            InitializeGlobalMapScene();
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
        _deck.LoadDeck();
        InitializePlayerDeck();
        _combatManager = FindObjectOfType<CombatManager>();
    }

    public void InitializeGlobalMapScene()
    {
        _puzzleController = FindObjectOfType<PuzzleController>();
        _puzzleController.Init(this);
    }

    public void ExitCombatScene()
    {
        // Действия при возврате в меню из боя
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
    {     
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.DeckBuildSceneName))
        {
            // Дополнительные дейсствия после загрузки сцены колодостроения
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.CombatSceneName))
        {
            InitializeCombatScene();
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName(Constants.CombatSceneName))
        {
            InitializeGlobalMapScene();
        }
    }

    private void InitializePlayerDeck() // Загружаем карты в руку игрока
    {
        foreach (string cardName in _deck.PlayerDeck)
        {
            Card card = _cardCollection.FindCard(cardName);
            if (card != null)
            {
                GameObject newCard = Instantiate(card.GameObject, _playerDeckContainer);
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
