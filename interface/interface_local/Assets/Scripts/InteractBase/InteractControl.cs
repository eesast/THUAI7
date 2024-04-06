using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractControl : Singleton<InteractControl>
{
    public enum InteractType
    {
        NoneType,
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
        {InteractType.NoneType,
            null},
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
    public readonly Dictionary<InteractOption, string> textDic = new Dictionary<InteractOption, string>()
    {
        {InteractOption.None, ""},
        {InteractOption.BuildCivil, "建造民用舰船"},
        {InteractOption.BuildMilitary, "建造军用舰船"},
        {InteractOption.BuildFlag, "建造旗舰"},
        {InteractOption.RecycleShip, "回收舰船"},
        {InteractOption.RecoverShip, "修复舰船"},
        {InteractOption.InstallModuleArmor1, "安装基础装甲"},
        {InteractOption.InstallModuleArmor2, "安装高级装甲"},
        {InteractOption.InstallModuleArmor3, "安装终极装甲"},
        {InteractOption.InstallModuleShield1, "安装基础护盾"},
        {InteractOption.InstallModuleShield2, "安装高级护盾"},
        {InteractOption.InstallModuleShield3, "安装终极护盾"},
        {InteractOption.InstallModuleProducer1, "安装基础采集器"},
        {InteractOption.InstallModuleProducer2, "安装高级采集器"},
        {InteractOption.InstallModuleProducer3, "安装终极采集器"},
        {InteractOption.InstallModuleConstructor1, "安装基础建造器"},
        {InteractOption.InstallModuleConstructor2, "安装高级建造器"},
        {InteractOption.InstallModuleConstructor3, "安装终极建造器"},
        {InteractOption.InstallModuleLaserGun, "安装激光炮"},
        {InteractOption.InstallModulePlasmaGun, "安装等离子炮"},
        {InteractOption.InstallModuleShellGun, "安装动能炮"},
        {InteractOption.InstallModuleMissileGun, "安装导弹发射器"},
        {InteractOption.InstallModuleArcGun, "安装电弧炮"},
        {InteractOption.Produce, "采集资源"},
        {InteractOption.ConstructFactory, "建造工厂"},
        {InteractOption.ConstructCommunity, "建造社区"},
        {InteractOption.ConstructFort, "建造堡垒"},
        {InteractOption.RepairWormhole, "修复虫洞"},
    };
}
