using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameMainMenuState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Game _game;

    public GameMainMenuState(StateMachine stateMachine, Game game)
    {
        _stateMachine = stateMachine;
        _game = game;
    }
    public void Enter()
    {
        _game.MainMenu.gameObject.SetActive(false);
    }

    public void Exit()
    {
        
    }

    public void Update()
    {

    }
}