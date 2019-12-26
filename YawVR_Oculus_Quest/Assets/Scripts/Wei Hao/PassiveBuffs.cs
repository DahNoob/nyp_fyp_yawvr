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

    private enum _Rarity
    {
        NORMAL,
        DELTA,
        BETA,
        OMEGA,
        ALPHA
    }

    private _Buffs buffs;

    private int healthCopy;
    private int damageCopy;
    private int moveSpeedCopy;

    // Start is called before the first frame update
    void Start()
    {
        //Random();
    }

    public void ApplyBuff(int health, int damage, int moveSpeed/*, EnemyBase._Rarity rarity*/)
    {
        healthCopy = health;
        damageCopy = damage;
        moveSpeedCopy = moveSpeed;

        Array values = Enum.GetValues(typeof(_Buffs));
        System.Random random = new System.Random();
        buffs = (_Buffs)values.GetValue(random.Next(values.Length));

        //if(rarity == EnemyBase._Rarity.DELTA)
        //{

        //}

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

    public int GetHealth()
    {
        return healthCopy;
    }
    public int GetDamage()
    {
        return damageCopy;
    }
    public int GetMoveSpeed()
    {
        return moveSpeedCopy;
    }
}
