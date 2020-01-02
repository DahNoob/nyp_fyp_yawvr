using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveBuffs : MonoBehaviour
{
    private enum _Buffs
    {
        HPx2,
        DMGx2,
        MSx2
    }

    //private enum _Rarity
    //{
    //    NORMAL,
    //    DELTA,
    //    BETA,
    //    OMEGA,
    //    ALPHA
    //}

    private _Buffs buffs;

    private float healthCopy;
    private float damageCopy;
    private float moveSpeedCopy;

    // Start is called before the first frame update
    void Start()
    {
        //Random();
    }

    public void ApplyBuff(float health, float damage, float moveSpeed, EnemyBase._Rarity rarity)
    {
        healthCopy = health;
        damageCopy = damage;
        moveSpeedCopy = moveSpeed;

        Array values = Enum.GetValues(typeof(_Buffs));
        System.Random random = new System.Random();
        buffs = (_Buffs)values.GetValue(random.Next(values.Length));

        // 1 buff
        if (rarity == EnemyBase._Rarity.DELTA)
        {

        }
        // 2 buff
        else if (rarity == EnemyBase._Rarity.BETA)
        {

        }
        // 3 buff
        else if (rarity == EnemyBase._Rarity.ALPHA)
        {

        }
        // 4 buff
        else if (rarity == EnemyBase._Rarity.OMEGA)
        {

        }

        switch (buffs)
        {
            case _Buffs.HPx2:
                healthCopy *= 2;
                break;
            case _Buffs.DMGx2:
                damageCopy *= 2;
                break;
            case _Buffs.MSx2:
                moveSpeedCopy *= 2;
                break;
        }
    }

    //public int GetHealth()
    //{
    //    return healthCopy;
    //}
    //public int GetDamage()
    //{
    //    return damageCopy;
    //}
    //public int GetMoveSpeed()
    //{
    //    return moveSpeedCopy;
    //}
}
