using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это состояние игры при входе в основной боевой режим
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
        if(_game.CurrentScene.name!= Constants.CombatSceneName)  _game.LoadScene(Constants.CombatSceneName);
        _game.MainMenu.GameMusicPlayer?.SetCombatPlaylist();
        _game.MainMenu.ChangeState(_game.MainMenu.CombatState);
        _game.MainMenu.Components.MenuPanel.SetActive(false);
    }

    public void Exit()
    {
        _game.ExitCombatScene();
    }

    public void Update()
    {
    
    }
}
