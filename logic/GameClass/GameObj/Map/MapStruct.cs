using Preparation.Utility;
using System.IO;

namespace GameClass.MapGenerator;

/// <summary>
/// 地图结构体，本结构将会被MapGenerator-Python引用——asdawej
/// </summary>
public struct MapStruct
{
    public uint height;
    public uint width;
    public PlaceType[,] map;
    #region ctor
    public MapStruct(uint height, uint width)
    {
        this.height = height; this.width = width;
        map = new PlaceType[height, width];
    }
    public MapStruct(uint height, uint width, PlaceType[,] map)
    {
        this.height = height; this.width = width; this.map = map;
    }
    public MapStruct(uint height, uint width, uint[,] map)
    {
        this.height = height; this.width = width;
        this.map = new PlaceType[height, width];
        for (uint i = 0; i < height; i++)
        {
            for (uint j = 0; j < width; j++)
            {
                this.map[i, j] = (PlaceType)map[i, j];
            }
        }
    }
    #endregion
    /// <summary>
    /// 地图文件读取
    /// </summary>
    /// <param name="filename">地图文件</param>
    /// <returns></returns>
    public static MapStruct FromFile(string filename)
    {
        using BinaryReader br = new(File.OpenRead(filename));
        var height = br.ReadUInt32(); var width = br.ReadUInt32();
        MapStruct ret = new(height, width);
        for (uint i = 0; i < height; i++)
        {
            for (uint j = 0; j < width; j++)
            {
                ret.map[i, j] = (PlaceType)br.ReadUInt32();
            }
        }
        return ret;
    }
    /// <summary>
    /// 地图文件存储
    /// </summary>
    /// <param name="filename">地图文件</param>
    /// <param name="src">地图</param>
    public static void ToFile(string filename, MapStruct src)
    {
        using BinaryWriter bw = new(File.OpenWrite(filename));
        bw.Write(src.height);
        bw.Write(src.width);
        for (uint i = 0; i < src.height; i++)
        {
            for (uint j = 0; j < src.width; j++)
            {
                bw.Write((uint)src.map[i, j]);
            }
        }
    }
    /// <summary>
    /// 地图清空
    /// </summary>
    public readonly void Clear()
    {
        for (uint i = 0; i < height; i++)
        {
            for (uint j = 0; j < width; j++) map[i, j] = PlaceType.Null;
        }
    }
    public readonly PlaceType this[uint i, uint j]
    {
        get => map[i, j];
        set => map[i, j] = value;
    }
}