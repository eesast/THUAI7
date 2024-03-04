namespace Playback;
public static class Constants
{
    /// <summary>
    /// 回放版本
    /// </summary>
    public const int Version = 7;
    /// <summary>
    /// 回放文件扩展名
    /// </summary>
    public static readonly string FileExtension = $".thuai{Version}.pb";
    /// <summary>
    /// 回放文件头
    /// </summary>
    public static readonly byte[] FileHeader = [(byte)'P', (byte)'B', Version, 0];
    /// <summary>
    /// 缓冲区最大容量，10MiB
    /// </summary>
    public const int BufferMaxSize = 10 * 1024 * 1024;
}
public static class Utils
{
    /// <summary>
    /// 回放文件名正则化
    /// </summary>
    /// <param name="fileName">文件名</param>
    public static void FileNameRegular(ref string fileName)
    {
        if (!fileName.EndsWith(Constants.FileExtension))
        {
            fileName += Constants.FileExtension;
        }
    }
}
