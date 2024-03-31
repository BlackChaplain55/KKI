using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuComponents : MonoBehaviour
{
    [SerializeField] public GameObject MenuPanel;
    [SerializeField] public GameObject MenuBG;
    [SerializeField] public GameObject ConfirmPanel;
    [SerializeField] public TMP_Text DialogText;
    [SerializeField] public Button StartButton;
    [SerializeField] public Button ContinueButton;
    [SerializeField] public Button ReturnButton;
    [SerializeField] public Button DeckButton;
    [SerializeField] public Button MainMenuButton;
    [SerializeField] public Button TutorialButton;
    [SerializeField] public Button PanteonButton;

    private void OnValidate()
    {
        Validate();
    }

    public void Validate()
    {
        if (!MenuBG) MenuBG = transform.Find("MainMenu")?.Find("MenuBG").gameObject;
        if (!MenuPanel) MenuPanel = transform?.Find("MainMenu")?.gameObject;
        if (!ConfirmPanel) ConfirmPanel = transform?.Find("Dialogs")?.transform.Find("ConfirmPanel")?.gameObject;
        if (!DialogText) DialogText = transform?.Find("Dialogs")?.transform.Find("ConfirmPanel")?.transform.Find("DialogText")?.GetComponent<TMP_Text>();
    }
}
