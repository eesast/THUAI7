﻿using Preparation.Interface;
using Preparation.Utility;

namespace GameClass.GameObj.Occupations;

public class NullOccupation : IOccupation
{
    public static NullOccupation Instance { get; } = new();
    public int MoveSpeed => 0;
    public int MaxHp => 0;
    public int ViewRange => 0;
    public int Cost => 0;
    public int BaseArmor => 0;
    public int BaseShield => 0;
    public bool IsModuleValid(ModuleType moduleType) => false;
    private NullOccupation() { }
}
