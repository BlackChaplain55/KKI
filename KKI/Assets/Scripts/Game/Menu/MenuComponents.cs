using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuComponents : MonoBehaviour
{
    public GameObject MenuPanel;
    public GameObject ConfirmPanel;
    public TMP_Text DialogText;
    public Button StartButton;
    public Button ContinueButton;
    public Button ReturnButton;
    public Button DeckButton;
    public Button MainMenuButton;

    private void OnValidate()
    {
        if (!MenuPanel) MenuPanel = transform.Find("MainMenu").gameObject;
        if (!ConfirmPanel) ConfirmPanel = transform.Find("Dialogs")?.transform.Find("ConfirmPanel")?.gameObject;
        if (!DialogText) DialogText = transform.Find("Dialogs")?.transform.Find("ConfirmPanel")?.transform.Find("DialogText")?.GetComponent<TMP_Text>();
    }
}
