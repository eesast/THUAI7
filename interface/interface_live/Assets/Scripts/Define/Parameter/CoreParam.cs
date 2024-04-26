using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;

public class CoreParam
{
    public class FrameQueue<T>
    {
        public FrameQueue(int _maxSize = 10)
        {
            maxSize = _maxSize;
            tail = maxSize - 1;
            valQueue = new T[maxSize];
        }
        public readonly int maxSize;
        private T[] valQueue;
        private int tail;
        public void Add(T val)
        {
            tail = (tail + 1) % maxSize;
            valQueue[tail] = val;
        }
        public T GetValue(int index)
        {
            if (index >= maxSize)
                return default;
            return valQueue[(tail + maxSize - index) % maxSize];
        }
    };
    public static FrameQueue<MessageToClient> frameQueue = new FrameQueue<MessageToClient>();
    public static MessageOfMap map;
    public static Dictionary<Tuple<int, int>, MessageOfResource> resources;
    public static MessageOfHome[] homes = new MessageOfHome[2];
    public static MessageOfTeam[] teams = new MessageOfTeam[2];
    public static MessageOfShip[] ships = new MessageOfShip[8];
    public static GameObject[] shipsG = new GameObject[8];
    public static Dictionary<long, MessageOfBullet> bullets;
    public static Dictionary<long, MessageOfBombedBullet> bombedBullets;
    public static List<MessageOfFactory> factories;
    public static List<MessageOfCommunity> communities;
    public static List<MessageOfFort> forts;
    public static bool initialized;
}
