using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores functions that access the QuadTree, adding and removing.
/// </summary>
public class DynamicQuadTreeObject : MonoBehaviour
{
    private QuadTreeManager.DYNAMIC_TYPES m_type = QuadTreeManager.DYNAMIC_TYPES.TOTAL_TYPE;

    /// <summary>
    /// Gets the type of this object.
    /// </summary>
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

    /// <summary>
    /// Adds the object to the quadtree and sets it's type for future reference.
    /// </summary>
    /// <param name="referenceObject">Object to add to the tree</param>
    /// <param name="type">The type of the object</param>
    public void AddToQuadTree(GameObject referenceObject, QuadTreeManager.DYNAMIC_TYPES type)
    {
        m_type = type;
        if (QuadTreeManager.instance.AddToDynamicQuadTree(referenceObject, type))
        {
        }

    }

    /// <summary>
    /// Remove referenceObject from quadtree.
    /// </summary>
    /// <param name="referenceObject">ReferenceObject to remove</param>
    public void RemoveFromQuadTree(GameObject referenceObject)
    {
        if (m_type != QuadTreeManager.DYNAMIC_TYPES.TOTAL_TYPE)
        {
            if (QuadTreeManager.instance.RemoveDynamicObject(referenceObject, m_type))
            {
            }
        }
    }
}


