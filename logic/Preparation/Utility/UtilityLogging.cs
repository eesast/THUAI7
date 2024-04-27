using Preparation.Utility.Logging;

namespace Preparation.Utility.Value.SafeValue
{
    public static class MyTimerLogging
    {
        public static readonly Logger logger = new("MyTimer");
    }
}

namespace Preparation.Utility.Value.SafeValue.LockedValue
{
    public static class LockedValueLogging
    {
        public static readonly Logger logger = new("LockedValue");
    }
}

namespace Preparation.Utility.Value.SafeValue.TimeBased
{

    public static class TimeBasedLogging
    {
        public static readonly Logger logger = new("TimeBased");
    }
}