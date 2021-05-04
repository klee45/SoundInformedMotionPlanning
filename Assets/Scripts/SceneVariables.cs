using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneVariables : Singleton<SceneVariables>
{
    [SerializeField]
    public bool DO_COLOR_WALLS = false;

    [SerializeField]
    public bool SHOW_ECHOLOCATION_MESSAGES = false;
}
