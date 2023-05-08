using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Framework.Extensions;

public static class ObjectExtensions
{
    [DebuggerStepThrough]
    public static B MapRef<A, B>(this A a, Func<A, B> func)
        where A : class
        where B : class
    {
        if (a.IsNotNull())
        {
            return func(a);
        }
        return null;
    }

    [DebuggerStepThrough]
    public static async Task<B> MapRefAsync<A, B>(this A a, Func<A, Task<B>> func)
        where A : class
        where B : class
    {
        if (a.IsNotNull())
        {
            return await func(a);
        }
        return default;
    }

    public static bool IsNull<T>([NotNullWhen(returnValue: false)] this T t)
        where T : class
    {
        return ReferenceEquals(t, null);
    }

    public static bool IsNotNull<T>([NotNullWhen(returnValue: true)] this T t)
        where T : class
    {
        return !ReferenceEquals(t, null);
    }

    public static bool IsNull<T>(this T? t)
        where T : struct
    {
        return !t.HasValue;
    }

    public static bool IsNotNull<T>(this T? t)
        where T : struct
    {
        return t.HasValue;
    }
}