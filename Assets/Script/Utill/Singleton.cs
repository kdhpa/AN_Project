using System;

public class Singleton<T> where T : class, new()
{
    private static readonly Lazy<T> _instance = new(() => new T());

    public static T Instance => _instance.Value;

    protected Singleton()
    {
        if (_instance.IsValueCreated)
        {
            throw new InvalidOperationException($"Singleton instance of {typeof(T).Name} already exists!");
        }
    }
}
