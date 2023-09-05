using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Grid grid;

    public static GridManager instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        } else
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ClickOnGrid();
        }
    }

    public Vector3 SnapPosition(Vector3 position)
    {
        return grid.CellToWorld(grid.WorldToCell(position));
    }

    public void ClickOnGrid()
    {
        Debug.Log(grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
    }
}
