using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RendererControl : Singleton<RendererControl>
{
    public Tuple<Color, Color> GetColFromTeam(int teamKey)
    {
        switch (teamKey)
        {
            case 0: return new Tuple<Color, Color>(new Color(114f / 255, 107f / 255, 217f / 255, 1), new Color(107f / 255, 144f / 255, 217f / 255, 1));
            case 1: return new Tuple<Color, Color>(new Color(217f / 255, 107f / 255, 107f / 255, 1), new Color(217f / 255, 163f / 255, 107f / 255, 1));
            default: return new Tuple<Color, Color>(new Color(0, 0, 0, 1), new Color(0, 0, 0, 1));
        }
    }
}
