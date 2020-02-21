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

/// <summary>
/// This class handles all logic for the player's mech movement
/// </summary>
public class MechMovement : MonoBehaviour
{
    [Header("Mech Speed Configuration")]
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

    [Header("Mech Movement Configuration")]
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float m_pitchLowerLimit = 25.0f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float m_pitchUpperLimit = 40.0f;

    private Vector3 MoveVector = Vector3.zero;
    private Vector3 Rotation;

    private Vector3 gravityVector;
    private float FallSpeed = 0.0f;

    [Header("Audio Sources")]
    [SerializeField]
    private AudioSource m_rotationStartupAudio;
    [SerializeField]
    private AudioSource m_rotationLoopAudio;
    [SerializeField]
    private AudioSource m_rotationEndAudio;
    [SerializeField]
    private AudioSource m_mechLegStep1;
    [SerializeField]
    private AudioSource m_mechLegStep2;
    [SerializeField]
    private AudioSource m_mechLandAudio;

    [Header("Visible variables")]
    public Vector3 movementDelta;
    public float movementAlpha;
    public float startWalkTime = 0;
    public float startFallTime = 0;
    public bool isWalking = false;
    public bool isRotating = false;
    public bool isFalling = false;

    [Header("Debug")]
    [SerializeField]
    Vector2 rStickDelta;
    [SerializeField]
    Vector2 lStickDelta;

    //Local variables
    private CharacterController cc;
    private bool stepAlternation = false;
    public float rotationAxisSmoothedDelta_Current, rotationAxisSmoothedDelta_Goal = 0;

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        m_pitchUpperLimit = -m_pitchUpperLimit;
        m_pitchLowerLimit = -m_pitchLowerLimit;
    }

    void Update()
    {
        Vector2 primaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.LTouch) + new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));//OVRInput.Get(OVRInput.RawAxis2D.LThumbstick); 
        Vector2 secondaryAxis = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, OVRInput.Controller.RTouch);

        lStickDelta = primaryAxis;
        rStickDelta = secondaryAxis;

        //speed = Mathf.Clamp(speed, minSpeed, maxSpeed);

        // Player Movement
        movementDelta = Vector3.zero;

        if (!CustomUtility.IsZero(primaryAxis))
        {
            speed = Mathf.Min(maxSpeed, speed + acceleration * Time.deltaTime);
            var moveDirGlobal = primaryAxis * speed;
            float rotY = Mathf.Deg2Rad * (transform.rotation.y/* - Camera.main.transform.localEulerAngles.y*/);
            var moveDirLocal = new Vector2(moveDirGlobal.x * Mathf.Cos(rotY) - moveDirGlobal.y * Mathf.Sin(rotY), moveDirGlobal.x * Mathf.Sin(rotY) + moveDirGlobal.y * Mathf.Cos(rotY));
            Vector3 derp = new Vector3(moveDirLocal.x, 0, moveDirLocal.y);
            Vector3 newPos = transform.position + derp * Time.deltaTime;
            //transform.Translate(derp * Time.deltaTime);
            derp = transform.rotation * derp;
            PlayerHandler.instance.SetLegsAngle(primaryAxis.x, primaryAxis.y);
            //cc.Move(derp * Time.deltaTime);
            movementDelta = derp;
            if (!isWalking)
            {
                startWalkTime = Time.time;
                isWalking = true;
            }
            PlayerHandler.instance.SetState(PlayerHandler.STATE.WALK);
            //rb.MovePosition(newPos);
        }
        else
        {
            speed = Mathf.Max(minSpeed, speed - deceleration * Time.deltaTime);
            PlayerHandler.instance.SetState(PlayerHandler.STATE.IDLE);
            if (isWalking)
            {
                isWalking = false;
            }
        }

        cc.SimpleMove(movementDelta);

        //Check fall
        if (isFalling && cc.isGrounded)
        {
            isFalling = false;
            if (Time.time > startFallTime)
            {
                m_mechLandAudio.Play();
                PlayerHandler.instance.Shake(0.2f);
                PlayerHandler.instance.BuzzYaw(0.4f, 40, 80, 80, 80);
                VibrationManager.SetControllerVibration(OVRInput.Controller.RTouch, 0.03f, 0.5f, false, 0.02f);
                VibrationManager.SetControllerVibration(OVRInput.Controller.LTouch, 0.03f, 0.5f, false, 0.02f);
            }
        }
        else if (!isFalling && !cc.isGrounded)
        {
            isFalling = true;
            startFallTime = Time.time + 0.5f;
        }

        // Player Rotation
        if (Input.GetKey(KeyCode.X))
            secondaryAxis.x += 1;
        if (Input.GetKey(KeyCode.Z))
            secondaryAxis.x -= 1;
        if (Input.GetKey(KeyCode.F))
            secondaryAxis.y += 1;
        if (Input.GetKey(KeyCode.V))
            secondaryAxis.y -= 1;
        rotationAxisSmoothedDelta_Goal = secondaryAxis.x;
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed * rotationAxisSmoothedDelta_Current);
        if (isRotating && rotationAxisSmoothedDelta_Goal == 0.0f)
        {
            m_rotationEndAudio.Play();
            m_rotationLoopAudio.Stop();
            m_rotationStartupAudio.Stop();
            isRotating = false;
        }
        else if (!isRotating && rotationAxisSmoothedDelta_Goal != 0.0f)
        {
            isRotating = true;
            m_rotationEndAudio.Stop();
            m_rotationStartupAudio.Play();
            m_rotationLoopAudio.Play();
        }
        if (secondaryAxis.y < 0)
            PlayerHandler.instance.goalPitch = m_pitchLowerLimit * secondaryAxis.y;
        else if (secondaryAxis.y > 0)
            PlayerHandler.instance.goalPitch = m_pitchUpperLimit * secondaryAxis.y;
        else
            PlayerHandler.instance.goalPitch = 0;

        //if(Input.GetKey(KeyCode.X))
        //{
        //    transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed * 1);
        //}
        //if (Input.GetKey(KeyCode.Z))
        //{
        //    transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed * -1);
        //}

        movementAlpha = GetMovementAlpha();

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

    private void FixedUpdate()
    {
        rotationAxisSmoothedDelta_Current = Mathf.LerpUnclamped(rotationAxisSmoothedDelta_Current, rotationAxisSmoothedDelta_Goal, 0.15f);
        if (rotationAxisSmoothedDelta_Current < 0.005f && rotationAxisSmoothedDelta_Current > -0.005f)
            rotationAxisSmoothedDelta_Current = 0;

    }

    /// <summary>
    /// Normalized value of speed and max speed clamped between 0 and 1
    /// </summary>
    /// <returns>Normalized Value between 0 and 1</returns>
    public float GetMovementAlpha()
    {
        return speed / maxSpeed;
    }

    /// <summary>
    /// Plays sound when the mech is walking
    /// </summary>
    public void PlayStepSound()
    {
        stepAlternation = !stepAlternation;
        if (stepAlternation)
            m_mechLegStep1.Play();
        else
            m_mechLegStep2.Play();
    }
}
