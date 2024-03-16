using CommandLine;

namespace Server
{
    public class DefaultArgumentOptions
    {
        public static string FileName = "xxxxxxxxxx";//
        public static string Token = "xxxxxxxxxx";//
        public static string Url = "xxxxxxxxxx";//
        public static string MapResource = "xxxxxxxxxx"; //
    }
    public class ArgumentOptions
    {
        [Option("ip", Required = false, HelpText = "Server listening ip")]
        public string ServerIP { get; set; } = "0.0.0.0";

        [Option('p', "port", Required = true, HelpText = "Server listening port")]
        public ushort ServerPort { get; set; } = 8888;

        [Option("teamCount", Required = false, HelpText = "The number of teams, 2 by defualt")]
        public ushort TeamCount { get; set; } = 2;

        [Option("civilShipNum", Required = false, HelpText = "The number of civil ship num, 1 by default")]
        public ushort CivilShipCount { get; set; } = 1;

        [Option("MaxCivilShipNum", Required = false, HelpText = "The number of max civil ship num, 2 by default")]
        public ushort MaxCivilShipCount { get; set; } = 2;

        [Option("MaxPlayerNumPerTeam", Required = false, HelpText = "The max player number of team, 6 by defualt")]
        public ushort MaxPlayerNumPerTeam { get; set; } = 6;

        [Option("warShipNum", Required = false, HelpText = "The number of war ship num, 0 by default")]
        public ushort WarShipCount { get; set; } = 0;

        [Option("MaxWarShipNum", Required = false, HelpText = "The number of war ship num, 2 by default")]
        public ushort MaxWarShipCount { get; set; } = 2;

        [Option("flagShipNum", Required = false, HelpText = "The number of flag ship num, 0 by default")]
        public ushort FlagShipCount { get; set; } = 0;

        [Option("MaxFlagShipNum", Required = false, HelpText = "The number of flag ship num, 1 by default")]
        public ushort MaxFlagShipCount { get; set; } = 1;

        [Option("MaxShipNum", Required = false, HelpText = "The max number of Ship, 5 by default")]
        public ushort MaxShipCount { get; set; } = 5;

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
        public string mapResource { get; set; } = DefaultArgumentOptions.MapResource;
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
        public ushort ShipCount => (ushort)(CivilShipCount + WarShipCount + FlagShipCount);
    }
}
