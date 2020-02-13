using UnityEngine;

/******************************  
** Name: QuadTree Rect
** Desc: Defines a bounding box for the quadtree
** Author: Isaac
** Date: 19/12/2019, 11:31 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    19/12/2019, 11:31 AM     Isaac   Created
*******************************/

/// <summary>
/// This class defines a bounding box in world space to be used for queries in the QuadTree.
/// </summary>
[System.Serializable]
public class QuadRect
{
    [SerializeField]
    private Vector3 m_position;
    [SerializeField]
    private float m_width;
    [SerializeField]
    private float m_height;

    public Vector3 position
    {
        get
        {
            return m_position;
        }
        set
        {
            m_position = value;
        }
    }

    public float width
    {
        get { return m_width; }
        set { m_width = value; }
    }
    public float height
    {
        get { return m_height; }
        set { m_height = value; }
    }

    /// <summary>
    /// Constructor
    /// </summary>
    public QuadRect()
    {

    }

    /// <summary>
    /// Overloaded constructor.
    /// </summary>
    /// <param name="point">Position of this bounding box</param>
    /// <param name="width">Width of box</param>
    /// <param name="height">Height of box</param>
    public QuadRect(Vector3 point, float width, float height)
    {
        this.m_position = point;
        this.m_width = width;
        this.m_height = height;
    }

    /// <summary>
    /// Checks if this box contains a point in world space.
    /// </summary>
    /// <param name="point">Point to check</param>
    /// <returns>True if contains, false if not.</returns>
    public bool Contains(Vector3 point)
    {
        return (point.x >= m_position.x - m_width
            && point.x < m_position.x + m_width
            && point.z >= m_position.z - m_height
            && point.z < m_position.z + m_height);
    }

    /// <summary>
    /// Checks if this bounding box intersects another bounding box.
    /// </summary>
    /// <param name="other">Other bounding box</param>
    /// <returns>True if intersects, false if not.</returns>
    public bool Intersects(QuadRect other)
    {
        return !(other.m_position.x - other.m_width > m_position.x + m_width
            || other.m_position.x + other.m_width < m_position.x - m_width
            || other.m_position.z - other.m_height > m_position.z + m_height
            || other.m_position.z + other.m_height < m_position.z - m_height);
    }
    /// <summary>
    /// Gets the formatted vector of width and height, ignoring the y value.
    /// </summary>
    /// <returns>Formatted Vector3.</returns>
    public Vector3 GetWidth()
    {
        return new Vector3(width, 0, height);
    }

}
