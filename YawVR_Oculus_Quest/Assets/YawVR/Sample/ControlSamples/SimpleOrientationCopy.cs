using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YawVR;
/// <summary>
/// Sets the YawTracker's orientation based on the GameObject's orientation
/// </summary>
public class SimpleOrientationCopy : MonoBehaviour
{
    /*
       This script simply copies this gameObject's rotation, and sends it to the YawTracker
    */
    YawController yawController; // reference to YawController

    private void Start() {
        yawController = YawController.Instance();
    }
    private void FixedUpdate() {
        yawController.TrackerObject.SetRotation(transform.localEulerAngles);
    }
}
