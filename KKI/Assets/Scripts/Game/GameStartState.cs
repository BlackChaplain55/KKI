using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это изначальное состояние игры

public class GameStartState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Game _game;

    public GameStartState(StateMachine stateMachine, Game game)
    {
        _stateMachine = stateMachine;
        _game = game;
    }
    public void Enter()
    {
        if (_game.CurrentScene.name != Constants.StartSceneName) _game.LoadScene(Constants.StartSceneName);
        _game.MainMenu.GameMusicPlayer.SetMenuPlaylist();
        _game.MainMenu.Components.MenuPanel.SetActive(true);        
        _game.IsCombat = false;
    }

    public void Exit()
    {
        
    }

    public void Update()
    {

    }
}