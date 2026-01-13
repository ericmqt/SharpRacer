using System.Runtime.CompilerServices;

namespace SharpRacer.Commands;

public static class CommandMessageAssert
{
    public static void Arg1Empty(CommandMessage message)
    {
        Assert.Equal(0, message.Arg1);
    }

    public static void Arg1Equals<TEnum>(TEnum arg1, CommandMessage message)
        where TEnum : unmanaged, Enum
    {
        var commandArgs1 = message.Arg1;

        var readEnum = Unsafe.As<ushort, TEnum>(ref commandArgs1);

        Assert.Equal(arg1, readEnum);
    }

    public static void Arg1Equals(ushort arg1, CommandMessage message)
    {
        Assert.Equal(arg1, message.Arg1);
    }

    public static void Arg2Empty(CommandMessage message)
    {
        Assert.Equal(0, message.Arg2);
    }

    public static void Arg2Equals<TEnum>(TEnum arg2, CommandMessage message)
        where TEnum : unmanaged, Enum
    {
        var commandArgs2 = message.Arg2;

        var readEnum = Unsafe.As<ushort, TEnum>(ref commandArgs2);

        Assert.Equal(arg2, readEnum);
    }

    public static void Arg2Equals(bool arg2, CommandMessage message)
    {
        Assert.Equal(arg2, message.Arg2 != 0);
    }

    public static void Arg2Equals(float arg2, CommandMessage message)
    {
        var real = (message.Arg2 << 16) | message.Arg3;
        var value = real / CommandMessageConstants.FloatArgument.ScaleFactor;

        Assert.Equal(arg2, value);
    }

    public static void Arg2Equals(int arg2, CommandMessage message)
    {
        var value = (message.Arg2 << 16) | message.Arg3;

        Assert.Equal(arg2, value);
    }

    public static void Arg2Equals(ushort arg2, CommandMessage message)
    {
        Assert.Equal(arg2, message.Arg2);
    }

    public static void Arg3Empty(CommandMessage message)
    {
        Assert.Equal(0, message.Arg3);
    }

    public static void Arg3Equals(ushort arg3, CommandMessage message)
    {
        Assert.Equal(arg3, message.Arg3);
    }
}
