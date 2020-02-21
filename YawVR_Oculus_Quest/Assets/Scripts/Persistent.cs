using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;
using YawVR;

/******************************  
** Name: Persistent Behaviour
** Desc: Makes it so objects with this behaviour will persist after scene changes, also acts as a singleton if needed
** Author: DahNoob
** Date: 27/11/2019, 5:05 PM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    27/11/2019, 5:05 PM     DahNoob   Created and implemented
*******************************/

/// <summary>
/// This class provides a DontDestroyOnLoad behaviour and contains variables accessible by all scenes in this program's lifetime.
/// </summary>
[System.Serializable]
public class Persistent : MonoBehaviour
{
    public static Persistent instance { get; private set; }

    public readonly Color COLOR_TRANSPARENT = new Color(0, 0, 0, 0);
    public GameObject GO_STATIC { get; private set; }
    public GameObject GO_DYNAMIC { get; private set; }
    [HideInInspector]
    public GameObject YAW_TRACKER;

    public GameObject PREFAB_MODULE_ICON;
    public GameObject PREFAB_BEACON;
    public GameObject PREFAB_CROWN;
    public GameObject PREFAB_SUPPLYCRATE;
    public GameObject PREFAB_SUPPLYCRATE_DROP;
    public GameObject PREFAB_PICKABLES_BEACON;
    public Sprite MINIMAP_ICON_ENEMY;
    public Sprite MINIMAP_ICON_OBJECTIVE;
    public Material MAT_WHITE;
    public Material MAT_ENEMYMECH;
    public Material MAT_PICKABLE_OUTLINE;
    public SoundFXRef SOUND_EXPLODE;
    public SoundFXRef SOUND_RELOAD_FADEOUT;
    public SoundFXRef SOUND_RELOAD_FADEIN;

    public bool isFirstTime = true;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            instance.GO_STATIC = GameObject.Find("Static");
            instance.GO_DYNAMIC = GameObject.Find("Dynamic");
            instance.YAW_TRACKER = GameObject.Find("YAWTracker");
            DestroyImmediate(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(this);
        GO_STATIC = GameObject.Find("Static");
        GO_DYNAMIC = GameObject.Find("Dynamic");
        YAW_TRACKER = GameObject.Find("YAWTracker");
        print("Persistent awake!");
    }
    void Start()
    {
        print("Persistent started!");
    }
    public void AttachYawTrackerTo(Transform _transform)
    {
        YAW_TRACKER.transform.SetParent(_transform);
    }
    public void SetYawCameraOffset(Transform _transform)
    {
        YawController.Instance().GetCameraIMUCancellation().SetCameraOffset(_transform);
    }
    public void DetachYawTracker()
    {
        YAW_TRACKER.transform.SetParent(YawController.Instance().transform);
    }
}
