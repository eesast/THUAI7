using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class ConstructionControl : MonoBehaviour
{
    public MessageOfConstruction messageOfConstruction;
    public ConstructionData constructionData;
    public GameObject repairIcon;
    public float iconRotateAngle, iconRotateTime;
    public int constructing;
    float constructCD;
    // Start is called before the first frame update
    void Start()
    {
        repairIcon = transform.Find("RepairIcon").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        constructCD -= Time.deltaTime;
        if (constructCD < 0)
            constructCD = 0;
        if (messageOfConstruction.hp == constructionData.hpMax && !messageOfConstruction.constructed)
        {
            messageOfConstruction.constructed = true;
            StartCoroutine(ProduceIE(constructionData.economyProduceSpeed));
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
    public void Construct(int constructAmount)
    {
        if (constructCD != 0)
            return;
        constructCD = 1f;
        messageOfConstruction.hp += constructAmount;
        if (messageOfConstruction.hp > constructionData.hpMax)
        {
            messageOfConstruction.hp = constructionData.hpMax;
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
        PlaceManager.GetInstance().emptyConstruction.Add(new Vector2(messageOfConstruction.x, messageOfConstruction.y));
        PlaceManager.GetInstance().factory.Remove(this);
        Destroy(gameObject);
    }
}
