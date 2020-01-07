using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Varied Objectives", menuName = "Objectives Info/Varied Objectives")]
public class VariedObjectives : ScriptableObject
{
    public enum TYPE
    {
        BOUNTYHUNT,

        TOTAL
    }
    [System.Serializable]
    public struct VariedObjectiveInfo
    {
        public TYPE type;
        public int mapPointIndex;
        [Range(0, 10)]
        public int intensity;
    }
    public VariedObjectiveInfo[] objectives;
}
