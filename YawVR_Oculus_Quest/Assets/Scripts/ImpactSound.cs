﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactSound : MonoBehaviour , IPooledObject
{
    public enum IMPACT_TYPE
    {
        ROCK,
        SAND,
        METAL
    }
    [SerializeField]
    private ImpactSoundInfo m_metalImpactInfo, m_sandImpactInfo, m_rockImpactInfo;

    public IMPACT_TYPE impactType = IMPACT_TYPE.SAND;

    private bool startedPlaying = false;

    public void OnObjectSpawn()
    {
        Start();
    }

    public void OnObjectDestroy()
    {
        ObjectPooler.instance.DisableInPool(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE_IMPACT);
        startedPlaying = false;
        gameObject.SetActive(false);
    }

    void Start()
    {
        AudioClip[] clips;
        if (impactType == IMPACT_TYPE.METAL)
        {
            clips = m_metalImpactInfo.m_audioClips;
        }
        else if (impactType == IMPACT_TYPE.ROCK)
        {
            clips = m_rockImpactInfo.m_audioClips;
        }
        else
        {
            clips = m_sandImpactInfo.m_audioClips;
        }
        GetComponent<AudioSource>().clip = clips[Random.Range(0, clips.Length)];
        GetComponent<AudioSource>().Play();
        startedPlaying = true;
    }
    private void Update()
    {
        if (startedPlaying && !GetComponent<ParticleSystem>().isEmitting)
        {
            OnObjectDestroy();
        }
    }
}
