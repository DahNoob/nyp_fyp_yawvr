﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

[System.Serializable]
public class PlayerUISoundManager : MonoBehaviour
{   
    //Honestly liek idk what is this tbh liek if i have time i will get back to this but...
    /// <summary>
    /// Instance for the soundManager
    /// </summary>
    public static PlayerUISoundManager instance;

    /// <summary>
    /// Enum for UI sound types.
    /// </summary>
    public enum UI_SOUNDTYPE
    {
        OBJECTIVE_SUCCESS,
        OBJECTIVE_FAILED,
        OBJECTIVE_TRIGGER,
        DEATH_SOUND,
        HIT_MARKER,
        PROXIMITY,
        TIME_RUNNING_OUT,
        WAYPOINT_PING,
        LOW_HEALTH,
        TOTAL_SOUNDTYPE
    }

    /// <summary>
    /// Enum for tutorial sound types.
    /// </summary>
    public enum TUTORIAL_SOUNDTYPE
    {
        WELCOME_TUTORIAL,
        LOOKAROUNDPART1,
        LOOKAROUNDPART2,
        MOVING,
        READYING,
        SHOOTING,
        RELOADING,
        RESET_POSE,
        SWAP_WEAPONS,
        COMPLIMENT,
        MOVETOPOINT,
        TOTAL_SOUNDTYPE
    }

    /// <summary>
    /// This class assigns a certain sound to a type that would be defined in the inspector.
    /// </summary>
    [System.Serializable]
    public class PlayerUISoundData
    {
        public string m_audioName;
        public UI_SOUNDTYPE m_soundType;
        //public AudioClip m_audioClip;
        //public AudioSource m_audioSource;
        public SoundFXRef m_audioRef;
    }

    /// <summary>
    /// This class assigns a certain sound to a type that would be defined in the inspector.
    /// </summary>
    [System.Serializable]
    public class PlayerTutorialSoundData
    {
        public string m_audioName;
        public TUTORIAL_SOUNDTYPE m_soundType;
        //public AudioClip m_audioClip;
        //public AudioSource m_audioSource;
        public SoundFXRef m_audioRef;
    }
    //I keep using this dictionary thing cause its useful but meh whatever
    private Dictionary<int, PlayerUISoundData> m_audioClipDictionary;
    [SerializeField]
    private List<PlayerUISoundData> m_soundData;

    //I keep using this dictionary thing cause its useful but meh whatever
    private Dictionary<int, PlayerTutorialSoundData> m_tutorialClipDictionary;
    [SerializeField]
    private List<PlayerTutorialSoundData> m_tutorialSoundData;

    [SerializeField]
    private SoundFXRef m_hoverUiSound;
    [SerializeField]
    private SoundFXRef m_clickUiSound;
    [SerializeField]
    private SoundFXRef m_allObjectivesClearedSound;

    //Dictionary for sounds
    // Start is called before the first frame update
    public void Awake()
    {
        if (instance == null)
            instance = this;

        m_audioClipDictionary = new Dictionary<int, PlayerUISoundData>();
        for(int i =0; i < m_soundData.Count; ++i)
        {
            m_audioClipDictionary.Add((int)m_soundData[i].m_soundType, m_soundData[i]);
        }

        m_tutorialClipDictionary = new Dictionary<int, PlayerTutorialSoundData>();
        for (int i = 0; i < m_tutorialSoundData.Count; ++i)
        {
            m_tutorialClipDictionary.Add((int)m_tutorialSoundData[i].m_soundType, m_tutorialSoundData[i]);
        }
    }

