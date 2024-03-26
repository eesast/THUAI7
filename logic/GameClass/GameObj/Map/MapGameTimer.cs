using System;
using System.Threading;
using Preparation.Utility;
using ITimer = Preparation.Interface.ITimer;

namespace GameClass.GameObj
{
    public partial class Map
    {
        // xfgg说：爱因斯坦说，每个坐标系都有与之绑定的时钟，(x, y, z, ict) 构成四维时空坐标，在洛伦兹变换下满足矢量性（狗头）
        private readonly GameTimer timer = new();
        public ITimer Timer => timer;

        public class GameTimer : ITimer
        {
            private long startTime;
            public int nowTime() => (int)(Environment.TickCount64 - startTime);

            private readonly AtomicBool isGaming = new(false);
            public AtomicBool IsGaming => isGaming;

            public bool StartGame(int timeInMilliseconds)
            {
                if (!IsGaming.TrySet(true))
                    return false;
                startTime = Environment.TickCount64;
                Thread.Sleep(timeInMilliseconds);
                IsGaming.SetROri(false);
                return true;
            }
        }
    }
}
