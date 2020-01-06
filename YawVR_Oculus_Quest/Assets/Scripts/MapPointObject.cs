using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPointObject : StaticQuadTreeObject
{
    // Start is called before the first frame update
    void Start()
    {
        AddToQuadTree(this.gameObject, QuadTreeManager.STATIC_TYPES.MAP_POINTS);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDisable()
    {
        RemoveFromQuadTree(this.gameObject);
    }
}
