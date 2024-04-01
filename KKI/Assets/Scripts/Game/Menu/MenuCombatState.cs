using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

//Это подсвеченное состояние карты

public class MenuCombatState : IState
{
    private readonly StateMachine _stateMachine;
    private readonly MainMenu _mainMenu;

    public MenuCombatState(StateMachine stateMachine, MainMenu mainMenu)
    {
        _stateMachine = stateMachine;
        _mainMenu = mainMenu;
    }
    public void Enter()
    {
        _mainMenu.Components.MenuBG.SetActive(true);
        _mainMenu.Components.ContinueButton.interactable = true;
        _mainMenu.Components.StartButton.gameObject.SetActive(true);
        _mainMenu.Components.MainMenuButton.gameObject.SetActive(true);
        _mainMenu.Components.ContinueButton.gameObject.SetActive(true);
        _mainMenu.Components.StartButton.gameObject.SetActive(true);
        _mainMenu.Components.ReturnButton.gameObject.SetActive(true);
        _mainMenu.Components.TutorialButton.gameObject.SetActive(true);
        _mainMenu.Components.PanteonButton.gameObject.SetActive(false);
        //Доступ к колоде временно доступен
        _mainMenu.Components.DeckButton.gameObject.SetActive(false);
    }

    public void Exit()
    {

    }

    public void Update()
    {

    }
}