using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerUISounds
{
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
        public AudioClip m_audioClip;
    }
    //I keep using this dictionary thing cause its useful but meh whatever
    private Dictionary<int, PlayerUISoundData> m_audioClipDictionary;
    [SerializeField]
    private List<PlayerUISoundData> m_soundData;
    [SerializeField]    [Tooltip("Simple prefab with audio source")]
    private GameObject m_audioPrefab;
    [SerializeField] [Tooltip("GameObject to hold all the instantiated audio sources")]
    private Transform m_audioParent;

    [SerializeField]
    //List of audio sources for playing
    private List<AudioSource> m_audioSourceList;


    //Dictionary for sounds
    // Start is called before the first frame update
    public void Start()
    {
        m_audioClipDictionary = new Dictionary<int, PlayerUISoundData>();
        for(int i =0; i < m_soundData.Count; ++i)
        {
            //Instantiate an audio source
            AudioSource audioSource = GameObject.Instantiate(m_audioPrefab, PlayerHandler.instance.transform.position,
                Quaternion.identity, m_audioParent).GetComponent<AudioSource>();

            audioSource.gameObject.name = m_soundData[i].m_audioName + " Audio Source";
            audioSource.clip = m_soundData[i].m_audioClip;

            m_audioSourceList.Add(audioSource);
            //m_audioClipDictionary.Add((int)m_soundData[i].m_soundType, m_soundData[i]);
        }
    }

    public void PlaySound(UI_SOUNDTYPE m_soundType)
    {
        if (m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return;

        int tag = (int)m_soundType;
        AudioSource audioSource = m_audioSourceList[tag];
        if(audioSource)
            audioSource.Play();
    }

    public void PlayOnce(UI_SOUNDTYPE m_soundType)
    {
        if (m_soundType == UI_SOUNDTYPE.TOTAL_SOUNDTYPE)
            return;

        int tag = (int)m_soundType;

        AudioSource audioSource = m_audioSourceList[tag];
        if (audioSource && !audioSource.isPlaying)
            audioSource.Play();
    }

}
