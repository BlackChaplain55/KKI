using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это подсвеченное состояние карты

public class MenuMainState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly MainMenu _mainMenu;

    public MenuMainState(StateMachine stateMachine, MainMenu mainMenu)
    {
        _stateMachine = stateMachine;
        _mainMenu = mainMenu;
    }
    public void Enter()
    {
        //_mainMenu.Components.ContinueButton.interactable = false;
        //_mainMenu.Components.StartButton.interactable = true;
        _mainMenu.Components.MainMenuButton.gameObject.SetActive(false);
        _mainMenu.Components.ReturnButton.gameObject.SetActive(false);
        _mainMenu.Components.DeckButton.gameObject.SetActive(true);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}