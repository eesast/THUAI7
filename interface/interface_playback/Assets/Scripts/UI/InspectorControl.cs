using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Protobuf;

public class InspectorControl : MonoBehaviour
{
    InteractBase interactBase;
    TextMeshProUGUI showText;
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
                switch (interactBase.messageOfObject.MessageOfObjCase)
                {
                    case MessageOfObj.MessageOfObjOneofCase.HomeMessage:
                        showText.text = "类型：基地" + (interactBase.messageOfObject.HomeMessage.TeamId == 0 ? "(RED)" : "(BLUE)") + "\n"
                        + "血量：" + interactBase.messageOfObject.HomeMessage.Hp + "/48000" + "\n";
                        break;
                    case MessageOfObj.MessageOfObjOneofCase.ShipMessage:
                        showText.text = "类型：" + ShipTypeToString(interactBase.messageOfObject.ShipMessage.ShipType)
                        + (interactBase.messageOfObject.ShipMessage.TeamId == 0 ? "(RED)" : "(BLUE)") + "\n"
                        + "血量：" + interactBase.messageOfObject.ShipMessage.Hp + " "
                        + WeaponTypeToString(interactBase.messageOfObject.ShipMessage.WeaponType) + "\n"
                        + ProducerTypeToString(interactBase.messageOfObject.ShipMessage.ProducerType) + " "
                        + ConstructorTypeToString(interactBase.messageOfObject.ShipMessage.ConstructorType) + "\n"
                        + ArmorTypeToString(interactBase.messageOfObject.ShipMessage.ArmorType) + " " + interactBase.messageOfObject.ShipMessage.Armor + "\n"
                        + ShieldTypeToString(interactBase.messageOfObject.ShipMessage.ShieldType) + " " + interactBase.messageOfObject.ShipMessage.Shield + "\n"
                        + interactBase.messageOfObject.ShipMessage.ShipState + "\n";
                        break;
                    case MessageOfObj.MessageOfObjOneofCase.ResourceMessage:
                        showText.text = "类型：资源" + "\n"
                        + "剩余资源：" + interactBase.messageOfObject.ResourceMessage.Progress + "/32000" + "\n";
                        break;
                    case MessageOfObj.MessageOfObjOneofCase.FactoryMessage:
                        showText.text = "类型：工厂" + (interactBase.messageOfObject.FactoryMessage.TeamId == 0 ? "(RED)" : "(BLUE)") + "\n"
                        + "血量：" + interactBase.messageOfObject.FactoryMessage.Hp + "/12000" + "\n";
                        break;
                    case MessageOfObj.MessageOfObjOneofCase.CommunityMessage:
                        showText.text = "类型：社区" + (interactBase.messageOfObject.CommunityMessage.TeamId == 0 ? "(RED)" : "(BLUE)") + "\n"
                        + "血量：" + interactBase.messageOfObject.CommunityMessage.Hp + "/10000" + "\n";
                        break;
                    case MessageOfObj.MessageOfObjOneofCase.FortMessage:
                        showText.text = "类型：堡垒" + (interactBase.messageOfObject.FortMessage.TeamId == 0 ? "(RED)" : "(BLUE)") + "\n"
                        + "血量：" + interactBase.messageOfObject.FortMessage.Hp + "/16000" + "\n";
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
            case ShipType.CivilianShip:
                return "民用舰船";
            case ShipType.MilitaryShip:
                return "军用舰船";
            case ShipType.FlagShip:
                return "旗舰";
            default: return "无类型";
        }
    }
    string ProducerTypeToString(ProducerType producerType)
    {
        switch (producerType)
        {
            case ProducerType.Producer1:
                return "基础采集器";
            case ProducerType.Producer2:
                return "高级采集器";
            case ProducerType.Producer3:
                return "终极采集器";
            default: return "无采集器";
        }
    }
    string ConstructorTypeToString(ConstructorType constructorType)
    {
        switch (constructorType)
        {
            case ConstructorType.Constructor1:
                return "基础建造器";
            case ConstructorType.Constructor2:
                return "高级建造器";
            case ConstructorType.Constructor3:
                return "终极建造器";
            default: return "无建造器";
        }
    }
    string ArmorTypeToString(ArmorType armorType)
    {
        switch (armorType)
        {
            case ArmorType.Armor1:
                return "基础装甲";
            case ArmorType.Armor2:
                return "高级装甲";
            case ArmorType.Armor3:
                return "终极装甲";
            default: return "无装甲";
        }
    }
    string ShieldTypeToString(ShieldType shieldType)
    {
        switch (shieldType)
        {
            case ShieldType.Shield1:
                return "基础护盾";
            case ShieldType.Shield2:
                return "高级护盾";
            case ShieldType.Shield3:
                return "终极护盾";
            default: return "无护盾";
        }
    }
    string WeaponTypeToString(WeaponType weaponTypes)
    {
        switch (weaponTypes)
        {
            case WeaponType.Lasergun:
                return "激光炮";
            case WeaponType.Plasmagun:
                return "等离子炮";
            case WeaponType.Shellgun:
                return "动能炮";
            case WeaponType.Missilegun:
                return "导弹发射器";
            case WeaponType.Arcgun:
                return "电弧炮";
            default: return "未装备武器";
        }
    }
}
