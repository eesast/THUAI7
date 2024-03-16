using System;

namespace Preparation.Utility;

public readonly struct XY
{
    public readonly int x;
    public readonly int y;

    #region ctor
    public XY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public XY(double angle, double length)
    {
        this.x = (int)(length * Math.Cos(angle));
        this.y = (int)(length * Math.Sin(angle));
    }
    public XY(XY Direction, double length)
    {
        if (Direction.x == 0 && Direction.y == 0)
        {
            this.x = 0;
            this.y = 0;
        }
        else
        {
            this.x = (int)(length * Math.Cos(Direction.Angle()));
            this.y = (int)(length * Math.Sin(Direction.Angle()));
        }
    }
    #endregion

    public override string ToString() => $"({x}, {y})";
    /// <returns>数量积</returns>
    public static int operator *(XY v1, XY v2) => (v1.x * v2.x) + (v1.y * v2.y);
    /// <returns>左数乘</returns>
    public static XY operator *(int a, XY v2) => new(a * v2.x, a * v2.y);
    /// <returns>右数乘</returns>
    public static XY operator *(XY v2, int a) => new(a * v2.x, a * v2.y);
    public static XY operator +(XY v1, XY v2) => new(v1.x + v2.x, v1.y + v2.y);
    public static XY operator -(XY v1, XY v2) => new(v1.x - v2.x, v1.y - v2.y);
    public static bool operator ==(XY v1, XY v2) => (v1.x == v2.x) && (v1.y == v2.y);
    public static bool operator !=(XY v1, XY v2) => (v1.x != v2.x) || (v1.y != v2.y);

    #region Distance
    public static double DistanceFloor3(XY p1, XY p2)
    {
        long c = (((long)(p1.x - p2.x) * (p1.x - p2.x)) + ((long)(p1.y - p2.y) * (p1.y - p2.y))) * 1000000;
        long t = c / 2 + 1;
        while (t * t > c || (t + 1) * (t + 1) <= c)
            t = (c / t + t) / 2;
        return t / 1000.0;
    }
    public static double DistanceCeil3(XY p1, XY p2)
    {
        long c = (((long)(p1.x - p2.x) * (p1.x - p2.x)) + ((long)(p1.y - p2.y) * (p1.y - p2.y))) * 1000000;
        long t = c / 2 + 1;
        while (t * t > c || (t + 1) * (t + 1) <= c)
            t = (c / t + t) / 2;
        if (t * t == c) return t / 1000.0;
        else return (t + 1) / 1000.0;
    }
    #endregion

    public double Length() => Math.Sqrt(((long)x * x) + ((long)y * y));
    public double Angle() => Math.Atan2(y, x);

    /// <summary>
    /// 逆时针旋转90°
    /// </summary>
    public XY Perpendicular() => new(-y, x);

    public override bool Equals(object? obj) => (obj is not null) && (obj is XY xy) && (this == xy);
    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

    /// <summary>
    /// 解包
    /// </summary>
    public void Deconstruct(out int x, out int y)
    {
        x = this.x; y = this.y;
    }
}

public readonly struct CellXY
{
    public readonly int x;
    public readonly int y;

    #region ctor
    public CellXY(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    public CellXY(double angle, double length)
    {
        this.x = (int)(length * Math.Cos(angle));
        this.y = (int)(length * Math.Sin(angle));
    }
    public CellXY(XY Direction, double length)
    {
        if (Direction.x == 0 && Direction.y == 0)
        {
            this.x = 0;
            this.y = 0;
        }
        else
        {
            this.x = (int)(length * Math.Cos(Direction.Angle()));
            this.y = (int)(length * Math.Sin(Direction.Angle()));
        }
    }
    #endregion

    public override string ToString() => $"({x}, {y})";
    /// <returns>数量积</returns>
    public static int operator *(CellXY v1, CellXY v2) => (v1.x * v2.x) + (v1.y * v2.y);
    /// <returns>左数乘</returns>
    public static CellXY operator *(int a, CellXY v2) => new(a * v2.x, a * v2.y);
    /// <returns>右数乘</returns>
    public static CellXY operator *(CellXY v2, int a) => new(a * v2.x, a * v2.y);
    public static CellXY operator +(CellXY v1, CellXY v2) => new(v1.x + v2.x, v1.y + v2.y);
    public static CellXY operator -(CellXY v1, CellXY v2) => new(v1.x - v2.x, v1.y - v2.y);
    public static bool operator ==(CellXY v1, CellXY v2) => (v1.x == v2.x) && (v1.y == v2.y);
    public static bool operator !=(CellXY v1, CellXY v2) => (v1.x != v2.x) || (v1.y != v2.y);

    #region Distance
    public static double DistanceFloor3(CellXY p1, CellXY p2)
    {
        long c = (((long)(p1.x - p2.x) * (p1.x - p2.x)) + ((long)(p1.y - p2.y) * (p1.y - p2.y))) * 1000000;
        long t = c / 2 + 1;
        while (t * t > c || (t + 1) * (t + 1) <= c)
            t = (c / t + t) / 2;
        return t / 1000.0;
    }
    public static double DistanceCeil3(CellXY p1, CellXY p2)
    {
        long c = (((long)(p1.x - p2.x) * (p1.x - p2.x)) + ((long)(p1.y - p2.y) * (p1.y - p2.y))) * 1000000;
        long t = c / 2 + 1;
        while (t * t > c || (t + 1) * (t + 1) <= c)
            t = (c / t + t) / 2;
        if (t * t == c) return t / 1000.0;
        else return (t + 1) / 1000.0;
    }
    #endregion

    public double Length() => Math.Sqrt(((long)x * x) + ((long)y * y));
    public double Angle() => Math.Atan2(y, x);

    /// <summary>
    /// 逆时针旋转90°
    /// </summary>
    public CellXY Perpendicular() => new(-y, x);

    public override bool Equals(object? obj) => (obj is not null) && (obj is CellXY xy) && (this == xy);
    public override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();

    /// <summary>
    /// 解包
    /// </summary>
    public void Deconstruct(out int x, out int y)
    {
        x = this.x; y = this.y;
    }
}
