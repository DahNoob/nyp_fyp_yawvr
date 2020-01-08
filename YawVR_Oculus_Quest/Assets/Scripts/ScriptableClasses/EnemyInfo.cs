using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Info", menuName = "Entity Info/Enemy Info")]
public class EnemyInfo : EntityInfo
{
    [Header("Base Enemy Info")]
    public int damage = 10;
    public int moveSpeed = 10;
    public GameObject dmgBuff, hpBuff, msBuff;
}