using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.localPosition;
        //transform.localPosition = new Vector3(pos.x + 0.5f * Time.deltaTime, pos.y, pos.z);
    }
}