    /// <summary>
    /// Plays a sound.
    /// </summary>
    /// <param name="m_soundType">Type of sound to play</param>
    /// <param name="delaySecs">Amount of delay before sound plays</param>
    /// <param name="volume">Volume to play the sound</param>
    public void PlaySound(UI_SOUNDTYPE m_soundType, float delaySecs = 0, float volume = 1)
    {
        if (m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return;

        int tag = (int)m_soundType;

        SoundFXRef audioRef = m_audioClipDictionary[tag].m_audioRef;
        audioRef.soundFX.volume = volume;
        audioRef.PlaySound(delaySecs);
        audioRef.AttachToParent(Camera.main.transform);
    }

    /// <summary>
    /// Plays a sound.
    /// </summary>
    /// <param name="m_soundType">Type of sound to play</param>
    /// <param name="delaySecs">Amount of delay before sound plays</param>
    /// <param name="volume">Volume to play the sound</param>
    public void PlaySound(TUTORIAL_SOUNDTYPE m_soundType, float delaySecs = 0, float volume = 1)
    {
        if (m_soundType == TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return;

        int tag = (int)m_soundType;

        SoundFXRef audioRef = m_tutorialClipDictionary[tag].m_audioRef;
        audioRef.soundFX.volume = volume;
        audioRef.PlaySound(delaySecs);
        audioRef.AttachToParent(Camera.main.transform);

    }
    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="m_soundType">Type of sound to play</param>
    /// <param name="position">Position to play sound</param>
    /// <param name="delaySecs">Amount of delay before sound plays</param>
    /// <param name="volume">Volume to play the sound</param>
    /// <param name="pitchMultiplier">Pitch multiplier</param>
    public void PlaySoundAt(UI_SOUNDTYPE m_soundType, Vector3 position, float delaySecs = 0, float volume = 1, float pitchMultiplier = 1)
    {
        if (m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return;

        int tag = (int)m_soundType;

        SoundFXRef audioRef = m_audioClipDictionary[tag].m_audioRef;
        audioRef.PlaySoundAt(position, delaySecs, volume, pitchMultiplier);
        audioRef.AttachToParent(Camera.main.transform);
    }
    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="m_soundType">Type of sound to play</param>
    /// <param name="position">Position to play sound</param>
    /// <param name="delaySecs">Amount of delay before sound plays</param>
    /// <param name="volume">Volume to play the sound</param>
    /// <param name="pitchMultiplier">Pitch multiplier</param>
    public void PlaySoundAt(TUTORIAL_SOUNDTYPE m_soundType, Vector3 position, float delaySecs = 0, float volume = 1, float pitchMultiplier = 1)
    {
        if (m_soundType == TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return;

        int tag = (int)m_soundType;

        SoundFXRef audioRef = m_tutorialClipDictionary[tag].m_audioRef;
        audioRef.PlaySoundAt(position, delaySecs, volume, pitchMultiplier);
        audioRef.AttachToParent(Camera.main.transform);
    }

    /// <summary>
    /// Stop sound.
    /// </summary>
    /// <param name="m_soundType">Type to stop</param>
    /// <returns>True if sound is stopped, false if soundType is invalid.</returns>
    public bool StopSound(UI_SOUNDTYPE m_soundType)
    {
        if (m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return false;

        int tag = (int)m_soundType;
        SoundFXRef audioRef = m_audioClipDictionary[tag].m_audioRef;
        return audioRef.StopSound();
    }
    /// <summary>
    /// Stop sound.
    /// </summary>
    /// <param name="m_soundType">Type to stop</param>
    /// <returns>True if sound is stopped, false if soundType is invalid.</returns>
    public bool StopSound(TUTORIAL_SOUNDTYPE m_soundType)
    {
        if (m_soundType == TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return false;
        
        int tag = (int)m_soundType;
        SoundFXRef audioRef = m_tutorialClipDictionary[tag].m_audioRef;
        audioRef.soundFX.volume = 0;
        return audioRef.StopSound();
    }
    /// <summary>
    /// Stops all UI sounds
    /// </summary>
    public void StopAllUISounds()
    {
        foreach(KeyValuePair<int, PlayerUISoundData> data in m_audioClipDictionary)
        {
            PlayerUISoundData soundsData = data.Value;

            if (soundsData.m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
                continue;

            SoundFXRef audioRef = soundsData.m_audioRef;
            audioRef.soundFX.volume = 0;
            audioRef.StopSound();
        }
    }

    /// <summary>
    /// Stops all tutorial sounds.
    /// </summary>
    public void StopAllTutorialSounds()
    {
        foreach (KeyValuePair<int, PlayerTutorialSoundData> data in m_tutorialClipDictionary)
        {
            PlayerTutorialSoundData soundsData = data.Value;

            if (soundsData.m_soundType == TUTORIAL_SOUNDTYPE.TOTAL_SOUNDTYPE)
                continue;

            SoundFXRef audioRef = soundsData.m_audioRef;
            audioRef.soundFX.volume = 0;
            audioRef.StopSound();
        }
    }

    /// <summary>
    /// Stops all sounds playing by this manager.
    /// </summary>
    public void StopAllSounds()
    {
        StopAllUISounds();
        StopAllTutorialSounds();
    }

    /// <summary>
    /// Play UIClickSound
    /// </summary>
    public void PlayUiClickSound()
    {
        m_clickUiSound.PlaySoundAt(transform.position);
    }

    /// <summary>
    /// Player UIHoverSound
    /// </summary>
    public void PlayUiHoverSound()
    {
        m_hoverUiSound.PlaySoundAt(transform.position);
    }

    /// <summary>
    /// Play all ObjectivesClearedSound.
    /// </summary>
    public void PlayAllObjectivesClearedSound()
    {
        m_allObjectivesClearedSound.PlaySound();
        m_allObjectivesClearedSound.AttachToParent(Camera.main.transform);
    }
}

/// <summary>
/// This class contains properties that will be passed into the SoundManager for sounds.
/// </summary>
[System.Serializable]
public class SoundData
{
    public float delaySecs = 0;
    public float volume = 1;
    public float pitchMultiplier = 1;
}