using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

[System.Serializable]
public class PlayerUISoundManager : MonoBehaviour
{   
    //Honestly liek idk what is this tbh liek if i have time i will get back to this but...
    public static PlayerUISoundManager instance;
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

    [System.Serializable]
    public class PlayerUISoundData
    {
        public string m_audioName;
        public UI_SOUNDTYPE m_soundType;
        //public AudioClip m_audioClip;
        //public AudioSource m_audioSource;
        public SoundFXRef m_audioRef;
    }
    //I keep using this dictionary thing cause its useful but meh whatever
    private Dictionary<int, PlayerUISoundData> m_audioClipDictionary;
    [SerializeField]
    private List<PlayerUISoundData> m_soundData;

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
    }

    public void PlaySound(UI_SOUNDTYPE m_soundType, float delaySecs = 0, float volume = 1)
    {
        if (m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return;

        int tag = (int)m_soundType;

        SoundFXRef audioRef = m_audioClipDictionary[tag].m_audioRef;
        audioRef.soundFX.volume = volume;
        audioRef.PlaySound(delaySecs);

    }

    public void PlaySound(UI_SOUNDTYPE m_soundType, Vector3 position, float delaySecs = 0, float volume = 1, float pitchMultiplier = 1)
    {
        if (m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return;

        int tag = (int)m_soundType;

        SoundFXRef audioRef = m_audioClipDictionary[tag].m_audioRef;
        audioRef.PlaySoundAt(position, delaySecs, volume, pitchMultiplier);
    }

    public bool StopSound(UI_SOUNDTYPE m_soundType)
    {
        if (m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return false;

        int tag = (int)m_soundType;
        SoundFXRef audioRef = m_audioClipDictionary[tag].m_audioRef;
        return audioRef.StopSound();
    }

    public void StopAllUISounds()
    {
        foreach(KeyValuePair<int, PlayerUISoundData> data in m_audioClipDictionary)
        {
            PlayerUISoundData soundsData = data.Value;

            if (soundsData.m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
                continue;

            SoundFXRef audioRef = soundsData.m_audioRef;
            audioRef.StopSound();
        }
    }
}
