using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ёто класс отвечает за возможные действи€ юнита

public class UnitActions : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 20f;
    [SerializeField] private Unit _unit;

    private void OnValidate()
    {
        if (!_unit) _unit = GetComponent<Unit>();
    }
}
