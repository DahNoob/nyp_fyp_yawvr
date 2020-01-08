using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Structure Info", menuName = "Structure Info/Structure Info")]
public class StructureInfo : ScriptableObject
{
    public int health = 100;
    public GameObject destroyEffect;
}
