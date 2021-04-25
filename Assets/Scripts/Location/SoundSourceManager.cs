using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSourceManager : Singleton<SoundSourceManager>
{
    private SoundSource currentSource;

    public void SetActiveSource(SoundSource source)
    {
        if (this.currentSource != null)
        {
            Debug.LogWarning("Replacing " + this.currentSource.name);
        }
        this.currentSource = source;
    }

    public void RemoveActiveSource()
    {
        this.currentSource = null;
    }
}
