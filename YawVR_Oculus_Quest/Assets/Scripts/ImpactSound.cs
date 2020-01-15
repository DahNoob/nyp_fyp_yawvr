using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSound : MonoBehaviour , IPooledObject
{
    public AudioClip[] m_audioClips;
    private bool startedPlaying = false;

    public void OnObjectSpawn()
    {
        GetComponent<AudioSource>().clip = m_audioClips[Random.Range(0, m_audioClips.Length - 1)];
        GetComponent<AudioSource>().Play();
    }

    public void OnObjectDestroy()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        GetComponent<AudioSource>().clip = m_audioClips[Random.Range(0, m_audioClips.Length - 1)];
        GetComponent<AudioSource>().Play();
        startedPlaying = true;
    }
    private void Update()
    {
        if (startedPlaying && !GetComponent<AudioSource>().isPlaying)
        {
            OnObjectDestroy();
        }
    }
}
