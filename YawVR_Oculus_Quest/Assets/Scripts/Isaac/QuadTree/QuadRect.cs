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

    public QuadRect()
    {

    }

    public QuadRect(Vector3 point, float width, float height)
    {
        this.m_position = point;
        this.m_width = width;
        this.m_height = height;
    }

    public bool Contains(Vector3 point)
    {
        return (point.x >= m_position.x - m_width
            && point.x < m_position.x + m_width
            && point.z >= m_position.z - m_height
            && point.z < m_position.z + m_height);
    }

    public bool Intersects(QuadRect other)
    {
        return !(other.m_position.x - other.m_width > m_position.x + m_width
            || other.m_position.x + other.m_width < m_position.x - m_width
            || other.m_position.z - other.m_height > m_position.z + m_height
            || other.m_position.z + other.m_height < m_position.z - m_height);
    }

    public Vector3 GetWidth()
    {
        return new Vector3(width, 0, height);
    }

}
