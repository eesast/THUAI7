using Preparation.Utility;
using System.Collections.Generic;

namespace Preparation.Interface
{
    public interface IWormhole : IGameObj
    {
        public List<XY> Grids { get; }
    }
}
