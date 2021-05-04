using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    [SerializeField]
    private PrimaryEchoSource echoSource;
    [SerializeField]
    private SoundListener listener;
    [SerializeField]
    private Animator robotAnimator;

    [SerializeField]
    private GameObject testingSoundSource;
    [SerializeField]
    private GameObject audioListener;

    [SerializeField]
    private GameObject playerObject;

    [SerializeField]
    private Camera firstCamera;

    [SerializeField]
    private GameObject electricFence;

    [SerializeField]
    private GameObject globalLightContainer;
    [SerializeField]
    private GameObject localLightContainer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            SoundSourceManager.instance.SetActiveSource(testingSoundSource.GetComponent<SoundSource>());
            echoSource.Emit();
        }

        if (Input.GetKeyDown("2"))
        {
            listener.Activate(true);
            firstCamera.orthographicSize += 2;
        }

        if (Input.GetKeyDown("3"))
        {
            listener.Activate(false);
            audioListener.transform.parent = playerObject.transform;
            audioListener.transform.localPosition = Vector3.zero;
            firstCamera.enabled = false;
        }

        if (Input.GetKeyDown("4"))
        {
            SoundSourceManager.instance.RemoveActiveSource();
            SoundSourceManager.instance.SetActiveSource(playerObject.GetComponent<SoundSource>());
            print("Turning off global lighting");
            StartCoroutine(TurnDownLighting());
            electricFence.SetActive(false);
            echoSource.Activate(true);
            listener.Activate(true);
            listener.AllowMovement();
            print("Activating robot");
        }

        if (Input.GetKeyDown("5"))
        {
            Debug.Log("Deactivating robot and turning lights back on");
            echoSource.Activate(false);
            listener.Activate(false);
            robotAnimator.SetFloat("speed", 0);
            StartCoroutine(TurnUpLighting());
            electricFence.SetActive(true);
        }
    }

    private IEnumerator TurnDownLighting()
    {
        yield return new WaitForSeconds(1f);
        globalLightContainer.SetActive(false);
        localLightContainer.SetActive(false);
        yield return new WaitForSeconds(2f);

        localLightContainer.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        localLightContainer.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        localLightContainer.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        localLightContainer.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        localLightContainer.SetActive(true);

        yield return null;
    }

    private IEnumerator TurnUpLighting()
    {
        localLightContainer.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        localLightContainer.SetActive(true);
        yield return new WaitForSeconds(0.1f);

        localLightContainer.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        localLightContainer.SetActive(true);
        globalLightContainer.SetActive(true);
        yield return null;
    }
}
