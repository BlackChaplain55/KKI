using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

[RequireComponent(typeof(SoundManager))]
[RequireComponent(typeof(MenuComponents))]

//Ётот класс управл€ет основным меню
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;

    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _click;
    [SerializeField] private AudioClip _gameStart;
    [SerializeField] private MenuComponents _menuComponents;
    [SerializeField] private Image _blankScreen;
    [SerializeField] private Slider _musicVol;
    [SerializeField] private Slider _ambientVol;
    [SerializeField] private Slider _effectsVol;
    [SerializeField] private Toggle _toggleSound;
    [SerializeField] private StateMachine _stateMachine;

    [SerializeField] private MusicPlayer _musicPlayer;
    [SerializeField] private SoundManager _SFXManager;

    [SerializeField] private Game _game;

    private MenuCombatState _combatState;
    private MenuDeckBuildState _deckbuildState;
    private MenuDefaultState _defaultState;
    private MenuMainState _playState;
    private MenuGlobalMapState _globalMapState;

    public MenuCombatState CombatState => _combatState;
    public MenuDeckBuildState DeckBuildState => _deckbuildState;
    public MenuDefaultState DefaultState => _defaultState;
    public MenuMainState PlayState => _playState;
    public MenuGlobalMapState GlobalMapState => _globalMapState;

    public StateMachine StateMachine => _stateMachine;
    public IState CurrentState => _stateMachine.CurrentState;

    public MusicPlayer GameMusicPlayer => _musicPlayer;

    public MenuComponents Components { get => _menuComponents; }


