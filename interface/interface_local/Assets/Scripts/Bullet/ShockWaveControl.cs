using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWaveControl : MonoBehaviour
{
    MessageOfBullet messageOfBullet;
    BulletData bulletData;
    SpriteRenderer spriteRenderer;
    public float lifeTime, timer;
    // Start is called before the first frame update
    void Start()
    {
        messageOfBullet = transform.parent.GetComponent<BulletControl>().messageOfBullet;
        bulletData = transform.parent.GetComponent<BulletControl>().bulletData;
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = new Vector3(bulletData.explosionRange, bulletData.explosionRange, bulletData.explosionRange);
        StartCoroutine(Disappear());
    }

    // Update is called once per frame
    void Update()
    {
    }
    IEnumerator Disappear()
    {
        timer = 0;
        while (timer < lifeTime)
        {
            if (timer > 0.5f)
                transform.GetComponent<Collider2D>().enabled = false;
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1 - timer / lifeTime);
            timer += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(1);
        Destroy(transform.parent.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D.CompareTag("Ship"))
        {
            if (collider2D.GetComponent<ShipControl>().messageOfShip.playerTeam != messageOfBullet.playerTeam)
            {
                Debug.Log("hitship!");
                if (bulletData.attackDamage.Length == 1)
                    collider2D.GetComponent<ShipControl>().TakeDamage(bulletData);
                else
                    collider2D.GetComponent<ShipControl>().TakeDamage(bulletData);
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
            }
        }
    }
}