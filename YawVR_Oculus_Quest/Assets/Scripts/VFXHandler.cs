using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that spawns VFX
/// </summary>
public class VFXHandler : MonoBehaviour , IPooledObject
{
    public float m_time = 1;

    public enum VFX_TYPE
    {
        BOOMBOOMS,
        BULLET_IMPACT_B,
        TOTAL_TYPE
    }

    [SerializeField]
    private VFX_TYPE vfx_Type;

    public void OnObjectSpawn()
    {
        Start();
    }

     public void OnObjectDestroy()
    {
        switch (vfx_Type)
        {
            case VFX_TYPE.BOOMBOOMS:
                ObjectPooler.instance.DisableInPool(PoolObject.OBJECTTYPES.ENEMY_DEATH_EFFECT);
                break;
            case VFX_TYPE.BULLET_IMPACT_B:
                ObjectPooler.instance.DisableInPool(PoolObject.OBJECTTYPES.PLAYER_PROJECTILE_BIG);
                break;
            case VFX_TYPE.TOTAL_TYPE:
                break;
            default:
                break;
        }
        this.gameObject.SetActive(false);
    }

    void Start()
    {
        if (vfx_Type == VFX_TYPE.BOOMBOOMS)
            Persistent.instance.SOUND_EXPLODE.PlaySoundAt(transform.position);
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(m_time);
        OnObjectDestroy();
    }
}
