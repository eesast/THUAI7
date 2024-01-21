namespace Preparation.Interface
{
    public interface IObjOfShip : IGameObj
    {
        IShip? Parent { get; set; }
    }
}
