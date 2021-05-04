using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private Rigidbody rigidBody;
    private Animator animator;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;
        //transform.localPosition = new Vector3(pos.x + 0.5f * Time.deltaTime, pos.y, pos.z);

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 tempVect = new Vector3(h, 0, v);
        
        animator.SetFloat("speed", tempVect.magnitude);

        tempVect = tempVect.normalized * speed * Time.deltaTime;
        tempVect = Quaternion.AngleAxis(transform.localEulerAngles.y, Vector3.up) * tempVect;

        rigidBody.MovePosition(rigidBody.transform.position + tempVect);
    }
}
