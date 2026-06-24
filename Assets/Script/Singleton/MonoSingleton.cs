using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;

    public static T Ins
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    _instance = go.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    public void Init()
    {
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = (T)this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
