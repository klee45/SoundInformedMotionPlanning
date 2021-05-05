using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controller manages the testing environment
/// and just starts things up and allows for exiting the application
/// </summary>
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
