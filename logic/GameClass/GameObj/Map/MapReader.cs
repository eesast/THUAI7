namespace MapGenerator;

/// <summary>
/// 地图结构体
/// </summary>
public struct MapStruct
{
    public MapStruct(uint height, uint width, uint[,] map)
    {
        this.height = height;
        this.width = width;
        this.map = map;
    }
    public uint height;
    public uint width;
    public uint[,] map;
}

public static class MapReader
{
    /// <summary>
    /// 读取二进制地图文件
    /// </summary>
    /// <param name="mapFile">地图文件路径</param>
    /// <param name="dtype">地图文件读取类型</param>
    /// <returns></returns>
    public static MapStruct MapRead(string mapFile, char dtype = 'B')
    {
        var fp = File.OpenRead(mapFile);
        BinaryReader br = new(fp);
        switch (dtype)
        {
            #region 以下支持

            case 'b':
                {
                    uint height = (uint)br.ReadSByte(), width = (uint)br.ReadSByte();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = (uint)br.ReadSByte();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'B':
                {
                    uint height = br.ReadByte(), width = br.ReadByte();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = br.ReadByte();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'u':
                {
                    uint height = br.ReadChar(), width = br.ReadChar();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = br.ReadChar();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'h':
                {
                    uint height = (uint)br.ReadInt16(), width = (uint)br.ReadInt16();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = (uint)br.ReadInt16();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'H':
                {
                    uint height = br.ReadUInt16(), width = br.ReadUInt16();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = br.ReadUInt16();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'i':
                {
                    uint height = (uint)br.ReadInt32(), width = (uint)br.ReadInt32();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = (uint)br.ReadInt32();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'I':
                {
                    uint height = br.ReadUInt32(), width = br.ReadUInt32();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = br.ReadUInt32();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }

            #endregion

            #region 以下支持但无效

            case 'l':
                {
                    uint height = (uint)br.ReadInt64(), width = (uint)br.ReadInt64();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = (uint)br.ReadInt64();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'L':
                {
                    uint height = (uint)br.ReadUInt64(), width = (uint)br.ReadUInt64();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = (uint)br.ReadUInt64();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'f':
                {
                    uint height = (uint)br.ReadSingle(), width = (uint)br.ReadSingle();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = (uint)br.ReadSingle();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'd':
                {
                    uint height = (uint)br.ReadDouble(), width = (uint)br.ReadDouble();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = (uint)br.ReadDouble();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }

            #endregion

            #region 以下不支持

            case 'q':
                {
                    uint height = br.ReadByte(), width = br.ReadByte();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = br.ReadByte();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            case 'Q':
                {
                    uint height = br.ReadByte(), width = br.ReadByte();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = br.ReadByte();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }
            default:
                {
                    uint height = br.ReadByte(), width = br.ReadByte();
                    uint[,] ret = new uint[height, width];
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            ret[i, j] = br.ReadByte();
                        }
                    }
                    return new MapStruct(height, width, ret);
                }

                #endregion
        };
    }
}