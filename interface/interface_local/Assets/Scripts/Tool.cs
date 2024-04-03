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
    public Vector2 GridToCell(Vector2 grid)
    {
        return new Vector2(50.5f - grid.y, 0.5f + grid.x);
    }
    // public Vector2 CellToGrid(Vector2 cell)
    // {
    //     return new Vector2(-0.5f + cell.y, 51.5f - cell.x);
    // }
    public bool CheckBeside(Vector2 grid, Vector2 cell)
    {
        if (Mathf.Abs(grid.x - cell.x) + Mathf.Abs(grid.y - cell.y) <= 2)
            return true;
        return false;
    }
}
