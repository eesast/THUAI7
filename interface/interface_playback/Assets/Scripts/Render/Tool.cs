using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public Tuple<int, int> GridToCell(int gridx, int gridy)
    {
        return new Tuple<int, int>((int)(gridx + 500) / 1000, (int)(gridy + 500) / 1000);
    }
    // public Vector2 CellToGrid(Vector2 cell)
    // {
    //     return new Vector2(-0.5f + cell.y, 51.5f - cell.x);
    // }
}
