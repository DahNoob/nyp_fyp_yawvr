using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainObject : StaticQuadTreeObject
{
    // Start is called before the first frame update
    void Start()
    {
        AddToQuadTree(this.gameObject);
    }

    private void OnDisable()
    {
        RemoveFromQuadTree(this.gameObject);
    }
}
