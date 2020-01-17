using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeakPoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {       
    }

    // Update is called once per frame
    void Update()
    {      
    }

    void OnCollisionEnter(Collision collision)
    {
        transform.parent.GetComponentInParent<FlyingMech3>().CollisionDetected(this);
    }
}
