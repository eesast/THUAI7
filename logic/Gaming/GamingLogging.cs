using Preparation.Utility.Logging;

namespace Gaming
{
    public static class GameLogging
    {
        public static readonly Logger logger = new("Game");
    }

    public static class ActionManagerLogging
    {
        public static readonly Logger logger = new("ActionManager");
    }

    public static class AttackManagerLogging
    {
        public static readonly Logger logger = new("AttackManager");
    }

    public static class ShipManagerLogging
    {
        public static readonly Logger logger = new("ShipManager");
    }
}