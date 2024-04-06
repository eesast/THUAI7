using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ConstructionControl : MonoBehaviour
{
    public MessageOfConstruction messageOfConstruction;
    public ConstructionData constructionData;
    public List<ShipControl> enemyInRange;
    public ShipControl selectedEnemy;
    public GameObject repairIcon;
    public float iconRotateAngle, iconRotateTime;
    public int constructing;
    float constructCD;
    float faceAngle;
    void SetQ()
    {
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, 0, faceAngle), 0.03f);
        if (messageOfConstruction.playerTeam == PlayerTeam.BLUE)
            faceAngle = 180;
    }
    // Start is called before the first frame update
    void Start()
    {
        repairIcon = transform.Find("RepairIcon").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (messageOfConstruction.constructionType == ConstructionType.FORT)
        {
            RendererControl.GetInstance().SetColToChild(messageOfConstruction.playerTeam, gameObject.transform);
            SetQ();
        }
        constructCD -= Time.deltaTime;
        if (constructCD < 0)
            constructCD = 0;
        if (messageOfConstruction.hp == constructionData.hpMax && !messageOfConstruction.constructed)
        {
            messageOfConstruction.constructed = true;
            StartCoroutine(ProduceIE(constructionData.economyProduceSpeed));
            if (constructionData.attackDamage != 0)
                StartCoroutine(AttackIE());
        }
        if (messageOfConstruction.hp < constructionData.hpMax / 2 && messageOfConstruction.constructed)
        {
            messageOfConstruction.constructed = false;
        }
    }
    IEnumerator ProduceIE(int economy)
    {
        while (messageOfConstruction.constructed)
        {
            MapControl.GetInstance().bases[(int)messageOfConstruction.playerTeam].AddEconomy(economy);
            yield return new WaitForSeconds(1);
        }
    }
    IEnumerator AttackIE()
    {
        while (messageOfConstruction.constructed)
        {
            enemyInRange.Clear();
            foreach (ShipControl shipControl in EntityManager.GetInstance().ship)
            {
                if (shipControl.messageOfShip.playerTeam != messageOfConstruction.playerTeam &&
                    Tool.GetInstance().CheckDistance(new Vector2(shipControl.messageOfShip.x, shipControl.messageOfShip.y),
                        new Vector2(messageOfConstruction.x, messageOfConstruction.y), constructionData.attackRange))
                    enemyInRange.Add(shipControl);
            }
            if (enemyInRange.Count == 0)
                yield return null;
            else
            {
                selectedEnemy = enemyInRange[Tool.GetInstance().GetRandom(0, enemyInRange.Count)];
                faceAngle = Mathf.Atan2(messageOfConstruction.x - selectedEnemy.messageOfShip.x, selectedEnemy.messageOfShip.y - messageOfConstruction.y) *
                    Mathf.Rad2Deg;
                selectedEnemy.TakeDamage(ParaDefine.GetInstance().fortBulletData);
                yield return new WaitForSeconds(1);
            }
        }
    }
    public bool Construct(int constructAmount)
    {
        if (constructCD != 0)
            return false;
        constructCD = 1f;
        messageOfConstruction.hp += constructAmount;
        if (messageOfConstruction.hp >= constructionData.hpMax)
        {
            messageOfConstruction.hp = constructionData.hpMax;
            return true;
        }
        else
        {
            if (!repairIcon)
                repairIcon = transform.Find("RepairIcon").gameObject;
            if (!repairIcon.activeInHierarchy)
                repairIcon.SetActive(true);
            constructing++;
            StartCoroutine(RotateRepairIcon());
        }
        return false;
    }
    IEnumerator RotateRepairIcon()
    {
        float timer = 0;
        while (timer < iconRotateTime / 4)
        {
            timer += Time.deltaTime;
            repairIcon.transform.localRotation = Quaternion.Euler(0, 0, iconRotateAngle * timer * 4 / iconRotateTime);
            yield return null;
        }
        while (timer < 3 * iconRotateTime / 4)
        {
            timer += Time.deltaTime;
            repairIcon.transform.localRotation = Quaternion.Euler(0, 0, 2 * iconRotateAngle - iconRotateAngle * timer * 4 / iconRotateTime);
            yield return null;
        }
        while (timer < iconRotateTime)
        {
            timer += Time.deltaTime;
            repairIcon.transform.localRotation = Quaternion.Euler(0, 0, -4 * iconRotateAngle + iconRotateAngle * timer * 4 / iconRotateTime);
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        constructing--;
        if (constructing == 0)
            repairIcon.SetActive(false);
    }
    public void Damage(int damageAmount)
    {
        messageOfConstruction.hp -= damageAmount;
        if (messageOfConstruction.hp < 0)
            messageOfConstruction.hp = 0;
        EntityManager.GetInstance().emptyConstruction.Add(new Vector2(messageOfConstruction.x, messageOfConstruction.y));
        switch (messageOfConstruction.constructionType)
        {
            case ConstructionType.FACTORY:
                EntityManager.GetInstance().factory.Remove(this);
                break;
            case ConstructionType.COMMUNITY:
                EntityManager.GetInstance().community.Remove(this);
                break;
            case ConstructionType.FORT:
                EntityManager.GetInstance().fort.Remove(this);
                break;
            default: break;
        }
        Destroy(gameObject);
    }
}
