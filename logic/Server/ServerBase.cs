using Protobuf;

namespace Server
{
    abstract class ServerBase : AvailableService.AvailableServiceBase
    {
        public abstract void WaitForEnd();
        public abstract int[] GetMoney();
        public abstract int[] GetScore();
    }
}
