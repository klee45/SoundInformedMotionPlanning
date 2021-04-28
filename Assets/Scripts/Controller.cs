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
    private GameObject globalLightContainer;
    [SerializeField]
    private GameObject localLightContainer;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            print("Turning off global lighting");
            StartCoroutine(TurnDownLighting());
        }

        if (Input.GetKeyDown("2"))
        {
            print("Activating robot");
            echoSource.Activate(true);
            listener.Activate(true);
        }

        if (Input.GetKeyDown("3"))
        {
            Debug.Log("Deactivating robot and turning lights back on");
            echoSource.Activate(false);
            listener.Activate(false);
            robotAnimator.SetFloat("speed", 0);
            StartCoroutine(TurnUpLighting());
        }
    }

    private IEnumerator TurnDownLighting()
    {
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
