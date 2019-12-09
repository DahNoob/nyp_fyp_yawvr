using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//PLZ DONT USE DIS CLASS AS AN ACTUAL FINAL THING HHHH ME JUS LAZY TO MAKE DA "BaseProjectile"  N STUF
public class placeholderprojectile : MonoBehaviour
{
    private float selfDestructTimer = 2;
    void Start()
    {
        GetComponent<Rigidbody>().AddRelativeForce(Vector3.forward * 100);
    }

    // Update is called once per frame
    void Update()
    {
        selfDestructTimer -= Time.deltaTime;
        if (selfDestructTimer < 0)
            Destroy(gameObject);
    }
}
