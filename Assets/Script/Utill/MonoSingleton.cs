using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static bool _shuttingDown = false;
    private static object _locker = new object();
    private static T _instance = null;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);
        _instance = GetComponent<T>();
    }

    public static T Instance
    {
        get
        {
            if (_shuttingDown)
            {
                Debug.LogWarning("[Instance] Instance" + typeof(T) + "is already destroyed. Returning null.");
                return null;
            }
            lock (_locker)
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<T>();
                    if (_instance == null)
                    {
                        _instance = new GameObject(typeof(T).ToString()).AddComponent<T>();
                        DontDestroyOnLoad(_instance);
                    }
                }
                return _instance;
            }
        }
    }
}
