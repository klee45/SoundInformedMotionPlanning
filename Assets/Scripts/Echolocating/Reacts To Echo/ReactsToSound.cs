using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for echo reaction
/// 
/// Both terrain and nodes need this behavior
/// but we can't inherit to both because of other parent classes
/// </summary>
public abstract class ReactsToEcho : MonoBehaviour
{
    public virtual void React(EchoSource source)
    {
        Debug.Log("React!");
    }
}