private void OnValidate()
    {
        if (!_settingsPanel) _settingsPanel = transform.Find("SettingsPanel").gameObject;        
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_musicPlayer) _musicPlayer = GetComponent<MusicPlayer>();
        if (!_menuComponents) _menuComponents = GetComponent<MenuComponents>();
        if (!_SFXManager) _SFXManager = GetComponent<SoundManager>();
    }

    private void Awake()
    {
        if (!_blankScreen)
        {
            Transform blank = transform.Find("BlankScreen");
            if (blank) {
                _blankScreen = blank.GetComponent<Image>();
                blank.gameObject.SetActive(false);
            } 
        }
    }

    public void Initialize(IState state=null)
    {
        _stateMachine = new StateMachine();
        _combatState = new(_stateMachine, this);
        _defaultState = new(_stateMachine, this);
        _deckbuildState = new(_stateMachine, this);
        _globalMapState = new(_stateMachine, this);
        _playState = new(_stateMachine, this);
        if (state == null) _stateMachine.Initialize(_defaultState);
        else _stateMachine.Initialize(state);
        _SFXManager.Init();
    }

    public void ChangeState(IState state)
    {
        _stateMachine.ChangeState(state);
    }

    public void GameStart(EncounterData encounter)
    {
        _game.Encounter = encounter;
        FadeScreen(_game.PlayState, _gameStart);
    }

    public void GMStart()
    {
        FadeScreen(_game.GlobalMapState, _gameStart);
    }

    public void NewGame()
    {
        ProgressData progress = new();
        progress.Bastet = false;
        progress.Geb = false;
        progress.Thoth = false;
        progress.Meritseger = false;
        progress.InitialAP = 0;
        progress.InitialDeckBonus = 0;
        progress.TurnAPBonus = 0;
        progress.TurnCardBonus = 0;
        progress.PlayerPosition = new Vector3(15.16f, 4.201f, 175.8f);
        progress.CompleteEncounters = "";
        SaveLoadManager.SaveProgresData(progress);
        string startDeck = "PhisicalAttack1,PhisicalAttack1,PhisicalAttack1,PhisicalAttack1,PhisicalAttack2,PhisicalAttack2,MagicalAttack1,MagicalAttack1,MagicalAttack1,MagicalAttack1,DefenceBoost1," +
                "DefenceBoost1,PBlessing1,MBlessing1,Blessing2,Heal1,Heal1,Heal2,SpeedBoost2,CardDraw,CardDraw";
        PlayerPrefs.SetString("PlayerDeck", startDeck);
        FadeScreen(_game.GlobalMapState, _gameStart);        
    }

    public void GoToMainMenu()
    {
        FadeScreen(_game.StartState);
    }

    public void ContinueGame()
    {
        if(_game.CurrentState is GameMainMenuState|| _game.CurrentState is GameStartState)
        {
            GMStart();
        }
        else
        {
            _game.MainMenu.Components.MenuPanel.SetActive(false);
            EventBus.Instance.SetGMNavigation?.Invoke(true);
        }
            
    }

    public void DeckBuild()
    {
        FadeScreen(_game.DeckBuildState, _gameStart);
    }

    public void ShowConfirmationWindow(string dialogText, bool state)
    {
        _menuComponents.DialogText.text = dialogText;
        if (!_menuComponents.ConfirmPanel) _menuComponents.Validate();
        _menuComponents.ConfirmPanel.SetActive(state);
    }

    public void Confirm()
    {
        _menuComponents.ConfirmPanel.SetActive(false);
        EventBus.Instance.Confirm?.Invoke();
    }

    public void ShowMenu()
    {
        //Debug.Log("ShowMenu");
        //Debug.Log(_game.ToString());
        //Debug.Log(_game.MainMenu.ToString());
        //Debug.Log(_game.MainMenu.Components.ToString());
        //Debug.Log(_game.MainMenu.Components.MenuPanel.ToString());
        _game.MainMenu.Components.MenuPanel.SetActive(true);
        EventBus.Instance.SetGMNavigation?.Invoke(false);
        OnButtonClick();
    }

    public void HallOfGods()
    {
        OnButtonClick();
        _game.MainMenu.Components.MenuPanel.SetActive(false);
        GameObject.Find("MainMenuBG").SetActive(false);
    }
    private void FadeScreen(IState state, AudioClip sound=null) //Ётот метод обеспечивает плавную смену сцен
    {
        float delay = 0.1f;
        if (sound != null)
        {
            _audio.PlayOneShot(sound);
            delay = sound.length;
        }
        if (_blankScreen)
        {
            _blankScreen.color = new Color(0, 0, 0, 0);
            _blankScreen.gameObject.SetActive(true);
            _blankScreen.DOFade(1, delay);
        }
        StartCoroutine(DelayedStart(delay,state));
    }

    private IEnumerator DelayedStart(float delay, IState state)
    {
        yield return new WaitForSeconds(delay);
        _game.ChangeState(state);
        yield return new WaitForSeconds(2f);
        if (_blankScreen)
        {
            _blankScreen.DOFade(0, delay).OnComplete(() => {
                _blankScreen.gameObject.SetActive(false);
                });
        }
    }

    public void ToggleSound()   // —ледующие 4 метода обеспечивают работу экрана настроек
    {
        Settings settings = _game.GameSettings;
        settings.SoundEnabled = !settings.SoundEnabled;
        _game.SetSettings(settings);
    }

    public void OnSliderMove(Slider slider)
    {
        Settings settings=_game.GameSettings;
        if(slider == _musicVol) settings.MusicVol = slider.value;
        if (slider == _ambientVol) settings.AmbientVol = slider.value;
        if (slider == _effectsVol) settings.EffectsVol = slider.value;
        _game.SetSettings(settings);
    }

    public void SaveAndClose()
    {
        SaveLoadManager.SettingsSave(_game.GameSettings);
        _settingsPanel.SetActive(false);
    }

    public void SetSettingsUI(Settings settings)
    {
        _toggleSound.isOn = settings.SoundEnabled;
        _musicVol.value = settings.MusicVol;
        _ambientVol.value = settings.AmbientVol;
        _effectsVol.value = settings.EffectsVol;
    }
        
    public void Exit()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OnButtonClick()
    {
        if (_click) _audio.PlayOneShot(_click);
    }
}
