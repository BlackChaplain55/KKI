using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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
        _game.MainMenu.GameMusicPlayer.SetMenuPlaylist();
    }

    public void Exit()
    {
        
    }

    public void Update()
    {

    }
}