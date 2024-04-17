using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUpdateManager
{
    void UpdateMessageByJson(string jsonInfo);//jsonInfo为网站传入的更新的MessageOfObj类型，将其解析为messageOfObj类型
}
