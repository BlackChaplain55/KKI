using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

//Ётот класс управл€ет основным меню
public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private GameObject _menuPanel;
    [SerializeField] private AudioSource _audio;
    [SerializeField] private AudioClip _click;
    [SerializeField] private AudioClip _gameStart;
    [SerializeField] private Button _startButton;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _deckButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Image _blankScreen;
    [SerializeField] private Slider _musicVol;    
    [SerializeField] private Slider _ambientVol;
    [SerializeField] private Slider _effectsVol;
    [SerializeField] private Toggle _toggleSound;

    private MusicPlayer _musicPlayer;
    private Game _game;

    public GameObject MenuPanel => _menuPanel;
    public MusicPlayer GameMusicPlayer => _musicPlayer;

    private void OnValidate()
    {
        if (!_settingsPanel) _settingsPanel = transform.Find("SettingsPanel").gameObject;        
        if (!_game) _game = FindObjectOfType<Game>();
        if (!_menuPanel) _menuPanel = transform.Find("MainMenu").gameObject;
        if (!_musicPlayer) _musicPlayer = GetComponent<MusicPlayer>();
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

    public void SetStartState()
    {
        _continueButton.enabled = false;
        _startButton.enabled = true;
        _mainMenuButton.gameObject.SetActive(false);
        _returnButton.gameObject.SetActive(false);
    }

    public void SetPlayState()
    {
        _continueButton.enabled = false;
        _startButton.enabled = true;
        _mainMenuButton.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(true);
        _returnButton.gameObject.SetActive(true);
        _deckButton.gameObject.SetActive(false);
    }
    public void SetMenuState()
    {
        _continueButton.enabled = true;
        _startButton.enabled = true;
        _continueButton.gameObject.SetActive(true);
        _startButton.gameObject.SetActive(true);
        _mainMenuButton.gameObject.SetActive(false);
        _returnButton.gameObject.SetActive(false);
        _deckButton.gameObject.SetActive(true);
    }

    public void SetDeckState()
    {
        _continueButton.enabled = true;
        _startButton.enabled = true;
        _mainMenuButton.gameObject.SetActive(true);
        _returnButton.gameObject.SetActive(true);
        _continueButton.gameObject.SetActive(false);
        _startButton.gameObject.SetActive(false);
        _deckButton.gameObject.SetActive(false);
    }

    public void GameStart()
    {
        FadeScreen(_game.PlayState, _gameStart);
    }

    public void GoToMainMenu()
    {
        FadeScreen(_game.MainMenuState);
    }

    public void ContinueGame()
    {
        _game.ChangeState(_game.PlayState);
    }

    public void DeckBuild()
    {
        FadeScreen(_game.PlayState, _gameStart);
    }

    private void FadeScreen(IState state, AudioClip sound=null) //Ётот метод обеспечивает плавную смену сцен
    {
        float delay = sound.length;
        if(sound!=null) _audio.PlayOneShot(sound);
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
