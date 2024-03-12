using Preparation.Utility;
using System;
using System.IO;

namespace GameClass.MapGenerator;

#region 异常
public class MapFileError(string filename) : Exception
{
    public override string Message => $"{filename} is illegal map file";
}

public class MapStreamError(Stream str) : Exception
{
    public override string Message => $"{str.ToString} is illegal stream of map";
}
#endregion

/// <summary>
/// 地图结构体，本结构将会被MapGenerator-Python引用——asdawej
/// </summary>
public readonly struct MapStruct
{
    public readonly uint height;
    public readonly uint width;
    public readonly PlaceType[,] map;

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

    #region 文件读写
    /// <summary>
    /// 地图文件读取
    /// </summary>
    /// <param name="filename">待读取地图文件</param>
    /// <returns>读取得到的地图</returns>
    /// <exception cref="MapFileError"></exception>
    public static MapStruct FromFile(string filename)
    {
        using BinaryReader br = new(File.OpenRead(filename));
        try
        {
            var height = br.ReadUInt32(); var width = br.ReadUInt32();
            MapStruct ret = new(height, width);
            for (uint i = 0; i < height; i++)
            {
                for (uint j = 0; j < width; j++)
                {
                    ret[i, j] = (PlaceType)br.ReadUInt32();
                }
            }
            return ret;
        }
        catch
        {
            throw new MapFileError(filename);
        }
    }
    /// <summary>
    /// 地图文件存储
    /// </summary>
    /// <param name="filename">待写入地图文件</param>
    /// <param name="src">待写入地图</param>
    public static void ToFile(string filename, MapStruct src)
    {
        using BinaryWriter bw = new(File.OpenWrite(filename));
        bw.Write(src.height);
        bw.Write(src.width);
        for (uint i = 0; i < src.height; i++)
        {
            for (uint j = 0; j < src.width; j++)
            {
                bw.Write((uint)src[i, j]);
            }
        }
    }
    #endregion

    #region 流读写
    /// <summary>
    /// 字节提取
    /// </summary>
    /// <param name="str">字节输出流</param>
    /// <returns>提取得到的地图</returns>
    /// <exception cref="MapStreamError"></exception>
    public static MapStruct FromBytes(Stream str)
    {
        if (!str.CanRead) throw new MapStreamError(str);
        try
        {
            BinaryReader br = new(str);
            var height = br.ReadUInt32(); var width = br.ReadUInt32();
            MapStruct ret = new(height, width);
            for (uint i = 0; i < height; i++)
            {
                for (uint j = 0; j < width; j++)
                {
                    ret[i, j] = (PlaceType)br.ReadUInt32();
                }
            }
            return ret;
        }
        catch
        {
            throw new MapStreamError(str);
        }
    }
    /// <summary>
    /// 字节解析
    /// </summary>
    /// <param name="str">字节输入流</param>
    /// <param name="src">待解析地图</param>
    /// <exception cref="MapStreamError"></exception>
    public static void ToBytes(Stream str, MapStruct src)
    {
        if (!str.CanWrite) throw new MapStreamError(str);
        BinaryWriter bw = new(str);
        bw.Write(src.height);
        bw.Write(src.width);
        for (uint i = 0; i < src.height; i++)
        {
            for (uint j = 0; j < src.width; j++)
            {
                bw.Write((uint)src[i, j]);
            }
        }
    }
    #endregion

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