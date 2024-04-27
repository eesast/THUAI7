using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Preparation.Utility.Value.SafeValue.SafeMethod
{
    static class CompareExchangeEnumImpl<T>
    {
        public delegate T dImpl(ref T location, T value, T comparand);
        public static readonly dImpl Impl = CreateCompareExchangeImpl();

        static dImpl CreateCompareExchangeImpl()
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(T), [typeof(T).MakeByRefType(), typeof(T), typeof(T)]);
            var ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Ldarg_2);
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(Interlocked).GetMethod(
                    "CompareExchange",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    [underlyingType.MakeByRefType(), underlyingType, underlyingType],
                    null));
            ilGenerator.Emit(OpCodes.Ret);
            return (dImpl)dynamicMethod.CreateDelegate(typeof(dImpl));
        }
    }

    static class ExchangeEnumImpl<T>
    {
        public delegate T dImpl(ref T location, T value);
        public static readonly dImpl Impl = CreateExchangeImpl();

        static dImpl CreateExchangeImpl()
        {
            var underlyingType = Enum.GetUnderlyingType(typeof(T));
            var dynamicMethod = new DynamicMethod(string.Empty, typeof(T), [typeof(T).MakeByRefType(), typeof(T)]);
            var ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(
                OpCodes.Call,
                typeof(Interlocked).GetMethod(
                    "Exchange",
                    BindingFlags.Static | BindingFlags.Public,
                    null,
                    [underlyingType.MakeByRefType(), underlyingType],
                    null));
            ilGenerator.Emit(OpCodes.Ret);
            return (dImpl)dynamicMethod.CreateDelegate(typeof(dImpl));
        }
    }

    public static class InterlockedEx
    {
        public static T CompareExchangeEnum<T>(ref T location, T value, T comparand)
        {
            return CompareExchangeEnumImpl<T>.Impl(ref location, value, comparand);
        }

        public static T ExchangeEnum<T>(ref T location, T value)
        {
            return ExchangeEnumImpl<T>.Impl(ref location, value);
        }

        public static T ReadEnum<T>(ref T location)
        {
            T dummy = default;
            return CompareExchangeEnum(ref location!, dummy, dummy)!;
        }
    }
}