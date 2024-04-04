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
    float speed;
    private float targetQ;
    public MessageOfShip messageOfShip;
    GameObject obj;
    void SetVQTo(Vector2 targetV)
    {
        if ((rb.velocity.normalized - targetV.normalized).magnitude < 1.5f || rb.velocity.magnitude < 0.1f)
            rb.velocity = Vector2.Lerp(rb.velocity, targetV, 0.1f);
        else
            rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, 0.1f);
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
    }

    // Update is called once per frame
    void Update()
    {
        RendererControl.GetInstance().SetColToChild(messageOfShip.playerTeam, gameObject.transform);
        switch (interactBase.interactOption)
        {
            case InteractControl.InteractOption.Produce:
                foreach (Vector2 resourcePos in PlaceManager.GetInstance().resource)
                {
                    if (Tool.GetInstance().CheckBeside(transform.position, resourcePos))
                    {
                        if (messageOfShip.shipState != ShipState.PRODUCING)
                            Produce();
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
            case InteractControl.InteractOption.ConstructFactory:
                // Debug.Log("choose to construct factory");
                for (int i = 0; i < PlaceManager.GetInstance().emptyConstruction.Count; i++)
                {
                    if (Tool.GetInstance().CheckBeside(transform.position, PlaceManager.GetInstance().emptyConstruction[i]))
                    {
                        obj = ObjCreater.GetInstance().CreateObj(ConstructionType.FACTORY,
                            PlaceManager.GetInstance().emptyConstruction[i]);
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.playerTeam = messageOfShip.playerTeam;
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.x = (int)PlaceManager.GetInstance().emptyConstruction[i].x;
                        obj.GetComponent<ConstructionControl>().messageOfConstruction.y = (int)PlaceManager.GetInstance().emptyConstruction[i].y;
                        PlaceManager.GetInstance().emptyConstruction.Remove(PlaceManager.GetInstance().emptyConstruction[i]);
                        PlaceManager.GetInstance().factory.Add(obj.GetComponent<ConstructionControl>());
                    }
                }
                foreach (ConstructionControl factory in PlaceManager.GetInstance().factory)
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
            default:
                break;
        }
        MoveTowards(interactBase.moveOption);
        AttackTowards(interactBase.attackOption);
        interactBase.attackOption = Vector2.zero;
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
            construction.Construct(constructSpeed);
            yield return new WaitForSeconds(1);
        }

    }
    void MoveTowards(Vector2 pos)
    {
        if (interactBase.enableMove)
        {
            if ((pos - (Vector2)transform.position).magnitude > 0.1f)
            {
                targetQ = DealQ(Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg - 90);
                SetVQTo((pos - (Vector2)transform.position).normalized * speed);
            }
            else
            {
                SetVQTo(Vector2.zero);
                interactBase.enableMove = false;
            }

            messageOfShip.shipState = ShipState.MOVING;
        }
        else
            SetVQTo(Vector2.zero);
    }
    void AttackTowards(Vector2 pos)
    {
        if (pos != Vector2.zero)
        {

            messageOfShip.shipState = ShipState.ATTACKING;
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
        Destroy(gameObject);
    }
}
