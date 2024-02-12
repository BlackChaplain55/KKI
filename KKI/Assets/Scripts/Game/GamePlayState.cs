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
        _game.LoadScene(Constants.CombatSceneName);
        _game.InitializeCombatScene();
        _game.MainMenu.GameMusicPlayer.SetCombatPlaylist();
        _game.MainMenu.MenuPanel.SetActive(false);
        _game.MainMenu.SetPlayState();
    }

    public void Exit()
    {
        _game.ExitCombatScene();
    }

    public void Update()
    {
    
    }
}
