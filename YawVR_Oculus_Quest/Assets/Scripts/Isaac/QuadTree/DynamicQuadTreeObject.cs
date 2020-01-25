using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicQuadTreeObject : MonoBehaviour
{
    private QuadTreeManager.DYNAMIC_TYPES m_type = QuadTreeManager.DYNAMIC_TYPES.TOTAL_TYPE;
    private bool m_addedToTree = false;

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

    public bool addedToTree
    {
        get
        {
            return m_addedToTree;
        }
        private set
        {
            m_addedToTree = value;
        }

    }

    //Adds the object to the quadtree and sets it's type for future reference.
    public void AddToQuadTree(GameObject referenceObject, QuadTreeManager.DYNAMIC_TYPES type)
    {
        m_type = type;

        if (!m_addedToTree)
        {
            if (QuadTreeManager.instance.AddToDynamicQuadTree(referenceObject, type))
                m_addedToTree = true;
        }
    }

    public void RemoveFromQuadTree(GameObject referenceObject)
    {
        if (m_addedToTree)
        {
            if (m_type != QuadTreeManager.DYNAMIC_TYPES.TOTAL_TYPE)
                if (QuadTreeManager.instance.RemoveDynamicObject(referenceObject, m_type))
                    m_addedToTree = false;
        }
    }

    public void ResetObject()
    {
        m_addedToTree = false;
    }
    
}
