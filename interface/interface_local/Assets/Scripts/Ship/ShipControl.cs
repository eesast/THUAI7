using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    Rigidbody2D rb;
    public InteractBase interactBase;
    [SerializeField]
    private float targetQ;
    public MessageOfShip messageOfShip;
    GameObject obj;
    void SetVQTo(Vector2 targetV)
    {
        if ((rb.velocity.normalized - targetV.normalized).magnitude < 1.5f || rb.velocity.magnitude < 0.1f)
            rb.velocity = Vector2.Lerp(rb.velocity, targetV, 0.1f);
        else
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.3f);
        rb.rotation = Mathf.Lerp(rb.rotation, targetQ, 0.03f);
    }
    float DealQ(float qTar)
    {
        while (qTar - targetQ > 180)
            qTar -= 360;
        while (targetQ - qTar > 180)
            qTar += 360;
        return qTar;
    }
    // Start is called before the first frame update
    void Start()
    {
        interactBase = GetComponent<InteractBase>();
        rb = GetComponent<Rigidbody2D>();
        switch (messageOfShip.shipType)
        {
            case ShipType.CIVILIAN_SHIP:
                messageOfShip.hp = ParaDefine.GetInstance().civilShipData.maxHp;
                break;
            case ShipType.MILITARY_SHIP:
                messageOfShip.hp = ParaDefine.GetInstance().militaryShipData.maxHp;
                break;
            case ShipType.FLAG_SHIP:
                messageOfShip.hp = ParaDefine.GetInstance().flagShipData.maxHp;
                break;
            default:
                break;
        }
        messageOfShip.shipState = ShipState.IDLE;
        EntityManager.GetInstance().ship.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        messageOfShip.x = transform.position.x;
        messageOfShip.y = transform.position.y;
        RendererControl.GetInstance().SetColToChild(messageOfShip.playerTeam, gameObject.transform);
        switch (interactBase.interactOption)
        {
            case InteractControl.InteractOption.Produce:
                foreach (Vector2 resourcePos in EntityManager.GetInstance().resource)
                {
                    if (Tool.GetInstance().CheckBeside(transform.position, resourcePos))
                    {
                        if (messageOfShip.shipState != ShipState.PRODUCING)
                            Produce();
                    }
                }
                break;
            case InteractControl.InteractOption.RecoverShip:
                if (Tool.GetInstance().CheckBeside(transform.position,
                    new Vector2(MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].messageOfBase.x,
                        MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].messageOfBase.y)))
                {
                    if (messageOfShip.shipState != ShipState.RECOVERING)
                        Recover();
                }
                else
                {
                    foreach (ConstructionControl community in EntityManager.GetInstance().community)
                    {
                        if (community.messageOfConstruction.playerTeam == messageOfShip.playerTeam &&
                        Tool.GetInstance().CheckBeside(transform.position, new Vector2(community.messageOfConstruction.x, community.messageOfConstruction.y)))
                        {
                            if (messageOfShip.shipState != ShipState.RECOVERING)
                                Recover();
                            break;
                        }
                    }
                }
                break;
            case InteractControl.InteractOption.RecycleShip:
                if (Tool.GetInstance().CheckBeside(transform.position,
                    new Vector2(MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].messageOfBase.x,
                        MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].messageOfBase.y)))
                {
                    if (messageOfShip.shipState != ShipState.RECYCLING)
                        Recycle();
                }
                else
                {
                    foreach (ConstructionControl community in EntityManager.GetInstance().community)
                    {
                        if (community.messageOfConstruction.playerTeam == messageOfShip.playerTeam &&
                        Tool.GetInstance().CheckBeside(transform.position, new Vector2(community.messageOfConstruction.x, community.messageOfConstruction.y)))
                        {
                            if (messageOfShip.shipState != ShipState.RECYCLING)
                                Recycle();
                            break;
                        }
                    }
                }
                break;
            case InteractControl.InteractOption.InstallModuleLaserGun:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().laserData.cost))
                    messageOfShip.weaponType = WeaponType.LASERGUN;
                break;
            case InteractControl.InteractOption.InstallModulePlasmaGun:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().plasmaData.cost))
                    messageOfShip.weaponType = WeaponType.PLASMAGUN;
                break;
            case InteractControl.InteractOption.InstallModuleShellGun:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().shellData.cost))
                    messageOfShip.weaponType = WeaponType.SHELLGUN;
                break;
            case InteractControl.InteractOption.InstallModuleMissileGun:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().missileData.cost))
                    messageOfShip.weaponType = WeaponType.MISSILEGUN;
                break;
            case InteractControl.InteractOption.InstallModuleArcGun:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().arcData.cost))
                    messageOfShip.weaponType = WeaponType.ARCGUN;
                break;
            case InteractControl.InteractOption.InstallModuleArmor1:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().armor1Data.cost))
                {
                    messageOfShip.armorType = ArmorType.ARMOR1;
                    messageOfShip.armor = ParaDefine.GetInstance().armor1Data.armor;
                }
                break;
            case InteractControl.InteractOption.InstallModuleArmor2:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().armor2Data.cost))
                {
                    messageOfShip.armorType = ArmorType.ARMOR2;
                    messageOfShip.armor = ParaDefine.GetInstance().armor2Data.armor;
                }
                break;
            case InteractControl.InteractOption.InstallModuleArmor3:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().armor3Data.cost))
                {
                    messageOfShip.armorType = ArmorType.ARMOR3;
                    messageOfShip.armor = ParaDefine.GetInstance().armor3Data.armor;
                }
                break;
            case InteractControl.InteractOption.InstallModuleShield1:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().armor1Data.cost))
                {
                    messageOfShip.shieldType = ShieldType.SHIELD1;
                    messageOfShip.shield = ParaDefine.GetInstance().armor1Data.armor;
                }
                break;
            case InteractControl.InteractOption.InstallModuleShield2:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().armor2Data.cost))
                {
                    messageOfShip.shieldType = ShieldType.SHIELD2;
                    messageOfShip.shield = ParaDefine.GetInstance().armor2Data.armor;
                }
                break;
            case InteractControl.InteractOption.InstallModuleShield3:
                if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().armor3Data.cost))
                {
                    messageOfShip.shieldType = ShieldType.SHIELD3;
                    messageOfShip.shield = ParaDefine.GetInstance().armor3Data.armor;
                }
                break;
            case InteractControl.InteractOption.InstallModuleProducer1:
                if (messageOfShip.shipType != ShipType.MILITARY_SHIP &&
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().producer1Data.cost))
                    messageOfShip.producerType = ProducerType.PRODUCER1;
                break;
            case InteractControl.InteractOption.InstallModuleProducer2:
                if (messageOfShip.shipType != ShipType.MILITARY_SHIP &&
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().producer2Data.cost))
                    messageOfShip.producerType = ProducerType.PRODUCER2;
                break;
            case InteractControl.InteractOption.InstallModuleProducer3:
                if (messageOfShip.shipType != ShipType.MILITARY_SHIP &&
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().producer3Data.cost))
                    messageOfShip.producerType = ProducerType.PRODUCER3;
                break;
            case InteractControl.InteractOption.InstallModuleConstructor1:
                if (messageOfShip.shipType != ShipType.MILITARY_SHIP &&
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().constructor1Data.cost))
                    messageOfShip.constructorType = ConstructorType.CONSTRUCTOR1;
                break;
            case InteractControl.InteractOption.InstallModuleConstructor2:
                if (messageOfShip.shipType != ShipType.MILITARY_SHIP &&
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().constructor2Data.cost))
                    messageOfShip.constructorType = ConstructorType.CONSTRUCTOR2;
                break;
            case InteractControl.InteractOption.InstallModuleConstructor3:
                if (messageOfShip.shipType != ShipType.MILITARY_SHIP &&
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(ParaDefine.GetInstance().constructor3Data.cost))
                    messageOfShip.constructorType = ConstructorType.CONSTRUCTOR3;
                break;
            case InteractControl.InteractOption.ConstructFactory:
                for (int i = 0; i < EntityManager.GetInstance().emptyConstruction.Count; i++)
                {
                    if (Tool.GetInstance().CheckBeside(transform.position, EntityManager.GetInstance().emptyConstruction[i]))
                    {
                        obj = ObjCreater.GetInstance().CreateObj(ConstructionType.FACTORY,
                            EntityManager.GetInstance().emptyConstruction[i]);
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.playerTeam = messageOfShip.playerTeam;
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.x = (int)EntityManager.GetInstance().emptyConstruction[i].x;
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.y = (int)EntityManager.GetInstance().emptyConstruction[i].y;
                        EntityManager.GetInstance().emptyConstruction.Remove(EntityManager.GetInstance().emptyConstruction[i]);
                        EntityManager.GetInstance().factory.Add(obj.GetComponent<ConstructionControl>());
                    }
                }
                foreach (ConstructionControl factory in EntityManager.GetInstance().factory)
                {
                    if (factory.messageOfConstruction.playerTeam == messageOfShip.playerTeam &&
                    factory.messageOfConstruction.hp < ParaDefine.GetInstance().factoryData.hpMax &&
                    Tool.GetInstance().CheckBeside(transform.position, new Vector2(factory.messageOfConstruction.x, factory.messageOfConstruction.y)))
                    {
                        if (messageOfShip.shipState != ShipState.CONSTRUCTING)
                            Construct(factory);
                    }
                }
                break;
            case InteractControl.InteractOption.ConstructCommunity:
                for (int i = 0; i < EntityManager.GetInstance().emptyConstruction.Count; i++)
                {
                    if (Tool.GetInstance().CheckBeside(transform.position, EntityManager.GetInstance().emptyConstruction[i]))
                    {
                        obj = ObjCreater.GetInstance().CreateObj(ConstructionType.COMMUNITY,
                            EntityManager.GetInstance().emptyConstruction[i]);
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.playerTeam = messageOfShip.playerTeam;
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.x = (int)EntityManager.GetInstance().emptyConstruction[i].x;
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.y = (int)EntityManager.GetInstance().emptyConstruction[i].y;
                        EntityManager.GetInstance().emptyConstruction.Remove(EntityManager.GetInstance().emptyConstruction[i]);
                        EntityManager.GetInstance().community.Add(obj.GetComponent<ConstructionControl>());
                    }
                }
                foreach (ConstructionControl community in EntityManager.GetInstance().community)
                {
                    if (community.messageOfConstruction.playerTeam == messageOfShip.playerTeam &&
                    community.messageOfConstruction.hp < ParaDefine.GetInstance().communityData.hpMax &&
                    Tool.GetInstance().CheckBeside(transform.position, new Vector2(community.messageOfConstruction.x, community.messageOfConstruction.y)))
                    {
                        if (messageOfShip.shipState != ShipState.CONSTRUCTING)
                            Construct(community);
                    }
                }
                break;
            case InteractControl.InteractOption.ConstructFort:
                for (int i = 0; i < EntityManager.GetInstance().emptyConstruction.Count; i++)
                {
                    if (Tool.GetInstance().CheckBeside(transform.position, EntityManager.GetInstance().emptyConstruction[i]))
                    {
                        obj = ObjCreater.GetInstance().CreateObj(ConstructionType.FORT,
                            EntityManager.GetInstance().emptyConstruction[i], messageOfShip.playerTeam == PlayerTeam.BLUE);
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.playerTeam = messageOfShip.playerTeam;
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.x = (int)EntityManager.GetInstance().emptyConstruction[i].x;
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.y = (int)EntityManager.GetInstance().emptyConstruction[i].y;
                        EntityManager.GetInstance().emptyConstruction.Remove(EntityManager.GetInstance().emptyConstruction[i]);
                        EntityManager.GetInstance().fort.Add(obj.GetComponent<ConstructionControl>());
                    }
                }
                foreach (ConstructionControl fort in EntityManager.GetInstance().fort)
                {
                    if (fort.messageOfConstruction.playerTeam == messageOfShip.playerTeam &&
                    fort.messageOfConstruction.hp < ParaDefine.GetInstance().fortData.hpMax &&
                    Tool.GetInstance().CheckBeside(transform.position, new Vector2(fort.messageOfConstruction.x, fort.messageOfConstruction.y)))
                    {
                        if (messageOfShip.shipState != ShipState.CONSTRUCTING)
                            Construct(fort);
                    }
                }
                break;
            default:
                break;
        }
        MoveTowards(interactBase.moveOption);
        AttackTowards(interactBase.attackOption);
        interactBase.attackOption = Vector2.zero;
    }
    void Recover()
    {
        switch (messageOfShip.shipType)
        {
            case ShipType.CIVILIAN_SHIP:
                if (ParaDefine.GetInstance().civilShipData.maxHp - messageOfShip.hp > 0 &&
                    MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(
                    (int)((ParaDefine.GetInstance().civilShipData.maxHp / messageOfShip.hp - 1) * 1.2f * ParaDefine.GetInstance().civilShipData.cost)))
                    messageOfShip.hp = ParaDefine.GetInstance().civilShipData.maxHp;
                break;
            case ShipType.MILITARY_SHIP:
                if (ParaDefine.GetInstance().militaryShipData.maxHp - messageOfShip.hp > 0 &&
                    MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(
                    (int)((ParaDefine.GetInstance().militaryShipData.maxHp / messageOfShip.hp - 1) * 1.2f * ParaDefine.GetInstance().militaryShipData.cost)))
                    messageOfShip.hp = ParaDefine.GetInstance().militaryShipData.maxHp;
                break;
            case ShipType.FLAG_SHIP:
                if (ParaDefine.GetInstance().flagShipData.maxHp - messageOfShip.hp > 0 &&
                    MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(
                    (int)((ParaDefine.GetInstance().flagShipData.maxHp / messageOfShip.hp - 1) * 1.2f * ParaDefine.GetInstance().flagShipData.cost)))
                    messageOfShip.hp = ParaDefine.GetInstance().flagShipData.maxHp;
                break;
            default:
                break;
        }
        // StartCoroutine(RecoverIE());
    }
    void Recycle()
    {
        switch (messageOfShip.shipType)
        {
            case ShipType.CIVILIAN_SHIP:
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].AddEconomy(
                    (int)(messageOfShip.hp / ParaDefine.GetInstance().civilShipData.maxHp * ParaDefine.GetInstance().civilShipData.cost * 0.5f));
                DestroyShip();
                break;
            case ShipType.MILITARY_SHIP:
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].AddEconomy(
                    (int)(messageOfShip.hp / ParaDefine.GetInstance().militaryShipData.maxHp * ParaDefine.GetInstance().militaryShipData.cost * 0.5f));
                DestroyShip();
                break;
            case ShipType.FLAG_SHIP:
                MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].AddEconomy(
                    (int)(messageOfShip.hp / ParaDefine.GetInstance().flagShipData.maxHp * ParaDefine.GetInstance().flagShipData.cost * 0.5f));
                DestroyShip();
                break;
            default:
                break;
        }
        // StartCoroutine(RecoverIE());
    }
    void Produce()
    {
        switch (messageOfShip.producerType)
        {
            case ProducerType.PRODUCER1:
                StartCoroutine(ProduceIE(ParaDefine.GetInstance().producer1Data.miningSpeed));
                break;
            case ProducerType.PRODUCER2:
                StartCoroutine(ProduceIE(ParaDefine.GetInstance().producer2Data.miningSpeed));
                break;
            case ProducerType.PRODUCER3:
                StartCoroutine(ProduceIE(ParaDefine.GetInstance().producer3Data.miningSpeed));
                break;
            default:
                break;
        }
    }
    void Construct(ConstructionControl construction)
    {
        {
            switch (messageOfShip.constructorType)
            {
                case ConstructorType.CONSTRUCTOR1:
                    StartCoroutine(ConstructIE(construction, ParaDefine.GetInstance().constructor1Data.constructSpeed));
                    break;
                case ConstructorType.CONSTRUCTOR2:
                    StartCoroutine(ConstructIE(construction, ParaDefine.GetInstance().constructor2Data.constructSpeed));
                    break;
                case ConstructorType.CONSTRUCTOR3:
                    StartCoroutine(ConstructIE(construction, ParaDefine.GetInstance().constructor3Data.constructSpeed));
                    break;
                default:
                    break;
            }
        }
    }
    // IEnumerator RecoverIE()
    // {
    // messageOfShip.shipState = ShipState.RECOVERING;
    // while (messageOfShip.shipState == ShipState.RECOVERING)
    // {
    //     yield return new WaitForSeconds(1);
    // }

    // }
    IEnumerator ProduceIE(int economy)
    {
        // Debug.Log("try produce");
        messageOfShip.shipState = ShipState.PRODUCING;
        while (messageOfShip.shipState == ShipState.PRODUCING)
        {
            MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].AddEconomy(economy);
            yield return new WaitForSeconds(1);
        }

    }
    IEnumerator ConstructIE(ConstructionControl construction, int constructSpeed)
    {
        messageOfShip.shipState = ShipState.CONSTRUCTING;
        while (messageOfShip.shipState == ShipState.CONSTRUCTING)
        {
            Debug.Log("construct");
            if (MapControl.GetInstance().bases[(int)messageOfShip.playerTeam].CostEconomy(constructSpeed))
                if (construction.Construct(constructSpeed))
                    messageOfShip.shipState = ShipState.IDLE;
                else
                    yield return new WaitForSeconds(1);
            else
                yield return null;
        }

    }
    void MoveTowards(Vector2 pos)
    {
        if (interactBase.enableMove)
        {
            interactBase.enableMove = false;
            messageOfShip.shipState = ShipState.MOVING;
        }
        if (messageOfShip.shipState == ShipState.MOVING)
        {
            if ((pos - (Vector2)transform.position).magnitude > 0.3f)
            {
                targetQ = DealQ(Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg - 90);
                SetVQTo((pos - (Vector2)transform.position).normalized * messageOfShip.speed);
            }
            else
            {
                SetVQTo(Vector2.zero);
                messageOfShip.shipState = ShipState.IDLE;
                interactBase.enableMove = false;
            }
        }
        else
            SetVQTo(Vector2.zero);
    }
    void AttackTowards(Vector2 pos)
    {
        if (pos != Vector2.zero && messageOfShip.weaponType != WeaponType.NULL_WEAPON_TYPE)
        {

            messageOfShip.shipState = ShipState.ATTACKING;
            targetQ = DealQ(Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg - 90);
            SetVQTo(Vector2.zero);
            switch (messageOfShip.weaponType)
            {
                case WeaponType.LASERGUN:
                    obj = ObjCreater.GetInstance().CreateObj(BulletType.LASER, transform.position,
                        Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, pos - (Vector2)transform.position), Vector3.forward));
                    obj.GetComponent<Rigidbody2D>().velocity = ParaDefine.GetInstance().laserData.speed * (pos - (Vector2)transform.position).normalized;
                    obj.GetComponent<BulletControl>().messageOfBullet.playerTeam = messageOfShip.playerTeam;
                    break;
                case WeaponType.PLASMAGUN:
                    obj = ObjCreater.GetInstance().CreateObj(BulletType.PLASMA, transform.position,
                        Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, pos - (Vector2)transform.position), Vector3.forward));
                    obj.GetComponent<Rigidbody2D>().velocity = ParaDefine.GetInstance().plasmaData.speed * (pos - (Vector2)transform.position).normalized;
                    obj.GetComponent<BulletControl>().messageOfBullet.playerTeam = messageOfShip.playerTeam;
                    break;
                case WeaponType.SHELLGUN:
                    obj = ObjCreater.GetInstance().CreateObj(BulletType.SHELL, transform.position,
                        Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, pos - (Vector2)transform.position), Vector3.forward));
                    obj.GetComponent<Rigidbody2D>().velocity = ParaDefine.GetInstance().shellData.speed * (pos - (Vector2)transform.position).normalized;
                    obj.GetComponent<BulletControl>().messageOfBullet.playerTeam = messageOfShip.playerTeam;
                    break;
                case WeaponType.MISSILEGUN:
                    obj = ObjCreater.GetInstance().CreateObj(BulletType.MISSILE, transform.position,
                        Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, pos - (Vector2)transform.position), Vector3.forward));
                    obj.GetComponent<Rigidbody2D>().velocity = ParaDefine.GetInstance().missileData.speed * (pos - (Vector2)transform.position).normalized;
                    obj.GetComponent<BulletControl>().messageOfBullet.playerTeam = messageOfShip.playerTeam;
                    break;
                case WeaponType.ARCGUN:
                    obj = ObjCreater.GetInstance().CreateObj(BulletType.ARC, transform.position,
                        Quaternion.AngleAxis(Vector2.SignedAngle(Vector2.up, pos - (Vector2)transform.position), Vector3.forward));
                    obj.GetComponent<Rigidbody2D>().velocity = ParaDefine.GetInstance().arcData.speed * (pos - (Vector2)transform.position).normalized;
                    obj.GetComponent<BulletControl>().messageOfBullet.playerTeam = messageOfShip.playerTeam;
                    break;
            }
            messageOfShip.shipState = ShipState.IDLE;
        }
    }
    public void TakeDamage(BulletData bulletData)
    {
        if (messageOfShip.shield > 0 && bulletData.shieldDamageMultiplier != -1)
        {
            messageOfShip.shield -=
                (int)bulletData.shieldDamageMultiplier *
                bulletData.attackDamage.Count() <= 1 ?
                bulletData.attackDamage[0] :
                Tool.GetInstance().GetRandom(
                    bulletData.attackDamage[0], bulletData.attackDamage[1]);
            if (messageOfShip.shield < 0)
                messageOfShip.shield = 0;
            return;
        }
        if (messageOfShip.armor > 0 && bulletData.armorDamageMultiplier != -1)
        {
            messageOfShip.armor -=
                (int)bulletData.armorDamageMultiplier *
                bulletData.attackDamage.Count() <= 1 ?
                bulletData.attackDamage[0] :
                Tool.GetInstance().GetRandom(
                    bulletData.attackDamage[0], bulletData.attackDamage[1]);
            if (messageOfShip.shield < 0)
                messageOfShip.shield = 0;
            return;
        }
        messageOfShip.hp -=
            bulletData.attackDamage.Count() <= 1 ?
            bulletData.attackDamage[0] :
            Tool.GetInstance().GetRandom(
                bulletData.attackDamage[0], bulletData.attackDamage[1]);
        if (messageOfShip.hp < 0)
            messageOfShip.hp = 0;
        if (messageOfShip.hp == 0)
        {
            DestroyShip();
        }
    }
    void DestroyShip()
    {
        EntityManager.GetInstance().ship.Remove(this);
        Destroy(gameObject);
    }
}
