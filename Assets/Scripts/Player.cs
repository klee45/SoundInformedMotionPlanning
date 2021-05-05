using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controller class for the player / tracking target
/// With auto-move on the player moves erratically without input
/// </summary>
public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private bool autoMove = true;

    [SerializeField]
    private Timer automoveTimer;

    private Rigidbody rigidBody;
    private Animator animator;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        if (autoMove)
        {
            GetComponentInChildren<CameraControl>().enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;
        //transform.localPosition = new Vector3(pos.x + 0.5f * Time.deltaTime, pos.y, pos.z);

        Vector3 tempVect = Vector3.zero;
        if (autoMove)
        {
            tempVect = new Vector3(0, 0, 1);
            if (automoveTimer.Tick(Time.deltaTime, out float currentTime))
            {
                transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
            }
        }
        else
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            tempVect = new Vector3(h, 0, v);

        }
        animator.SetFloat("speed", tempVect.magnitude);

        tempVect = tempVect.normalized * speed * Time.deltaTime;
        tempVect = Quaternion.AngleAxis(transform.localEulerAngles.y, Vector3.up) * tempVect;

        rigidBody.MovePosition(rigidBody.transform.position + tempVect);
    }
}
