using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechRig : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().Play("Idle");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
