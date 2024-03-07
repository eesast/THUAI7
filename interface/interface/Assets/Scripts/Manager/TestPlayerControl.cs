using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protobuf;
using System;

public class TestPlayerControl : MonoBehaviour
{
    public GameObject BulletFa;
    public Rigidbody2D rbPlayer;
    public float moveSpeed = 10f;
    private float targetQ;
    Vector2 mousePos, targetMovePos;
    bool moveByMouse = false;
    GameObject st, ed;
    void Start()
    {
        st = GameObject.Find("st");
        ed = GameObject.Find("ed");
    }
    void SetVQTo(Vector2 targetV, bool smoothMode = false)
    {
        if (smoothMode)
        {
            if (rbPlayer.velocity.normalized != targetV.normalized && rbPlayer.velocity.magnitude < 0.1f)
            {
                rbPlayer.velocity = Vector2.Lerp(rbPlayer.velocity, Vector2.zero, 0.05f);
            }
            else
            {
                rbPlayer.velocity = Vector2.Lerp(rbPlayer.velocity, targetV, 0.05f);
            }
        }
        else
        {
            rbPlayer.velocity = Vector2.Lerp(rbPlayer.velocity, targetV, 0.05f);
        }
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
        if (Input.GetMouseButtonDown(1))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetMovePos = mousePos;
            moveByMouse = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            targetQ = DealQ(90);
            SetVQTo(moveSpeed * Vector2.left);
            moveByMouse = false;
            return;
        }
        if (Input.GetKey(KeyCode.D))
        {
            targetQ = DealQ(-90);
            SetVQTo(moveSpeed * Vector2.right);
            moveByMouse = false;
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            targetQ = DealQ(0);
            SetVQTo(moveSpeed * Vector2.up);
            moveByMouse = false;
            return;
        }
        if (Input.GetKey(KeyCode.S))
        {
            targetQ = DealQ(180);
            SetVQTo(moveSpeed * Vector2.down);
            moveByMouse = false;
            return;
        }
        if (moveByMouse && (targetMovePos - (Vector2)transform.position).magnitude > 1f)
        {
            targetQ = DealQ(Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg - 90);
            SetVQTo(moveSpeed * new Vector2(-Mathf.Sin(targetQ), Mathf.Cos(targetQ)), true);
            return;
        }
        moveByMouse = false;
        SetVQTo(new Vector2(0, 0));
    }
    void Fire()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            targetQ = DealQ(Mathf.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) * Mathf.Rad2Deg - 90);
            if (!BulletFa)
                BulletFa = GameObject.Find("Bullet");
            GameObject obj = ObjectCreater.GetInstance().CreateObject(BulletType.Laser, transform.position, Quaternion.AngleAxis(targetQ, Vector3.forward), BulletFa.transform);
            obj.GetComponent<Rigidbody2D>().velocity = 20 * (Vector2.left * Mathf.Sin(targetQ * Mathf.Deg2Rad) + Vector2.up * Mathf.Cos(targetQ * Mathf.Deg2Rad));
        }
    }
    void Update()
    {
        Debug.Log(targetQ);
        if (!rbPlayer)
            rbPlayer = GetComponent<Rigidbody2D>();
        Move();
        Fire();
        ed.transform.position = st.transform.position + (Vector3)rbPlayer.velocity.normalized * 20;
    }
}
