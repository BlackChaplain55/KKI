using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

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

    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}