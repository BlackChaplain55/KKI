using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuComponents : MonoBehaviour
{
    public GameObject MenuPanel;
    public Button StartButton;
    public Button ContinueButton;
    public Button ReturnButton;
    public Button DeckButton;
    public Button MainMenuButton;

    private void OnValidate()
    {
        if (!MenuPanel) MenuPanel = transform.Find("MainMenu").gameObject;
    }
}
