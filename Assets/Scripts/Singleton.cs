using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton base class
/// There are a lot of singletons in this project
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T instance;
    [SerializeField]
    public bool isPresistant = true;

    protected virtual void Awake()
    {
        if (isPresistant)
        {
            DontDestroyOnLoad(this);
        }
        if (instance != null)
        {
            Debug.LogWarning("More than one " + name + "! It's a singleton");
            Destroy(instance);
        }
        instance = this as T;
        Debug.Log("Set instance of " + instance.ToString());
    }
}