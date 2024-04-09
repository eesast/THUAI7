using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InspectorControl : MonoBehaviour
{
    InteractBase interactBase;
    TextMeshProUGUI showText;
    BaseControl baseControl;
    ShipControl shipControl;
    // Start is called before the first frame update
    void Start()
    {
        showText = transform.Find("InspectorText").GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerControl.GetInstance().selectedInt.Count > 1)
        {
            showText.text = "已选择多个目标";
        }
        else
        {
            if (PlayerControl.GetInstance().selectedInt.Count == 1)
            {
                interactBase = PlayerControl.GetInstance().selectedInt[0];
                switch (interactBase.interactType)
                {
                    case InteractControl.InteractType.Base:
                        if (interactBase)
                            baseControl = interactBase.GetComponent<BaseControl>();
                        showText.text = "类型：基地" + (baseControl.messageOfBase.playerTeam == PlayerTeam.RED ? "(RED)" : "(BLUE)") + "\n"
                        + "经济：" + baseControl.messageOfBase.economy + "\n"
                        + "血量：" + baseControl.messageOfBase.hp + "/24000" + "\n";
                        break;
                    case InteractControl.InteractType.Ship:
                        if (interactBase)
                            shipControl = interactBase.GetComponent<ShipControl>();
                        showText.text = "类型：" + ShipTypeToString(shipControl.messageOfShip.shipType)
                        + (shipControl.messageOfShip.playerTeam == PlayerTeam.RED ? "(RED)" : "(BLUE)") + "\n"
                        + "血量：" + baseControl.messageOfBase.hp + " "
                        + WeaponTypeToString(shipControl.messageOfShip.weaponType) + "\n"
                        + ProducerTypeToString(shipControl.messageOfShip.producerType) + " "
                        + ConstructorTypeToString(shipControl.messageOfShip.constructorType) + "\n"
                        + ArmorTypeToString(shipControl.messageOfShip.armorType) + " "
                        + ShieldTypeToString(shipControl.messageOfShip.shieldType) + "\n"
                        + shipControl.messageOfShip.shipState + "\n";
                        break;
                    default: break;
                }
            }
        }
    }
    string ShipTypeToString(ShipType shipType)
    {
        switch (shipType)
        {
            case ShipType.CIVILIAN_SHIP:
                return "民用舰船";
            case ShipType.MILITARY_SHIP:
                return "军用舰船";
            case ShipType.FLAG_SHIP:
                return "旗舰";
            default: return "无类型";
        }
    }
    string ProducerTypeToString(ProducerType producerType)
    {
        switch (producerType)
        {
            case ProducerType.PRODUCER1:
                return "基础采集器";
            case ProducerType.PRODUCER2:
                return "高级采集器";
            case ProducerType.PRODUCER3:
                return "终极采集器";
            default: return "无采集器";
        }
    }
    string ConstructorTypeToString(ConstructorType constructorType)
    {
        switch (constructorType)
        {
            case ConstructorType.CONSTRUCTOR1:
                return "基础建造器";
            case ConstructorType.CONSTRUCTOR2:
                return "高级建造器";
            case ConstructorType.CONSTRUCTOR3:
                return "终极建造器";
            default: return "无建造器";
        }
    }
    string ArmorTypeToString(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.ARMOR1:
                return "基础装甲";
            case ArmorType.ARMOR2:
                return "高级装甲";
            case ArmorType.ARMOR3:
                return "终极装甲";
            default: return "无装甲";
        }
    }
    string ShieldTypeToString(ShieldType shieldType)
    {
        switch (shieldType)
        {
            case ShieldType.SHIELD1:
                return "基础护盾";
            case ShieldType.SHIELD2:
                return "高级护盾";
            case ShieldType.SHIELD3:
                return "终极护盾";
            default: return "无护盾";
        }
    }
    string WeaponTypeToString(WeaponType weaponTypes)
    {
        switch (weaponTypes)
        {
            case WeaponType.LASERGUN:
                return "激光炮";
            case WeaponType.PLASMAGUN:
                return "等离子炮";
            case WeaponType.SHELLGUN:
                return "动能炮";
            case WeaponType.MISSILEGUN:
                return "导弹发射器";
            case WeaponType.ARCGUN:
                return "电弧炮";
            default: return "未装备武器";
        }
    }
}
