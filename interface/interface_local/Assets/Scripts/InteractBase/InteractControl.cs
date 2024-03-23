using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractControl : Singleton<InteractControl>
{
    public enum InteractType
    {
        Base,
        Ship,

    }
    public enum InteractOption
    {
        None,
        BuildCivil,
        BuildMilitary,
        BuildFlag,
        RecycleShip,
        RecoverShip,
        InstallModuleArmor1,
        InstallModuleArmor2,
        InstallModuleArmor3,
        InstallModuleShield1,
        InstallModuleShield2,
        InstallModuleShield3,
        InstallModuleProducer1,
        InstallModuleProducer2,
        InstallModuleProducer3,
        InstallModuleConstructor1,
        InstallModuleConstructor2,
        InstallModuleConstructor3,
        InstallModuleLaserGun,
        InstallModulePlasmaGun,
        InstallModuleShellGun,
        InstallModuleMissileGun,
        InstallModuleArcGun,
        Produce,
        ConstructFactory,
        ConstructCommunity,
        ConstructFort,
        RepairWormhole,
    }
    public readonly Dictionary<InteractType, List<InteractOption>> interactOptions = new Dictionary<InteractType, List<InteractOption>>(){
        {InteractType.Base,
            new List<InteractOption>{
                InteractOption.BuildCivil,
                InteractOption.BuildMilitary,
                InteractOption.BuildFlag,
                }},
        {InteractType.Ship,
            new List<InteractOption>{
                InteractOption.RecycleShip,
                InteractOption.RecoverShip,
                InteractOption.InstallModuleArmor1,
                InteractOption.InstallModuleArmor2,
                InteractOption.InstallModuleArmor3,
                InteractOption.InstallModuleShield1,
                InteractOption.InstallModuleShield2,
                InteractOption.InstallModuleShield3,
                InteractOption.InstallModuleProducer1,
                InteractOption.InstallModuleProducer2,
                InteractOption.InstallModuleProducer3,
                InteractOption.InstallModuleConstructor1,
                InteractOption.InstallModuleConstructor2,
                InteractOption.InstallModuleConstructor3,
                InteractOption.InstallModuleLaserGun,
                InteractOption.InstallModulePlasmaGun,
                InteractOption.InstallModuleShellGun,
                InteractOption.InstallModuleMissileGun,
                InteractOption.InstallModuleArcGun,
                InteractOption.Produce,
                InteractOption.ConstructFactory,
                InteractOption.ConstructCommunity,
                InteractOption.ConstructFort,
                InteractOption.RepairWormhole,
                }},
    };
}
