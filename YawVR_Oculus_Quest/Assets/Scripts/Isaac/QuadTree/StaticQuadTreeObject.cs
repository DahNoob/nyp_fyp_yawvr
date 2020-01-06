using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticQuadTreeObject : MonoBehaviour
{
    //Checks what type soemthing is, also checks if a type has been assigned
    private QuadTreeManager.STATIC_TYPES m_type = QuadTreeManager.STATIC_TYPES.TOTAL_TYPE;

    public QuadTreeManager.STATIC_TYPES Type
    {
        get
        {
            return m_type;
        }
        private set
        {
            m_type = value;
        }
    }

    //Adds the object to the quadtree and sets it's type for future reference.
    public void AddToQuadTree(GameObject referenceObject, QuadTreeManager.STATIC_TYPES type)
    {
        m_type = type;
        QuadTreeManager.instance.AddToStaticQuadTree(referenceObject,type);
    }

    public void RemoveFromQuadTree(GameObject referenceObject)
    {
        if (m_type != QuadTreeManager.STATIC_TYPES.TOTAL_TYPE)
            QuadTreeManager.instance.RemoveStaticObject(referenceObject, m_type);
    }
}
