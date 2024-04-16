using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEditor.VersionControl;
using UnityEngine;

public class CoreParam
{
    public struct MessageOfFrame//存储一帧需要使用的信息
    {
        public MessageOfObj messageOfObj;//存储接受到的信息
    }
    public static MessageOfFrame messageOfFrame;
}
