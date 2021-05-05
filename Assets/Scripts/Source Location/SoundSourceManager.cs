using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls which sound source is being actively
/// observed with the kalman filters
/// 
/// The current work cannot handle having multiple sound
/// sources with no way to distinguish them
/// </summary>
public class SoundSourceManager : Singleton<SoundSourceManager>
{
    [SerializeField]
    private SoundSource currentSource;

    public void SetActiveSource(SoundSource source)
    {
        if (this.currentSource != null)
        {
            Debug.Log("Replacing " + this.currentSource.name);
        }
        this.currentSource = source;
    }

    public void RemoveActiveSource()
    {
        this.currentSource = null;
    }

    public bool TryGetActiveSource(out SoundSource source)
    {
        source = this.currentSource;
        return currentSource != null;
    }
}
