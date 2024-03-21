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
        lifeTime += Time.deltaTime;
        Debug.Log(rb.velocity);
        if (lifeTime > bulletData.attackDistance / bulletData.speed)
            Destroy(gameObject);
    }
    private void OntriggetEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Ship"))
        {
            if (collider2D.GetComponent<ShipControl>().messageOfShip.playerTeam != messageOfBullet.playerTeam)
            {
                Debug.Log("hit!");
                Destroy(gameObject);
            }
        }
    }
}
