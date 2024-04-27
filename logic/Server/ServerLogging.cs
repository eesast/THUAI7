using Preparation.Utility.Logging;

namespace Server
{
    public static class GameServerLogging
    {
        public static readonly Logger logger = new("GameServer");
    }

    public static class PlaybackServerLogging
    {
        public static readonly Logger logger = new("PlaybackServer");
    }
}
