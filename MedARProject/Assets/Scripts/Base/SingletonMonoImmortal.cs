//Stella

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMonoImmortal : MonoBehaviour
{
    private static SingletonMonoImmortal _Instance;
    public static SingletonMonoImmortal Instance { get { return _Instance; } }

    protected virtual void Awake()
    {
        if (_Instance != null)
        {
            Debug.LogWarning("More than one instance of " + this.GetType().Name + " exists. This will be destroyed!", this.gameObject);
            Destroy(this);
        }
        else
            _Instance = this;
    }
}
