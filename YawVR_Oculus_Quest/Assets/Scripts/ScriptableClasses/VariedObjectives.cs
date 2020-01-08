using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Varied Objectives", menuName = "Objectives Info/Varied Objectives")]
public class VariedObjectives : ScriptableObject
{
    public enum TYPE
    {
        BOUNTYHUNT,
        DEFEND_STRUCTURE,

        TOTAL
    }
    [System.Serializable]
    public struct VariedObjectiveInfo
    {
        public TYPE type;
        public int mapPointIndex;
        public string tag;
    }
    public VariedObjectiveInfo[] objectives;
}
