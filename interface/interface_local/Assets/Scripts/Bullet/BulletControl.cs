using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public BulletData bulletData;
    public MessageOfBullet messageOfBullet;
    public float lifeTime;
    void Start()
    {
    }
    void Update()
    {
        RendererControl.GetInstance().SetColToChild(messageOfBullet.type, messageOfBullet.playerTeam, transform);
        lifeTime += Time.deltaTime;
        // Debug.Log(rb.velocity);
        if (lifeTime > bulletData.attackDistance / bulletData.speed)
        {
            if (messageOfBullet.type == BulletType.MISSILE)
            {
                transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                transform.Find("mask1").GetComponent<SpriteRenderer>().enabled = false;
                transform.Find("ShockWave").gameObject.SetActive(true);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (messageOfBullet.type == BulletType.MISSILE)
        {
            if (collider2D.CompareTag("Block"))
                MissileDestroy();
            if (collider2D.CompareTag("Ship"))
                if (collider2D.GetComponent<ShipControl>().messageOfShip.playerTeam != messageOfBullet.playerTeam)
                    MissileDestroy();
            if (collider2D.CompareTag("Base"))
                if (collider2D.GetComponent<BaseControl>().messageOfBase.playerTeam != messageOfBullet.playerTeam)
                    MissileDestroy();
            return;
        }
        if (collider2D.CompareTag("Ship"))
        {
            if (collider2D.GetComponent<ShipControl>().messageOfShip.playerTeam != messageOfBullet.playerTeam)
            {
                Debug.Log("hitship!");
                if (bulletData.attackDamage.Length == 1)
                    collider2D.GetComponent<ShipControl>().TakeDamage(bulletData);
                else
                    collider2D.GetComponent<ShipControl>().TakeDamage(bulletData);
                Destroy(gameObject);
            }
        }
        if (collider2D.CompareTag("Base"))
        {
            if (collider2D.GetComponent<BaseControl>().messageOfBase.playerTeam != messageOfBullet.playerTeam)
            {
                Debug.Log("hitbase!");
                if (bulletData.attackDamage.Length == 1)
                    collider2D.GetComponent<BaseControl>().TakeDamage(bulletData.attackDamage[0]);
                else
                    collider2D.GetComponent<BaseControl>().TakeDamage(Tool.GetInstance().GetRandom(bulletData.attackDamage[0], bulletData.attackDamage[1]));
                Destroy(gameObject);
            }
        }
        if (collider2D.CompareTag("Block"))
        {
            Destroy(gameObject);
        }
    }
    void MissileDestroy()
    {
        transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        transform.Find("mask1").GetComponent<SpriteRenderer>().enabled = false;
        transform.Find("ShockWave").gameObject.SetActive(true);
        Debug.Log("missle destroy");
    }
}
