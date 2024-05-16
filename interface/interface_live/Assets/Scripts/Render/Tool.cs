using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tool : Singleton<Tool>
{
    System.Random a = new System.Random();
    public int GetRandom(int min, int max)
    {
        return a.Next(min, max);
    }
    public Vector2 CellToUxy(int cellx, int celly)
    {
        return new Vector2(celly, 50 - cellx);
    }
    public Vector2 GridToUxy(float gridx, float gridy)
    {
        return new Vector2(gridy / 1000 - 0.5f, 50.5f - gridx / 1000);
    }
    public Vector2 GridToCell(Vector2 grid)
    {
        return new Vector2((int)(grid.x + 0.5f), (int)(grid.y + 0.5f));
    }
    // public Vector2 CellToGrid(Vector2 cell)
    // {
    //     return new Vector2(-0.5f + cell.y, 51.5f - cell.x);
    // }
    public bool CheckBeside(Vector2 grid, Vector2 cell)
    {
        if (Mathf.Abs(GridToCell(grid).x - cell.x) + Mathf.Abs(GridToCell(grid).y - cell.y) <= 2)
            return true;
        return false;
    }
    public bool CheckDistance(Vector2 grid, Vector2 cell, float dist)
    {
        if ((grid - cell).magnitude <= dist)
            return true;
        return false;
    }
}
