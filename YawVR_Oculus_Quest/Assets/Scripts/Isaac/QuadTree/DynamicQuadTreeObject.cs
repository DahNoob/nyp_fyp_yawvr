using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicQuadTreeObject : QuadTreeObject
{
    public override void AddToQuadTree(GameObject referenceObject)
    {
        QuadTreeManager.instance.AddToDynamicQuadTree(referenceObject);
    }
}
