using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

/// <summary>
/// This class contains audio files to be used when projectiles hit a certain type(sand, rock, metal)
/// </summary>
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

    /// <summary>
    /// Called when this sound should be "spawned"
    /// </summary>
    public void OnObjectSpawn()
    {
    }

    /// <summary>
    /// Called when this sound should be "destroyed"
    /// </summary>
    public void OnObjectDestroy()
    {
        ObjectPooler.instance.DisableInPool(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE_IMPACT);
        startedPlaying = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Start playing sounds
    /// </summary>
    public void StartImpact()
    {
        AudioClip[] clips;
        if (impactType == IMPACT_TYPE.METAL)
        {
            //clips = m_metalImpactInfo.m_audioClips;
            m_metalImpactInfo.m_sound.PlaySoundAt(transform.position);
        }
        else if (impactType == IMPACT_TYPE.ROCK)
        {
            //clips = m_rockImpactInfo.m_audioClips;
            m_rockImpactInfo.m_sound.PlaySoundAt(transform.position);
        }
        else
        {
            //clips = m_sandImpactInfo.m_audioClips;
            m_sandImpactInfo.m_sound.PlaySoundAt(transform.position);
        }
        //GetComponent<AudioSource>().clip = clips[Random.Range(0, clips.Length)];
        //GetComponent<AudioSource>().Play();
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
