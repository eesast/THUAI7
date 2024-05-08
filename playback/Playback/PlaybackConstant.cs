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
    public static readonly string FileExtension = $".thuaipb";
    /// <summary>
    /// 回放文件头
    /// </summary>
    public static readonly byte[] FileHeader = [(byte)'P', (byte)'B', Version, 0];
}
/// <summary>
/// 回放文件格式错误
/// </summary>
/// <param name="fileName"></param>
public class FileFormatNotLegalException(string fileName) : Exception
{
    public string FileName { get; } = fileName;
    public override string Message { get; }
        = $"The file: {fileName} is not a legal playback file for THUAI{Constants.Version}.";
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
    /// <summary>
    /// 文件头数据写入
    /// </summary>
    /// <param name="fs">文件写入流</param>
    /// <param name="teamCount">队伍数</param>
    /// <param name="playerCount">玩家数</param>
    public static void WriteHeader(this FileStream fs, uint teamCount, uint playerCount)
    {
        BinaryWriter bw = new(fs);
        bw.Write(Constants.FileHeader); // 写入文件头
        bw.Write(teamCount);            // 写入队伍数
        bw.Write(playerCount);          // 写入玩家数
    }
    /// <summary>
    /// 文件头数据读取
    /// </summary>
    /// <returns></returns>
    /// <param name="fs">文件读取流</param>
    /// <exception cref="FileFormatNotLegalException"></exception>
    public static (uint teamCount, uint playerCount) ReadHeader(this FileStream fs)
    {
        BinaryReader br = new(fs);
        if (!br.ReadBytes(Constants.FileHeader.Length)      // 判断文件头
               .SequenceEqual(Constants.FileHeader))
            throw new FileFormatNotLegalException(fs.Name);
        return (br.ReadUInt32(), br.ReadUInt32());          // 读取队伍数和每队玩家人数
    }
}
