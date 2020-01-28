using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicQuadTreeObject : MonoBehaviour
{
    private QuadTreeManager.DYNAMIC_TYPES m_type = QuadTreeManager.DYNAMIC_TYPES.TOTAL_TYPE;

    public QuadTreeManager.DYNAMIC_TYPES Type
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
    public void AddToQuadTree(GameObject referenceObject, QuadTreeManager.DYNAMIC_TYPES type)
    {
        m_type = type;
        if (QuadTreeManager.instance.AddToDynamicQuadTree(referenceObject, type))
        {
            Debug.Log("Added " + this.gameObject.name + "to stupid tree");

        }

    }

    public void RemoveFromQuadTree(GameObject referenceObject)
    {
        if (m_type != QuadTreeManager.DYNAMIC_TYPES.TOTAL_TYPE)
        {
            if (QuadTreeManager.instance.RemoveDynamicObject(referenceObject, m_type))
            {
                Debug.Log("Removed " + this.gameObject.name + "from stupid tree");
            }
        }
    }
}


