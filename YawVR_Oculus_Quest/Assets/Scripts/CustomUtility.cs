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
}