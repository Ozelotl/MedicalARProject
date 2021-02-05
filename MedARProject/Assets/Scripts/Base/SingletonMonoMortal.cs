//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton for a Component that is NOT set to DontDestroyOnLoad
/// use for Manager classes whose lifetime is managed by scenes
/// </summary>
[DefaultExecutionOrder (-100)]
public class SingletonMonoMortal<T> : MonoBehaviour where T : Component
{
    private static T _Instance;
    public static T Instance { get { return _Instance; } }

    protected virtual void Awake()
    {
        if (_Instance != null)
        {
            Debug.LogWarning("More than one instance of " + nameof(T) + " exists. This will be destroyed!", this.gameObject);
            Destroy(this);
        }
        else
            _Instance = this as T;
    }
}
