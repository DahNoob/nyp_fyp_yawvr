using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicQuadTreeObject : QuadTreeObject
{
    public override void AddToQuadTree(GameObject referenceObject)
    {
        QuadTreeManager.instance.AddToDynamicQuadTree(referenceObject);
    }

    public override void RemoveFromQuadTree(GameObject referenceObject)
    {
        QuadTreeManager.instance.Remove(referenceObject, false);
    }
}
