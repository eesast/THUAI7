using System;
using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine;

public class CoreParam
{
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
    public static Dictionary<Tuple<int, int>, MessageOfObj> resources = new Dictionary<Tuple<int, int>, MessageOfObj>();
    public static Dictionary<Tuple<int, int>, GameObject> resourcesG = new Dictionary<Tuple<int, int>, GameObject>();
    public static MessageOfObj[] homes = new MessageOfObj[2];
    public static GameObject[] homesG = new GameObject[2];
    public static MessageOfTeam[] teams = new MessageOfTeam[2];
    public static Dictionary<long, MessageOfObj> ships = new Dictionary<long, MessageOfObj>();
    public static Dictionary<long, GameObject> shipsG = new Dictionary<long, GameObject>();
    public static Dictionary<long, MessageOfBullet> bullets = new Dictionary<long, MessageOfBullet>();
    public static Dictionary<long, GameObject> bulletsG = new Dictionary<long, GameObject>();
    public static Dictionary<Tuple<int, int>, MessageOfObj> factories = new Dictionary<Tuple<int, int>, MessageOfObj>();
    public static Dictionary<Tuple<int, int>, GameObject> factoriesG = new Dictionary<Tuple<int, int>, GameObject>();
    public static Dictionary<Tuple<int, int>, MessageOfObj> communities = new Dictionary<Tuple<int, int>, MessageOfObj>();
    public static Dictionary<Tuple<int, int>, GameObject> communitiesG = new Dictionary<Tuple<int, int>, GameObject>();
    public static Dictionary<Tuple<int, int>, MessageOfObj> forts = new Dictionary<Tuple<int, int>, MessageOfObj>();
    public static Dictionary<Tuple<int, int>, GameObject> fortsG = new Dictionary<Tuple<int, int>, GameObject>();
    public static Dictionary<Tuple<int, int>, MessageOfWormhole> wormholes = new Dictionary<Tuple<int, int>, MessageOfWormhole>();
    public static bool initialized;
    public static float playSpeed = 3;
    public static string fileName = "";
    public static int cnt = 0;
}
