using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это состояние игры при составлении колоды
public class GameDeckBuildState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Game _game;

    public GameDeckBuildState(StateMachine stateMachine, Game game)
    {
        _stateMachine = stateMachine;
        _game = game;
    }
    public void Enter()
    {
        _game.MainMenu.GameMusicPlayer.SetDeckbuildPlaylist();
        _game.MainMenu.SetDeckState();
        _game.MainMenu.MenuPanel.SetActive(false);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}