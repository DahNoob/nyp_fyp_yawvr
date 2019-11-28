using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVRTouchSample;

/******************************  
** Name: Pilot Controller Behaviour
** Desc: Detect player hands in control holes
** Author: DahNoob
** Date: 27/11/2019, 5:10 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     DahNoob   Created and implemented
*******************************/
public class PilotController : MonoBehaviour
{
    [SerializeField]
    //The corresponding controller
    private OVRInput.Controller m_controller;

    //Test variables
    private Vector3 origScale;
    private bool yeet = false;

    private void Start()
    {
        origScale = transform.localScale;
    }
    private void Update()
    {
        if (yeet)
            transform.localScale = origScale + new Vector3(0.03f, 0.03f, 0.03f) * Mathf.Cos(Time.time);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hand hand = collision.gameObject.GetComponent<Hand>();
        if (hand)
            yeet = true;
    }
    private void OnCollisionExit(Collision collision)
    {
        yeet = false;
        transform.localScale = origScale;
    }
    private void OnTriggerEnter(Collider other)
    {
        Hand hand = other.gameObject.GetComponent<Hand>();
        if (hand)
            yeet = true;
    }
    private void OnTriggerExit(Collider other)
    {
        yeet = false;
        transform.localScale = origScale;
    }
}
