using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HallOfGodsManager : MonoBehaviour
{
    [SerializeField] List<GameObject> _unitsGO;
    [SerializeField] GameObject _currentUnit;
    [SerializeField] TMP_Text _heroStats;
    private void HideAll()
    {
        foreach (var unitGO in _unitsGO)
        {
            unitGO.SetActive(false);
        }
    }

    public void SetCurrnetUnit(GameObject go)
    {
        HideAll();
        go.SetActive(true);
        _currentUnit = go;
        Unit unit = _currentUnit.GetComponent<Unit>();
        if (!unit)
        {
            PlayerUnit pUnit = _currentUnit.GetComponent<PlayerUnit>();
            _heroStats.text = pUnit.View.GetStatsText();
        } else
            _heroStats.text = unit.View.GetStatsText();
    }

    public void ShowMenu()
    {
        GameObject.Find("UI").GetComponent<MainMenu>().ShowMenu();
    }
}
