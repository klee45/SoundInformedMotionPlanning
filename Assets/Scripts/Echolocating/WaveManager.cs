using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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