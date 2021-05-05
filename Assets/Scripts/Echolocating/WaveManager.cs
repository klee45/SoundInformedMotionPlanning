using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple class to have a place to instantiate echo waves from
/// </summary>
public class WaveManager : Singleton<WaveManager>
{
    [SerializeField]
    private EchoWave waveObject;

    /*
    public static WaveManager instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Debug.LogWarning("Can be only one wave manager");
            Destroy(instance);
        }
        instance = this;
    }
    */

    public EchoWave CreateWave()
    {
        return Instantiate<EchoWave>(waveObject);
    }
}