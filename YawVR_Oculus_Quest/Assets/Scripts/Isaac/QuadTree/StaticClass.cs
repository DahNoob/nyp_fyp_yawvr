using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticClass :  StaticQuadTreeObject
{
    // Start is called before the first frame update
    void Start()
    {
        AddToQuadTree(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
