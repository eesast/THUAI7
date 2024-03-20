using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControl : MonoBehaviour
{
    Rigidbody2D rb;
    public InteractBase interactBase;
    [SerializeField]
    float speed;
    private float targetQ;
    public MessageOfShip messageOfShip;
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
        MoveTowards(interactBase.moveOption);
        AttackTowards(interactBase.attackOption);
        interactBase.attackOption = Vector2.zero;
    }
    void MoveTowards(Vector2 pos)
    {
        if ((pos - (Vector2)transform.position).magnitude > 0.1f)
        {
            targetQ = DealQ(Mathf.Atan2(pos.y - transform.position.y, pos.x - transform.position.x) * Mathf.Rad2Deg - 90);
            SetVQTo((pos - (Vector2)transform.position).normalized * speed);
        }
        else
            SetVQTo(Vector2.zero);
    }
    void AttackTowards(Vector2 pos)
    {
        if (pos != Vector2.zero)
        {
            GameObject obj;
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
}
