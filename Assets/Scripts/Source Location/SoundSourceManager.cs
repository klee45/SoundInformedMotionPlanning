using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
