using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestingController : MonoBehaviour
{
    [SerializeField]
    private PrimaryEchoSource echoSource;
    [SerializeField]
    private SoundListener listener;
    [SerializeField]
    private GameObject playerObject;

    private void Start()
    {
        SoundSourceManager.instance.SetActiveSource(playerObject.GetComponent<SoundSource>());

        echoSource.Activate(true);
        listener.Activate(true);
        listener.AllowMovement();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
}
