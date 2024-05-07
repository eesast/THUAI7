using Preparation.Utility.Value.SafeValue.LockedValue;

namespace Preparation.Interface
{
    public interface IHome
    {
        public long TeamID { get; }
        public InVariableRange<long> HP { get; }
    }
}
