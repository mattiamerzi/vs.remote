namespace Vs.Remote.Utils;

internal static class ArrayExt
{
    public static T[] SkipA<T>(this T[] array, int count = 1)
    {
        return array.Skip(count).ToArray();
    }

    public static T[] SkipLastA<T>(this T[] array, int count = 1)
    {
        return array.SkipLast(count).ToArray();
    }

}
