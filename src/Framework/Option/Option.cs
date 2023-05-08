using Framework.Extensions;

namespace Framework.Option;

public class Option<T>
    where T : class
{
    private T? _value;
    
    public static Option<T> Some(T obj)
    {
        return new Option<T> { _value = obj };
    }

    public static Option<T> None()
    {
        return new Option<T>();
    }

    public Option<TResult> Map<TResult>(Func<T, TResult> map)
        where TResult : class
    {
        return new Option<TResult>() { _value = _value.IsNotNull() ? map(_value) : null };
    }
    
    public T GetOrElse(T orElse)
    {
        return _value ?? orElse;
    }
    
    public T? GetOrNull()
    {
        return _value ?? null;
    }
}