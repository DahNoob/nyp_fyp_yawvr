using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Mech Movement Script
** Desc: Allows player to move using left thumbstick and rotate with right thumbstick
** Author: Wei Hao
** Date: 2/12/2019, 9:58 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     Wei Hao   Created and implemented
*******************************/
public class MechMovement : MonoBehaviour
{
    [Header("Mech Speed")]
    [SerializeField]
    private float speed = 5.0f;         // Current Mech Speed (Do not edit this)
    [SerializeField]
    private float minSpeed = 5.0f;      // Maximum speed mech can go
    [SerializeField]
    private float maxSpeed = 10.0f;      // Maximum speed mech can go
    [SerializeField]
    //private float maxSpeed = 5.0f;      // Maximum speed mech can go
    private float acceleration = 1.0f;  // How fast will the mech reach max speed
    [SerializeField]
    private float deceleration = 1.0f;  // How fast will the mech reach 0 speed
    [SerializeField]
    private float rotationSpeed = 50.0f;  // Speed of rotation

    private Vector3 MoveVector = Vector3.zero;
    private Vector3 Rotation;

    [Header("Debug")]
    [SerializeField]
    Vector2 rStickDelta;
    [SerializeField]
    Vector2 lStickDelta;

    private Rigidbody rb;
    public GameObject body;


    // Start is called before the first frame update
    void Start()
    {
        rb = body.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //Thumbstick movement
        /* Primary thumbstick is left */
        /* Secondary thumbstick is right*/
        Vector2 primaryAxis = OVRInput.Get(OVRInput.RawAxis2D.LThumbstick);       
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.RawAxis2D.RThumbstick);

        lStickDelta = primaryAxis;
        rStickDelta = secondaryAxis;

        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        //Move forward
        if (primaryAxis.y > 0.0f)
        {
            speed += acceleration * Time.fixedDeltaTime;
            rb.AddForce(transform.forward * speed);
        }
        //Move backwards
        else if (primaryAxis.y < 0.0f)
        {
            speed += acceleration * Time.fixedDeltaTime;
            rb.AddForce(-transform.forward * speed);
        }
        else
        {
            speed -= deceleration * Time.deltaTime;
        }

        //Move left
        if (primaryAxis.x < 0.0f)
        {
            speed += acceleration * Time.fixedDeltaTime;
            rb.AddForce(-transform.right * speed);
        }
        //Move right
        else if (primaryAxis.x > 0.0f)
        {
            speed += acceleration * Time.fixedDeltaTime;
            rb.AddForce(transform.right * speed);
        }
        else
        {
            speed -= deceleration * Time.deltaTime;
        }

        //Rotate Left
        if (secondaryAxis.x < 0.0f)
        {
            transform.Rotate(Vector3.down * Time.deltaTime * rotationSpeed);
        }
        //Rotate Right
        if (secondaryAxis.x > 0.0f)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
        }
        //else
        //{
        //    speed -= deceleration * Time.deltaTime;
        //    if (speed <= 10)
        //    {
        //        speed = 10;
        //    }
        //}
        // Compute this for key movement
        //float moveInfluence = acceleration * 0.1f * MoveScale * MoveScaleMultiplier;
    }
}
