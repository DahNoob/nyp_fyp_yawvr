using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyStructure : BaseStructure
{
    [SerializeField]
    private OVR.SoundFXRef m_coinDropSound;
    public override void Die()
    {
        PlayerHandler.instance.AddCurrency(m_structureInfo.currencyValue);
        PlayerHandler.instance.AddHealth(15);
        m_coinDropSound.PlaySoundAt(transform.position);
        base.Die();
    }
}