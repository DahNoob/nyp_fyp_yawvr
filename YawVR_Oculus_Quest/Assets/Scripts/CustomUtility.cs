using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomUtility
{
    public static bool IsObjectPrefab(GameObject _object)
    {
        return _object.scene.rootCount == 0;
    }
}