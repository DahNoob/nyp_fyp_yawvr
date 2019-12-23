using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: CustomUtility interface
** Desc: An interface that can house static functions to be used anywhere
** wel it technicaly is a class n not an interface but wutebvz
** Author: DahNoob
** Date: FO(RGOT
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    i forgot                DahNoob   Created
** 2    also forgot             DahNoob   Added IsObjectPrefab function
*******************************/
[System.Serializable]
public class CustomUtility
{
    public static bool IsObjectPrefab(GameObject _object)
    {
        return _object.scene.rootCount == 0;
    }
    public static bool IsHitRadius(Vector3 _pos1, Vector3 _pos2, float _radius)
    {
        return HitCheckRadius(_pos1, _pos2) <= (_radius * _radius);
    }
    public static float HitCheckRadius(Vector3 _pos1, Vector3 _pos2)
    {
        return (_pos1 - _pos2).sqrMagnitude;
    }
    public static bool IsZero(Vector2 _val, float _EPSILON = 0.005f)
    {
        return _val.x < _EPSILON && _val.x > -_EPSILON &&
               _val.y < _EPSILON && _val.y > -_EPSILON;
    }
}