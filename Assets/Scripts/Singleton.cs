using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            Debug.LogWarning("More than one sprite manager! It's a singleton");
            Destroy(instance);
        }
        instance = this as T;
        Debug.Log("Set instance of " + instance.ToString());
    }
}