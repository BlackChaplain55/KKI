using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Это генератор ячеек боевой сетки.Используется только из редактора
public class Grid : MonoBehaviour
{
    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private Cell _cellPrefab;
    [SerializeField] private float _offset;
    [SerializeField] private Vector3 _cellsize;

    public Vector3 CellSize => _cellsize;
    public float OffsetSize => _offset;

    [ContextMenu("Generate")]

    private void GenerateGrid()
    {
        ClearGrid();

        var cellsize = _cellPrefab.GetComponent<MeshRenderer>().bounds.size;
        _cellsize = cellsize;

        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                var position = new Vector3(x*(cellsize.x+_offset),transform.position.y,y * (cellsize.z + _offset));

                var cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);

                cell.Initialize(new Vector2Int(x, y));
                cell.name = "Gridcell_"+x+"_"+y;
            }
        }
    }

    [ContextMenu("Clear")]
    private void ClearGrid()
    {
        for (int i = transform.childCount-1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}
