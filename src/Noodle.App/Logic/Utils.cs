using System.Buffers;

namespace Noodle.App.Logic;

public static class Utils
{
    private static readonly Random Generator = new Random();

    public static T Random<T>(this IList<T> list)
    {
        return list[Generator.Next(list.Count)];
    }

    public static void Generate(this Span<byte> span)
    {
        Generator.NextBytes(span);
    }

    public static void Generate(this Memory<byte> memory)
    {
        Generator.NextBytes(memory.Span);
    }

    public static void Generate(this byte[] buffer)
    {
        Generator.NextBytes(buffer);
    }
}