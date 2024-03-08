using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
public class PlayerControl : MonoBehaviour
{
    public Rigidbody2D rbPlayer;
    public float moveSpeed;
    private float targetQ;
    void SetVQTo(Vector2 targetV)
    {
        rbPlayer.velocity = Vector2.Lerp(rbPlayer.velocity, targetV, 0.05f);
        rbPlayer.rotation = Mathf.Lerp(rbPlayer.rotation, targetQ, 0.03f);
    }
    float DealQ(float qTar)
    {
        while (qTar - targetQ > 180)
            qTar -= 360;
        while (targetQ - qTar > 180)
            qTar += 360;
        return qTar;
    }
    void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            targetQ = DealQ(90);
            SetVQTo(new Vector2(-moveSpeed, 0));
            return;
        }
        if (Input.GetKey(KeyCode.D))
        {
            targetQ = DealQ(-90);
            SetVQTo(new Vector2(moveSpeed, 0));
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            targetQ = DealQ(0);
            SetVQTo(new Vector2(0, moveSpeed));
            return;
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetQ = DealQ(180);
            SetVQTo(new Vector2(0, -moveSpeed));
            return;
        }
        SetVQTo(new Vector2(0, 0));
    }
    void Start()
    {
        rbPlayer = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        Move();
    }
}
