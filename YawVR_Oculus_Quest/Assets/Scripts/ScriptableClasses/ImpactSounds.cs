using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class contains variables that would be made into a ScriptableObject later.
/// </summary>
[CreateAssetMenu(fileName = "Impact Sound Info", menuName = "Sound Info/Impact Sound Info")]
public class ImpactSoundInfo : ScriptableObject
{
    [Header("Impact Sound Info")]
    public AudioClip[] m_audioClips;
    public OVR.SoundFXRef m_sound;
}
