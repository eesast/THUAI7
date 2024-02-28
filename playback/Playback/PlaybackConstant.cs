namespace Playback
{
    public static class PlayBackConstant
    {
        /// <summary>
        /// 版本
        /// </summary>
        public const int Version = 7;
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public static readonly string ExtendedName = $".thuai{Version}.pb";
        /// <summary>
        /// 文件头
        /// </summary>
        public static readonly byte[] FileHeader = [(byte)'P', (byte)'B', Version, 0];
        /// <summary>
        /// 缓冲区最大容量，10MiB
        /// </summary>
        public const int BufferMaxSize = 10 * 1024 * 1024;
    }
}
