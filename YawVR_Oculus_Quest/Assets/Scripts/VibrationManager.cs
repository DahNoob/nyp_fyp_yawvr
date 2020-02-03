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
** 3    06/12/2019, 9:34 AM     DahNoob   Made it a singleton (though not rlly much of a use for doing that rn)
*******************************/
[System.Serializable]
public class VibrationManager : MonoBehaviour
{
    public enum PREFABS
    {
        FLAT_LIGHT,
        FLAT_MEDIUM,
        FLAT_HARD,

        ASCEND_HARD,
        DESCEND_HARD
    }

    private static VibrationManager instance;

    //Local variables
    protected IEnumerator asyncHapticPulseRight, asyncHapticPulseLeft;
    protected bool keepAliveRight = false, keepAliveLeft = false;


    void Awake()
    {
        if (instance == null)
            instance = this;
        print("VibrationManager awake!");
    }

    void Start()
    {
        
        print("VibrationManager started!");
    }

    public static void SetControllerVibration(OVRInput.Controller _controller, int _iteration, int _frequency, int _strength)
    {
        OVRHapticsClip clip = new OVRHapticsClip();

        for(int i = 0; i < _iteration; ++i)
        {
            clip.WriteSample(i % _frequency == 0 ? (byte)_strength : (byte)0);
        }

        if (_controller == OVRInput.Controller.RTouch)
            OVRHaptics.RightChannel.Preempt(clip);
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

    public static void SetControllerVibration(OVRInput.Controller _controller, float totalDuration, float intensity, bool isContinuous = false, float pulseDuration = 0.01f, float intervalDuration = 0.01f)
    {
        instance.StartCoroutine(instance.HapticPulse( _controller,totalDuration, intensity, isContinuous, pulseDuration, intervalDuration));
    }

    //Basic haptic pulse function
    protected IEnumerator HapticPulse(OVRInput.Controller _controller, float totalDuration, float intensity, bool isContinuous = false, float pulseDuration = 0.01f, float intervalDuration = 0.01f)
    {
        if(_controller == OVRInput.Controller.RTouch && !keepAliveRight)
        {
            keepAliveRight = isContinuous;
            while (totalDuration > 0 || keepAliveRight)
            {
                OVRInput.SetControllerVibration(.1f, intensity, _controller);
                yield return new WaitForSeconds(pulseDuration);
                OVRInput.SetControllerVibration(0, 0, _controller);
                yield return new WaitForSeconds(intervalDuration);

                if (!keepAliveRight) totalDuration -= pulseDuration + intervalDuration;
            }
            asyncHapticPulseRight = null;

        }
        else if(_controller == OVRInput.Controller.LTouch && !keepAliveLeft)
        {
            keepAliveLeft = isContinuous;
            while (totalDuration > 0 || keepAliveLeft)
            {
                OVRInput.SetControllerVibration(.1f, intensity, _controller);
                yield return new WaitForSeconds(pulseDuration);
                OVRInput.SetControllerVibration(0, 0, _controller);
                yield return new WaitForSeconds(intervalDuration);

                if (!keepAliveLeft) totalDuration -= pulseDuration + intervalDuration;
            }
            asyncHapticPulseLeft = null;
        }
    }

    public static void StopHapticPulse(OVRInput.Controller _controller)
    {
        if (_controller == OVRInput.Controller.RTouch)
            instance.keepAliveRight = false;
        else if (_controller == OVRInput.Controller.LTouch)
            instance.keepAliveLeft = false;
    }

    //public static void SetControllerVibration(OVRInput.Controller _controller, )
}
