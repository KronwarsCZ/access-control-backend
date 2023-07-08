namespace Application.Utilities;

public static class Check
{
    public static T Contract<T>(T obj, string errorMessage)
    {
        if (obj is null) throw new BusinessException(errorMessage ?? $"Invalid {typeof(T)}");
        return obj;
    }

    public static void Contract(bool val, string errorMessage)
    {
        if (val is false) throw new BusinessException(errorMessage);
    }
}

public class BusinessException : Exception
{
    public BusinessException(string message)
    {
        Message = message;
    }

    public override string Message { get; }
}
