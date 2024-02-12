using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ёто класс отвечает за возможные действи€ юнита

public class UnitActions : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 0.1f;
    public void MoveUnit(Cell cell, Cell newCell)
    {
        Unit unit = cell.Unit;
        unit.transform.LookAt(newCell.transform.position);
        cell.ShowMoves(false);
        cell.SetUnit(null);
        StartCoroutine(UnitMovement(unit, newCell));
    }

    public IEnumerator UnitMovement(Unit unit, Cell newCell)
    {
        bool hardStop = false;
        var end = newCell.transform.position;
        while (Vector3.Magnitude(unit.transform.position - end)>0.01f && !hardStop)
        {
            unit.transform.position = Vector3.MoveTowards(unit.transform.position, end, Time.deltaTime * movementSpeed);
            yield return new WaitForFixedUpdate();
        }
        newCell.SetUnit(unit);
        yield return null;
    }

}
