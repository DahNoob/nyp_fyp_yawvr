using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticQuadTreeObject : QuadTreeObject
{
    public override void AddToQuadTree(GameObject referenceObject)
    {
        QuadTreeManager.instance.AddToStaticQuadTree(referenceObject);
    }
}
