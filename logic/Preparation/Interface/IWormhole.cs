using Preparation.Utility;
using System.Collections.Generic;

namespace Preparation.Interface
{
    public interface IWormhole : IGameObj
    {
        public List<XY> Entrance { get; }
        public List<XY> Content { get; }
    }
}
