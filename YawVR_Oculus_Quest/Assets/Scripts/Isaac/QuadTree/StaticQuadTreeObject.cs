using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class stores functions that access the QuadTree, adding and removing.
/// </summary>
public class StaticQuadTreeObject : MonoBehaviour
{
    //Checks what type soemthing is, also checks if a type has been assigned
    private QuadTreeManager.STATIC_TYPES m_type = QuadTreeManager.STATIC_TYPES.TOTAL_TYPE;
    /// <summary>
    /// Gets the type of this object.
    /// </summary>
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

    /// <summary>
    /// Adds the object to the quadtree and sets it's type for future reference.
    /// </summary>
    /// <param name="referenceObject">Object to add to the tree</param>
    /// <param name="type">The type of the object</param>
    public void AddToQuadTree(GameObject referenceObject, QuadTreeManager.STATIC_TYPES type)
    {
        m_type = type;
        QuadTreeManager.instance.AddToStaticQuadTree(referenceObject,type);
    }
    /// <summary>
    /// Remove referenceObject from quadtree.
    /// </summary>
    /// <param name="referenceObject">ReferenceObject to remove</param>
    public void RemoveFromQuadTree(GameObject referenceObject)
    {
        if (m_type != QuadTreeManager.STATIC_TYPES.TOTAL_TYPE)
            QuadTreeManager.instance.RemoveStaticObject(referenceObject, m_type);
    }
}
