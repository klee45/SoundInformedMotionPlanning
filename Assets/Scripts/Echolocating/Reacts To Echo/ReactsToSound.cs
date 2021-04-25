using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ReactsToEcho : MonoBehaviour
{
    public virtual void React(EchoSource source)
    {
        Debug.Log("React!");
    }
}
