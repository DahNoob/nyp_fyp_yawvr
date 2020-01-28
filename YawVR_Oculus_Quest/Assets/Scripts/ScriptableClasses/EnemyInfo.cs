using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Info", menuName = "Entity Info/Enemy Info")]
public class EnemyInfo : EntityInfo
{
    public enum ENEMY_TYPE
    {
        LIGHT_MECH1,
        LIGHT_MECH2,
        HEAVY_MECH2,
        TOTAL_TYPE
    }


    [Header("Base Enemy Info")]
    public int damage = 10;
    public int moveSpeed = 10;
    public GameObject dmgBuff, hpBuff, msBuff;
    public ENEMY_TYPE enemyType;
}