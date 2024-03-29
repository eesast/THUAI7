# Value

---

用于为游戏引擎提供预先写好的通用类。

- 类的部分方法遵循下面原则：

```
  Set //返回值为void，类似有Add,Sub

  SetRNow //返回操作后的值

  SetRChange //返回操作后减去操作前的差值

  SetROri  //返回操作前的值
```



## SafeValue

所有类型的操作是线程安全的

- 不能保证不出现死锁
