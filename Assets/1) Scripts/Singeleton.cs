using UnityEngine;

public class Singeleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Sahnede varsa bul
                _instance = FindAnyObjectByType<T>();
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            Debug.Log($"[Singleton] Created instance of {typeof(T).Name} on {gameObject.name}");
        }
        else if (_instance != this)
        {
            Debug.LogWarning($"[Singleton] Duplicate {typeof(T).Name} on {gameObject.name}, destroying this one.");
            Destroy(gameObject);
        }
    }
}