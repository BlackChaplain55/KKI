using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UnitView : MonoBehaviour
{
    [SerializeField] private Unit _unit;
    [SerializeField] private Image _initiativeSlider;
    [SerializeField] private Image _healthSlider;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _initiativeText;
    [SerializeField] private TMP_Text _effectText;
    [SerializeField] private TMP_Text _statsText;
    [SerializeField] private GameObject _healthDamage;
    [SerializeField] private GameObject _statsPanel;
    [SerializeField] private GameObject _heal;
    [SerializeField] private GameObject _initiativeDamage;
    [SerializeField] private GameObject _localCanvas;
    [SerializeField] private GameObject _selectIndicator;
    [SerializeField] private GameObject _highlight;

    private void OnValidate()
    {
        Transform unitCanvas = transform.Find("UnitCanvas");
        Transform panel = null;
        if (unitCanvas)
        {
            if (!_localCanvas) _localCanvas = unitCanvas.gameObject;
            if (_localCanvas && Camera.main) _localCanvas.transform.rotation = Camera.main.transform.rotation;
            panel = unitCanvas.Find("Panel");
            if (panel != null)
            {
                if (!_name) _name = panel.Find("NameBG")?.Find("Name")?.GetComponent<TMP_Text>();
                if (!_healthSlider) _healthSlider = panel.Find("HealthSlider")?.Find("Filler")?.GetComponent<Image>();
                if (!_initiativeSlider) _initiativeSlider = panel.Find("InitiativeSlider")?.Find("Filler")?.GetComponent<Image>();
                if (!_healthText) _healthText = panel.Find("HealthText")?.GetComponent<TMP_Text>();
                if (!_initiativeText) _initiativeText = panel.Find("InitiativeText")?.GetComponent<TMP_Text>();
                if (!_effectText) _effectText = panel.Find("EffectName")?.GetComponent<TMP_Text>();
                if (!_healthDamage) _healthDamage = panel.Find("HealthDamage")?.gameObject;
                if (!_initiativeDamage) _initiativeDamage = panel.Find("InitiativeDamage")?.gameObject;
                if (!_heal) _heal = panel.Find("Heal")?.gameObject;
                if (!_statsPanel) _statsPanel = panel.Find("StatsPanel").gameObject;
                if (!_statsText&& _statsPanel) _statsText = _statsPanel.transform.Find("Stats")?.GetComponent<TMP_Text>();
            }
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
        _statsPanel.SetActive(false);
    }

    public void ShowStats(bool state)
    {
        _statsPanel.SetActive(state);
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
        _healthSlider.fillAmount = _unit.CurrentHealth /(_unit.MaxHealth);
        _initiativeSlider.fillAmount = _unit.CurrentInitiative/(_unit.MaxInitiative);
        _healthText.text = _unit.CurrentHealth.ToString() + "/" + _unit.MaxHealth.ToString();
        _initiativeText.text = _unit.CurrentInitiative.ToString() + "/" + _unit.MaxInitiative.ToString();
        string damBonusColor = _unit.Bonus.Damage >= 0 ? "<color=\"green\">" : "<color=\"red\">";
        string defBonusColor = _unit.Bonus.Defence >= 0 ? "<color=\"green\">" : "<color=\"red\">";
        string mDamageBonusColor = _unit.Bonus.MDamage >= 0 ? "<color=\"green\">" : "<color=\"red\">";
        string mResistColor = _unit.Bonus.MResistance >= 0 ? "<color=\"green\">" : "<color=\"red\">";
        _statsText.text =   "���. ����: " + _unit.Damage.ToString() + "+" + damBonusColor + _unit.Bonus.Damage + "</color>\r\n" +
                            "������: " + _unit.Defence.ToString() + "\r\n" + "+" + defBonusColor + _unit.Bonus.Defence + "</color>\r\n" +
                            "���. ����: " + _unit.MDamage.ToString() + "\r\n" + "+" + mDamageBonusColor + _unit.Bonus.MDamage + "</color>\r\n" +
                            "���������.: " + _unit.MResistance.ToString() + "\r\n" + "+" + mResistColor + _unit.Bonus.MResistance + "</color>\r\n";
                            //"��������: " + _unit.Defence.ToString() + "\r\n" + "+" + _unit.Bonus.Defence + "\r\n" +
                            //"����������: " + _unit.Defence.ToString() + "\r\n" + "+" + _unit.Bonus.Defence + "\r\n" +
            ;
    }

    public void SetSelect(bool state)
    {
        _selectIndicator.SetActive(state);
    }

    public void SetHighlight(bool state)
    {
        if (_highlight) _highlight.SetActive(state);
    }

    public void Indicators(float damage, float initiative, float heal)
    {
        if (damage < 0)
        {
            _healthDamage.SetActive(true);
            _healthDamage.GetComponent<TMP_Text>().text = damage.ToString();
            _healthDamage.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1f,vibrato: 1, elasticity: 0.2f).OnComplete(()=> _healthDamage.SetActive(false));
        }

        if (heal > 0)
        {
            _heal.SetActive(true);
            _heal.GetComponent<TMP_Text>().text = "+"+heal.ToString();
            _heal.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1f, vibrato: 1, elasticity: 0.2f).OnComplete(() => _heal.SetActive(false));
        }

        if (initiative != 0)
        {
            _heal.SetActive(true);
            string sign="";
            if (initiative > 0) sign = "+";
            _heal.GetComponent<TMP_Text>().text =  sign+initiative.ToString();
            _heal.transform.DOPunchScale(new Vector3(1.5f, 1.5f, 1.5f), 1f, vibrato: 1, elasticity: 0.2f).OnComplete(() => _heal.SetActive(false));
        }
    }

    public void ShowEffectName(string text, bool isBonus)
    {
        if (isBonus)
        {
            _effectText.color = _unit.Combat.BonusColor;
        }
        else
        {
            _effectText.color = _unit.Combat.MalusColor;
        }

        _effectText.text = text;
        _effectText.gameObject.SetActive(true);
        _effectText.transform.DOPunchScale(new Vector3(1.2f, 1.2f, 1.2f), 2f, vibrato: 1, elasticity: 0.2f).OnComplete(() => _effectText.gameObject.SetActive(false));
    }
}
