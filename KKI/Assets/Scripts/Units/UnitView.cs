using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitView : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private Image _initiativeSlider;
    [SerializeField] private Image _healthSlider;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private GameObject _localCanvas;
    [SerializeField] private GameObject _selectIndicator;
    [SerializeField] private GameObject _highlight;

    private void OnValidate()
    {
        if (_localCanvas && Camera.main) _localCanvas.transform.rotation = Camera.main.transform.rotation;
        Transform unitCanvas = transform.Find("UnitCanvas");
        Transform panel=null;
        if (unitCanvas)
        {
            if (!_localCanvas) _localCanvas = unitCanvas.gameObject;
            panel = unitCanvas.Find("Panel");
            if (!_name && panel != null) _name = panel.Find("Name").GetComponent<TMP_Text>();
            if (!_healthSlider && panel != null) _healthSlider = panel.Find("HealthSlider").Find("Filler").GetComponent<Image>();
            if (!_initiativeSlider && panel != null) _initiativeSlider = panel.Find("InitiativeSlider").Find("Filler").GetComponent<Image>();
        }
        if (!_selectIndicator) _selectIndicator = transform.Find("Selection").gameObject;

        if (!_highlight)
        {
            Transform spotTransform = transform.Find("Spot");
            if (spotTransform) _highlight = spotTransform.gameObject;
        }
    }

    private void Awake()
    {
        if (_localCanvas && Camera.main) _localCanvas.transform.rotation = Camera.main.transform.rotation;
    }

    public void Init(Unit unit, string name)
    {
        _unit = unit;
        _name.text = name;
        _healthSlider.fillAmount = 1;
        _initiativeSlider.fillAmount = 0;
    }

    public void UpdateUI()
    {
        _healthSlider.fillAmount = _unit.CurrentHealth /_unit.MaxHealth;
        _initiativeSlider.fillAmount = _unit.CurrentInitiative/_unit.MaxInitiative;
    }

    public void SetSelect(bool state)
    {
        _selectIndicator.SetActive(state);
    }

    public void SetHighlight(bool state)
    {
        if (_highlight) _highlight.SetActive(state);
    }
}
