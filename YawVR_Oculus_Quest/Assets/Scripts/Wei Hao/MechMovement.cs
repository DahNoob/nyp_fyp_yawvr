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
    private float speed = 8.0f;         // Current Mech Speed (Do not edit this)
    [SerializeField]
    private float minSpeed = 8.0f;      // Maximum speed mech can go
    [SerializeField]
    private float maxSpeed = 11.0f;      // Maximum speed mech can go
    [SerializeField]
    //private float maxSpeed = 5.0f;      // Maximum speed mech can go
    private float acceleration = 1.0f;  // How fast will the mech reach max speed
    [SerializeField]
    private float deceleration = 1.0f;  // How fast will the mech reach 0 speed
    [SerializeField]
    private float rotationSpeed = 30.0f;  // Speed of rotation

    private Vector3 MoveVector = Vector3.zero;
    private Vector3 Rotation;

    private float targetRotation;

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
        //rb = body.GetComponent<Rigidbody>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Thumbstick movement
        /* Primary thumbstick is left */
        /* Secondary thumbstick is right*/

        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);//OVRInput.Get(OVRInput.RawAxis2D.LThumbstick); 
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        lStickDelta = primaryAxis;
        rStickDelta = secondaryAxis;

        //Vector3 direction = new Vector3(primaryAxis.x, 0, primaryAxis.y);
        //rb.velocity = direction;
        //velocity = Camera.main.transform.TransformDirection(velocity);

        //transform.eulerAngles = new Vector3(0, centerEye.transform.localEulerAngles.y, 0);
        //transform.Translate(Vector3.forward * speed * primaryAxis.y * Time.deltaTime);
        //transform.Translate(Vector3.right * speed * primaryAxis.x * Time.deltaTime);

        //pObject.transform.position = Vector3.Lerp(pObject.transform.position, transform.position, 10.0f * Time.deltaTime);

        //speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        //transform.Translate(Vector3.forward * 0.1f * Time.fixedDeltaTime);

        if(!primaryAxis.Equals(Vector2.zero))
        {
            var moveDirGlobal = primaryAxis * maxSpeed;
            float rotY = Mathf.Deg2Rad * (transform.rotation.y);
            var moveDirLocal = new Vector2(moveDirGlobal.x * Mathf.Cos(rotY) - moveDirGlobal.y * Mathf.Sin(rotY), moveDirGlobal.x * Mathf.Sin(rotY) + moveDirGlobal.y * Mathf.Cos(rotY));
            Vector3 derp = new Vector3(moveDirLocal.x, 0, moveDirLocal.y);
            Vector3 newPos = transform.position + derp * Time.fixedDeltaTime;
            transform.Translate(derp * Time.fixedDeltaTime);
            //rb.MovePosition(newPos);
        }
        if(secondaryAxis.x != 0.0f)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed * secondaryAxis.x);
        }

        ////Move forward
        //if (primaryAxis.y > 0.0f)
        //{
        //    speed += acceleration * Time.fixedDeltaTime;
        //    if (speed >= maxSpeed)
        //    {
        //        speed = maxSpeed;
        //    }
        //    rb.AddForce(transform.forward * speed);

        //    //Vector3 move = new Vector3(0, 0, primaryAxis.y * Time.deltaTime);
        //    //move = this.transform.TransformDirection(move);
        //}

        ////Move backwards
        //else if (primaryAxis.y < 0.0f)
        //{
        //    speed += acceleration * Time.fixedDeltaTime;
        //    if (speed >= maxSpeed)
        //    {
        //        speed = maxSpeed;
        //    }
        //    rb.AddForce(-transform.forward * speed);

        //    //Vector3 move = new Vector3(0, 0, primaryAxis.y * Time.deltaTime);
        //    //move = this.transform.TransformDirection(move);
        //}
        //else
        //{
        //    speed -= deceleration * Time.deltaTime;
        //    if (speed <= minSpeed)
        //    {
        //        speed = minSpeed;
        //    }
        //}

        ////Move left
        //if (primaryAxis.x < 0.0f)
        //{
        //    speed += acceleration * Time.fixedDeltaTime;
        //    if (speed >= maxSpeed)
        //    {
        //        speed = maxSpeed;
        //    }
        //    rb.AddForce(-transform.right * speed);

        //    //Vector3 move = new Vector3(primaryAxis.x * Time.deltaTime, 0, 0);
        //    //move = this.transform.TransformDirection(move);
        //}

        ////Move right
        //else if (primaryAxis.x > 0.0f)
        //{
        //    speed += acceleration * Time.fixedDeltaTime;
        //    if (speed >= maxSpeed)
        //    {
        //        speed = maxSpeed;
        //    }
        //    rb.AddForce(transform.right * speed);

        //    //Vector3 move = new Vector3(primaryAxis.x * Time.deltaTime, 0, 0);
        //    //move = this.transform.TransformDirection(move);
        //}
        //else
        //{
        //    speed -= deceleration * Time.deltaTime;
        //    if (speed <= minSpeed)
        //    {
        //        speed = minSpeed;
        //    }
        //}

        ////Rotate Left
        //if (secondaryAxis.x < 0.0f)
        //{
        //    transform.Rotate(Vector3.down * Time.deltaTime * rotationSpeed);
        //}
        ////Rotate Right
        //if (secondaryAxis.x > 0.0f)
        //{
        //    transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
        //}
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
