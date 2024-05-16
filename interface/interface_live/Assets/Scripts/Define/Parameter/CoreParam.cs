using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;

public class CoreParam
{
    // public class FrameQueue<T>
    // {
    //     public FrameQueue(int _maxSize = 20)
    //     {
    //         maxSize = _maxSize;
    //         tail = maxSize - 1;
    //         valQueue = new T[maxSize];
    //     }
    //     public readonly int maxSize;
    //     private T[] valQueue;
    //     private int tail;
    //     public void Add(T val)
    //     {
    //         tail = (tail + 1) % maxSize;
    //         valQueue[tail] = val;
    //     }
    //     public T GetValue(int index)
    //     {
    //         if (index >= maxSize)
    //             return default;
    //         return valQueue[(tail + maxSize - index) % maxSize];
    //     }
    // };
    public class FrameQueue<T>
    {
        public FrameQueue()
        {
            valQueue = new Queue<T>();
        }
        private Queue<T> valQueue;
        public void Add(T val)
        {
            valQueue.Enqueue(val);
        }
        public T GetValue()
        {
            if (valQueue.Count == 0)
                return default;
            return valQueue.Dequeue();
        }
        public int GetSize()
        {
            return valQueue.Count;
        }
    };
    // public static FrameQueue<string> stringQueue = new FrameQueue<string>();
    public static FrameQueue<MessageToClient> frameQueue = new FrameQueue<MessageToClient>();
    public static MessageToClient firstFrame, currentFrame;
    public static MessageOfMap map;
    public static Dictionary<Tuple<int, int>, MessageOfResource> resources = new Dictionary<Tuple<int, int>, MessageOfResource>();
    public static MessageOfHome[] homes = new MessageOfHome[2];
    public static MessageOfTeam[] teams = new MessageOfTeam[2];
    public static Dictionary<long, MessageOfShip> ships = new Dictionary<long, MessageOfShip>();
    public static Dictionary<long, GameObject> shipsG = new Dictionary<long, GameObject>();
    public static Dictionary<long, MessageOfBullet> bullets = new Dictionary<long, MessageOfBullet>();
    public static Dictionary<long, GameObject> bulletsG = new Dictionary<long, GameObject>();
    public static Dictionary<Tuple<int, int>, MessageOfFactory> factories = new Dictionary<Tuple<int, int>, MessageOfFactory>();
    public static Dictionary<Tuple<int, int>, GameObject> factoriesG = new Dictionary<Tuple<int, int>, GameObject>();
    public static Dictionary<Tuple<int, int>, MessageOfCommunity> communities = new Dictionary<Tuple<int, int>, MessageOfCommunity>();
    public static Dictionary<Tuple<int, int>, GameObject> communitiesG = new Dictionary<Tuple<int, int>, GameObject>();
    public static Dictionary<Tuple<int, int>, MessageOfFort> forts = new Dictionary<Tuple<int, int>, MessageOfFort>();
    public static Dictionary<Tuple<int, int>, GameObject> fortsG = new Dictionary<Tuple<int, int>, GameObject>();
    public static Dictionary<Tuple<int, int>, MessageOfWormhole> wormholes = new Dictionary<Tuple<int, int>, MessageOfWormhole>();
    public static bool initialized;
    public static int cnt = 0;
}
