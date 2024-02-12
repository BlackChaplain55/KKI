using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePlayState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly Game _game;

    public GamePlayState(StateMachine stateMachine, Game game)
    {
        _stateMachine = stateMachine;
        _game = game;
    }
    public void Enter()
    {
        _game.LoadScene(Constants.CombatSceneName);
        
    }

    public void Exit()
    {

    }

    public void Update()
    {
    
    }
}
