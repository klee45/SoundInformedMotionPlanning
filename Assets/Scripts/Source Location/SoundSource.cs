using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player or dummy sound source
/// that the EKF is locating
/// </summary>
public class SoundSource : MonoBehaviour
{
    private bool isActive = true;

    public bool GetStatus()
    {
        return isActive;
    }


}
