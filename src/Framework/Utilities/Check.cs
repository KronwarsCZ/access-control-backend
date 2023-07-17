using Framework.Option;

namespace Framework.Utilities;

public static class Check
{
    public static T Contract<T>(T obj, string errorMessage)
    {
        if (obj is null) throw new ContractException(errorMessage ?? $"Invalid {typeof(T)}");
        return obj;
    }

    public static T Contract<T>(Option<T> obj, string errorMessage)
        where T : class
    {
        if (obj.HasValue()) return obj.GetOrNull()!;
        throw new ContractException(errorMessage);
    }

    public static void Contract(bool val, string errorMessage)
    {
        if (val is false) throw new ContractException(errorMessage);
    }
}


