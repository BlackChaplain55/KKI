using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CombatUI : MonoBehaviour
{
    [SerializeField] private Image _turnFiller;
    [SerializeField] private CombatManager _combatManager;
    [SerializeField] private TMP_Text _APCounter;
    private void OnValidate()
    {
        if (!_turnFiller) _turnFiller = transform.Find("CombatUI")?.Find("Turn")?.Find("TurnFiller")?.GetComponent<Image>();
        if (!_APCounter) _APCounter = transform.Find("CombatUI")?.Find("ActionPoints")?.Find("APCounter")?.GetComponent<TMP_Text>();
    }

    public void Init(CombatManager manager)
    {
        _combatManager = manager;
        UpdateUI();
    }

    public void UpdateUI()
    {
        _turnFiller.fillAmount = _combatManager.CurrentTurnLength/_combatManager.TurnLength;
        _APCounter.text = _combatManager.ActionPoints.ToString();
    }
}
