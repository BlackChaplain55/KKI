using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это состояние игры при входе в меню
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
        _game.MainMenu.Components.MenuPanel.SetActive(true);
        if (_game.IsCombat)
        {
            _game.MainMenu.ChangeState(_game.MainMenu.PlayState);
        }
        else
        {
            _game.MainMenu.ChangeState(_game.MainMenu.DefaultState);
        }
    }

    public void Exit()
    {
        _game.MainMenu.Components.MenuPanel.SetActive(false);
    }

    public void Update()
    {

    }
}