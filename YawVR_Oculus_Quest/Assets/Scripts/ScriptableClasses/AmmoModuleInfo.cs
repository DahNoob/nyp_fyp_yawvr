using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Info", menuName = "Ammo Info/Ammo Info")]
public class AmmoModuleInfo : ScriptableObject
{
    [Header("Ammo Info")]
    public int maxAmmo = 100;
    public int maxClip = 0;
    public float reloadTime = 1.0f;
    [Header("Ammo Behaviour")]
    public bool usesClips = false;
    public bool usesAmmo = true;
}
