using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour
{
    public BulletData bulletData;
    public MessageOfBullet messageOfBullet;
    float lifeTime;
    Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        RendererControl.GetInstance().SetColToChild(messageOfBullet.type, messageOfBullet.playerTeam, transform);
        lifeTime += Time.deltaTime;
        // Debug.Log(rb.velocity);
        if (lifeTime > bulletData.attackDistance / bulletData.speed)
            Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Ship"))
        {
            if (collider2D.GetComponent<ShipControl>().messageOfShip.playerTeam != messageOfBullet.playerTeam)
            {
                Debug.Log("hitship!");
                if (bulletData.attackDamage.Length == 1)
                    collider2D.GetComponent<ShipControl>().TakeDamage(bulletData.attackDamage[0]);
                else
                    collider2D.GetComponent<ShipControl>().TakeDamage(Tool.GetInstance().GetRandom(bulletData.attackDamage[0], bulletData.attackDamage[1]));
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
    }
}
