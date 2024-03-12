using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;

public class MessageManager : Singleton<MessageManager>
{

    public Dictionary<long, MessageOfShip> Ship = new Dictionary<long, MessageOfShip>();
    public Dictionary<long, GameObject> ShipG = new Dictionary<long, GameObject>();
    public Dictionary<long, MessageOfBullet> Bullet = new Dictionary<long, MessageOfBullet>();
    public Dictionary<long, GameObject> BulletG = new Dictionary<long, GameObject>();
}