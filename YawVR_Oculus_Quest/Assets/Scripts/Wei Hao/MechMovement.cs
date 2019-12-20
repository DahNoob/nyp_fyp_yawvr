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
    private float acceleration = 1.0f;  // How fast will the mech reach max speed
    [SerializeField]
    private float deceleration = 1.5f;  // How fast will the mech reach 0 speed
    [SerializeField]
    private float rotationSpeed = 30.0f;  // Speed of rotation

    private Vector3 MoveVector = Vector3.zero;
    private Vector3 Rotation;

    private Vector3 gravityVector;
    private float FallSpeed = 0.0f;

    [Header("Debug")]
    [SerializeField]
    Vector2 rStickDelta;
    [SerializeField]
    Vector2 lStickDelta;

    private CharacterController cc;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch);//OVRInput.Get(OVRInput.RawAxis2D.LThumbstick); 
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        lStickDelta = primaryAxis;
        rStickDelta = secondaryAxis;

        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        // Player Movement
        Vector3 movementDelta = Vector3.zero;

        if (!primaryAxis.Equals(Vector2.zero))
        {
            speed += acceleration * Time.deltaTime;
            var moveDirGlobal = primaryAxis * speed;
            float rotY = Mathf.Deg2Rad * (transform.rotation.y - Camera.main.transform.localEulerAngles.y);
            var moveDirLocal = new Vector2(moveDirGlobal.x * Mathf.Cos(rotY) - moveDirGlobal.y * Mathf.Sin(rotY), moveDirGlobal.x * Mathf.Sin(rotY) + moveDirGlobal.y * Mathf.Cos(rotY));
            Vector3 derp = new Vector3(moveDirLocal.x, 0, moveDirLocal.y);
            Vector3 newPos = transform.position + derp * Time.deltaTime;
            //transform.Translate(derp * Time.deltaTime);
            derp = transform.rotation * derp;
            //cc.Move(derp * Time.deltaTime);
            movementDelta = derp;
            PlayerHandler.instance.SetState(PlayerHandler.STATE.WALK);
            //rb.MovePosition(newPos);
        }
        else
        {
            speed -= deceleration * Time.deltaTime;
            PlayerHandler.instance.SetState(PlayerHandler.STATE.IDLE);
        }

        cc.SimpleMove(movementDelta);
        if(OVRInput.Get(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            cc.Move(new Vector3(0, 5.0f * Time.deltaTime, 0));
        }

        // Player Rotation
        if (secondaryAxis.x != 0.0f)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed * secondaryAxis.x);
        }

        ////Reset the MoveVector
        //gravityVector = Vector3.zero;

        ////Check if cjharacter is grounded
        //if (cc.isGrounded == false)
        //{
        //    //Add our gravity Vecotr
        //    gravityVector += Physics.gravity;
        //}

        ////Apply our move Vector , remeber to multiply by Time.delta
        //cc.Move(gravityVector * Time.deltaTime);
    }
}
