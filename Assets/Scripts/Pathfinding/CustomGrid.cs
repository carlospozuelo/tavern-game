using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid<TGridObject>
{

    private int width, height;
    private float cellSize;

    private Vector3 originPosition;

    private TGridObject[,] matrix;

    private TextMesh[,] debugTextArray;

    public int GetWidth() { return width; }
    public int GetHeight() { return height; }

    public float GetCellSize() { return cellSize; }

    private bool debug = true;

    public CustomGrid(int width, int height, float cellSize, Vector3 originPosition, Func<CustomGrid<TGridObject>, int, int, TGridObject> createGridObject, bool debug = false)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.originPosition = originPosition;
        this.debug = debug;

        matrix = new TGridObject[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                matrix[x, y] = createGridObject(this, x, y);
            }
        }

        if (this.debug)
        {
            debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    debugTextArray[x, y] = Utils.CreateWorldText(matrix[x, y]?.ToString(), null, GetWorldPosition(x, y) + new Vector3(cellSize, cellSize) / 2, 2, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white, 100f);
        }

    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }

    public void SetValue(int x, int y, TGridObject value)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            matrix[x, y] = value;
            debugTextArray[x, y].text = value.ToString();
        }
        else
        {
            Debug.LogError("x or y out of bounds for the custom grid. Ignoring");
        }
    }

    public void UpdateValueText(int x, int y, Color color)
    {
        if (debug)
        {
            debugTextArray[x, y].text = matrix[x, y].ToString();
            debugTextArray[x, y].color = color;
        }
    }


    public void SetValue(Vector3 worldPosition, TGridObject value)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        SetValue(x, y, value);
    }

    public TGridObject GetValue(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < width && y < height)
        {
            return matrix[x, y];
        }
        else
        {
            Debug.LogError("x or y out of bounds for the custom grid. Ignoring");
            return default(TGridObject);
        }
    }

    public TGridObject GetValue(Vector3 worldPosition)
    {
        int x, y;
        GetXY(worldPosition, out x, out y);
        return GetValue(x, y);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

}
