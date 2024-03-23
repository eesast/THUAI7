using CommandLine;

namespace Server
{
    public class DefaultArgumentOptions
    {
        public const string FileName = "xxxxxxxxxx";//
        public const string Token = "xxxxxxxxxx";//
        public const string Url = "xxxxxxxxxx";//
        public const string MapResource = "xxxxxxxxxx"; //
    }
    public class ArgumentOptions
    {
        [Option("ip", Required = false, HelpText = "Server listening ip")]
        public string ServerIP { get; set; } = "172.28.192.1";

        [Option('p', "port", Required = true, HelpText = "Server listening port")]
        public ushort ServerPort { get; set; } = 8888;

        [Option("teamCount", Required = false, HelpText = "The number of teams, 2 by defualt")]
        public ushort TeamCount { get; set; } = 2;

        [Option("shipNum", Required = false, HelpText = "The max number of Ship, 4 by default")]
        public ushort ShipCount { get; set; } = 4;

        [Option("homeNum", Required = false, HelpText = "The number of Home , 1 by default")]
        public ushort HomeCount { get; set; } = 1;

        [Option('g', "gameTimeInSecond", Required = false, HelpText = "The time of the game in second, 10 minutes by default")]
        public uint GameTimeInSecond { get; set; } = 10 * 60;
        [Option('f', "fileName", Required = false, HelpText = "The file to store playback file or to read file.")]
        public string FileName { get; set; } = "114514";
        [Option("notAllowSpectator", Required = false, HelpText = "Whether to allow a spectator to watch the game.")]
        public bool NotAllowSpectator { get; set; } = false;
        [Option('b', "playback", Required = false, HelpText = "Whether open the server in a playback mode.")]
        public bool Playback { get; set; } = false;
        [Option("playbackSpeed", Required = false, HelpText = "The speed of the playback, between 0.25 and 4.0")]
        public double PlaybackSpeed { get; set; } = 1.0;
        [Option("resultOnly", Required = false, HelpText = "In playback mode to get the result directly")]
        public bool ResultOnly { get; set; } = false;
        [Option('k', "token", Required = false, HelpText = "Web API Token")]
        public string Token { get; set; } = "114514";
        [Option('u', "url", Required = false, HelpText = "Web Url")]
        public string Url { get; set; } = "114514";
        [Option('m', "mapResource", Required = false, HelpText = "Map Resource Path")]
        public string MapResource { get; set; } = DefaultArgumentOptions.MapResource;
        [Option("requestOnly", Required = false, HelpText = "Only send web requests")]
        public bool RequestOnly { get; set; } = false;
        [Option("finalGame", Required = false, HelpText = "Whether it is the final game")]
        public bool FinalGame { get; set; } = false;
        [Option("cheatMode", Required = false, HelpText = "Whether to open the cheat code")]
        public bool CheatMode { get; set; } = false;
        [Option("resultFileName", Required = false, HelpText = "Result file name, saved as .json")]
        public string ResultFileName { get; set; } = "114514";
        [Option("startLockFile", Required = false, HelpText = "Whether to create a file that identifies whether the game has started")]
        public string StartLockFile { get; set; } = "114514";
        [Option("mode", Required = false, HelpText = "Whether to run final competition")]
        public int Mode { get; set; } = 0;
    }
}
