using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это состояние игры при входе в основной боевой режим
public class GameGlobalMapState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Game _game;

    public GameGlobalMapState(StateMachine stateMachine, Game game)
    {
        _stateMachine = stateMachine;
        _game = game;
    }
    public void Enter()
    {
        if(_game.CurrentScene.name!= Constants.GlobalMapSceneName)  _game.LoadScene(Constants.GlobalMapSceneName);
        _game.MainMenu.GameMusicPlayer.SetGlobalMapPlaylist();
        _game.MainMenu.ChangeState(_game.MainMenu.GlobalMapState);
        _game.MainMenu.Components.MenuPanel.SetActive(false);
    }

    public void Exit()
    {

    }

    public void Update()
    {
    
    }
}
