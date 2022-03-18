namespace Noodle.App.Logic;

public static class Utils
{
    private static readonly Random Generator = new Random();

    public static T Random<T>(this IList<T> list)
    {
        return list[Generator.Next(list.Count)];
    }
}