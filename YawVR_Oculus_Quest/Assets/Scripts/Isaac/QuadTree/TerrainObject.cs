using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainObject : StaticQuadTreeObject
{
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Adding " + gameObject.name + " to static tree");
        AddToQuadTree(this.gameObject,QuadTreeManager.STATIC_TYPES.TERRAIN);
    }

    private void OnDisable()
    {
        RemoveFromQuadTree(this.gameObject);
    }
}
