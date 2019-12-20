using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuadTreeObject : MonoBehaviour
{
    //Base function
    public abstract void AddToQuadTree(GameObject referenceObject);
}
