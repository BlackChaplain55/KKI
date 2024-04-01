using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это подсвеченное состояние карты

public class MenuGlobalMapState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly MainMenu _mainMenu;

    public MenuGlobalMapState(StateMachine stateMachine, MainMenu mainMenu)
    {
        _stateMachine = stateMachine;
        _mainMenu = mainMenu;
    }
    public void Enter()
    {
        _mainMenu.Components.MenuBG.SetActive(true);
        _mainMenu.Components.ContinueButton.interactable = true;
        _mainMenu.Components.StartButton.interactable = false;
        _mainMenu.Components.MainMenuButton.gameObject.SetActive(true);
        _mainMenu.Components.ContinueButton.gameObject.SetActive(true);
        _mainMenu.Components.StartButton.gameObject.SetActive(false);
        _mainMenu.Components.ReturnButton.gameObject.SetActive(true);
        _mainMenu.Components.TutorialButton.gameObject.SetActive(true);
        _mainMenu.Components.PanteonButton.gameObject.SetActive(false);
        _mainMenu.Components.DeckButton.gameObject.SetActive(true);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}