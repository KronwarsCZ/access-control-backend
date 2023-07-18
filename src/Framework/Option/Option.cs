using Framework.Extensions;

namespace Framework.Option;

public struct Option<T>
    where T : class?
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
    
    /// <summary>
    /// Evaluates a specified function, based on whether a value is present or not.
    /// </summary>
    /// <param name="some">The function to evaluate if the value is present.</param>
    /// <param name="none">The function to evaluate if the value is missing.</param>
    /// <returns>The result of the evaluated function.</returns>
    public TResult Match<TResult>(Func<T, TResult> some, Func<TResult> none)
    {
        if (some == null) throw new ArgumentNullException(nameof(some));
        if (none == null) throw new ArgumentNullException(nameof(none));
        return _value.IsNotNull() ? some(_value) : none();
    }

    /// <summary>
    /// Evaluates a specified action, based on whether a value is present or not.
    /// </summary>
    /// <param name="some">The action to evaluate if the value is present.</param>
    /// <param name="none">The action to evaluate if the value is missing.</param>
    public void Match(Action<T?> some, Action none)
    {
        if (some == null) throw new ArgumentNullException(nameof(some));
        if (none == null) throw new ArgumentNullException(nameof(none));

        if (_value.IsNotNull())
        {
            some(_value);
        }
        else
        {
            none();
        }
    }

    
    public T GetOrElse(T orElse)
    {
        return _value ?? orElse;
    }
    
    public T? GetOrNull()
    {
        return _value ?? null;
    }

    public bool HasValue()
    {
        return _value.IsNotNull();
    }
}