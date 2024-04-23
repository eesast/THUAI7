using System.Collections;
using System.Collections.Generic;
using Protobuf;
using UnityEngine.UIElements;

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
    public static FrameQueue<MessageToClient> frameQueue;
}
