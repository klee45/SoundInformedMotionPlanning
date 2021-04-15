using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReactsToSound : MonoBehaviour
{
    public virtual void React(SoundSource source)
    {
        Debug.Log("React!");
    }
}
