using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for terrain / sound sources that turn off to avoid
/// too many raycasts
/// </summary>
public interface ICanDisable
{
    void TurnOn();
    void TurnOff();
}
