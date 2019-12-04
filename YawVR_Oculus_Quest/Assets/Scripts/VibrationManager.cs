using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Vibration Manager
** Desc: Simplifies executing haptics/vibrations for Touch Controllers 
** Author: DahNoob
** Date: 03/12/2019, 12:53 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    03/12/2019, 12:53PM     DahNoob   Created
** 2    03/12/2019, 1:46 PM     DahNoob   Implemented
*******************************/
[System.Serializable]
public class VibrationManager// : MonoBehaviour
{
    //public VibrationManager instance { get; } = new VibrationManager(); void Start()
    //{
    //    print("VibrationManager instanced!");
    //}

    public static void SetControllerVibration(OVRInput.Controller _controller, int _iteration, int _frequency, int _strength)
    {
        OVRHapticsClip clip = new OVRHapticsClip();

        for(int i = 0; i < _iteration; ++i)
        {
            clip.WriteSample(i % _frequency == 0 ? (byte)_strength : (byte)0);
        }

        if (_controller == OVRInput.Controller.RTouch)
            OVRHaptics.RightChannel.Mix(clip);
        else if (_controller == OVRInput.Controller.LTouch)
            OVRHaptics.LeftChannel.Preempt(clip);
    }

    public static void SetControllerVibration(OVRInput.Controller _controller, OVRHapticsClip _clip)
    {
        if (_controller == OVRInput.Controller.RTouch)
            OVRHaptics.RightChannel.Preempt(_clip);
        else if (_controller == OVRInput.Controller.LTouch)
            OVRHaptics.LeftChannel.Preempt(_clip);
    }
}
