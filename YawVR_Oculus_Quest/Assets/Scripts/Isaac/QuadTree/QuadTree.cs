using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: QuadTree
** Desc: Logic that defines the quadtree and it's behaviours..
** Author: Isaac
** Date: 19/12/2019, 11:31 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    19/12/2019, 11:31 AM     Isaac   Created
*******************************/
public class QuadTree
{
    //Bounds of the quad tree
    private QuadRect m_bounds;
    //Capacity of each node
    private int maxCapacity;

    //Quadtree regions
    private QuadTree topLeft;
    private QuadTree topRight;
    private QuadTree botLeft;
    private QuadTree botRight;

    //Has this point been divided
    bool isDivided;

    //What's inside each object
    private List<GameObject> m_objectList;

    public QuadTree(QuadRect m_bounds, int maxCapacity)
    {
        this.m_bounds = m_bounds;
        this.maxCapacity = maxCapacity;
        this.isDivided = false;
        m_objectList = new List<GameObject>();
    }

    public void Init(QuadRect m_bounds, int maxCapacity)
    {
        this.m_bounds = m_bounds;
        this.maxCapacity = maxCapacity;
        this.isDivided = false;
        m_objectList = new List<GameObject>();
    }

    // public bool Insert(TquadTreeObject)
    public bool Insert(GameObject quadTreeObject)
    {
        if (quadTreeObject == null)
            return false;

        if (!m_bounds.Contains(quadTreeObject.transform.position))
            return false;

        if (m_objectList.Count < maxCapacity)
        {
            m_objectList.Add(quadTreeObject);
            return true;
        }
        else
        {
            if (!isDivided)
                SubDivide();

            if (topRight.Insert(quadTreeObject))
                return true;
            else if (topLeft.Insert(quadTreeObject))
                return true;
            else if (botRight.Insert(quadTreeObject))
                return true;
            else if (botLeft.Insert(quadTreeObject))
                return true;
        }
        return false;
    }

    public void Clear()
    {        
        //Else if its not null, we just clear
        if (m_objectList != null)
              m_objectList.Clear();

        if(isDivided)
        {
            //Clear the rest of the stuff.
            topLeft.Clear();
            topLeft = null;
            topRight.Clear();
            topRight = null;
            botLeft.Clear();
            botLeft = null;
            botRight.Clear();
            botRight = null;
        }
    }

    void SubDivide()
    {
        float x = m_bounds.position.x; // in this case x is... x?s
        float z = m_bounds.position.z; // in this case y is z.
        float w = m_bounds.width;
        float h = m_bounds.height;

        QuadRect topRightRect = new QuadRect(
            new Vector3(x + w * 0.5f, 0, z - h * 0.5f)
            , w * 0.5f
            , h * 0.5f);

        QuadRect topLeftRect = new QuadRect(
            new Vector3(x - w * 0.5f, 0, z - h * 0.5f)
            , w * 0.5f
            , h * 0.5f);

        QuadRect botRightRect = new QuadRect(
            new Vector3(x + w * 0.5f, 0, z + h * 0.5f)
            , w * 0.5f
            , h * 0.5f);

        QuadRect botLeftRect = new QuadRect(
            new Vector3(x - w * 0.5f, 0, z + h * 0.5f)
            , w * 0.5f
            , h * 0.5f);

        topLeft = new QuadTree(topLeftRect, maxCapacity);
        topRight = new QuadTree(topRightRect, maxCapacity);
        botLeft = new QuadTree(botLeftRect, maxCapacity);
        botRight = new QuadTree(botRightRect, maxCapacity);

        //Set divided to be true
        isDivided = true;

    }
    public List<GameObject> Query(QuadRect range)
    {
        List<GameObject> objectsInRange = new List<GameObject>();
        QueryRange(range, ref objectsInRange);
        return objectsInRange;
    }

    void QueryRange(QuadRect range, ref List<GameObject> objectsInRange)
    {
        
        if (!m_bounds.Intersects(range))
            return;
        else
        {
            //Add all objects in this tree first
            foreach (GameObject quadObject in m_objectList)
            {
                if (range.Contains(quadObject.transform.position))
                {
                    objectsInRange.Add(quadObject);
                }
            }
            if (isDivided)
            {
                if(topRight != null)
                     topRight.QueryRange(range, ref objectsInRange);
                if (topLeft != null)
                    topLeft.QueryRange(range, ref objectsInRange);
                if (botRight != null)
                    botRight.QueryRange(range, ref objectsInRange);
                if (botLeft != null)
                    botLeft.QueryRange(range, ref objectsInRange);
            }
        }
    }

    public void Render(Vector3 offset = new Vector3())
    {
        //Prevent some error in the editor.
        if (this != null)
        {
            Gizmos.DrawWireCube(
            new Vector3(m_bounds.position.x + offset.x, offset.y, m_bounds.position.z + offset.z),
            new Vector3(m_bounds.width * 2, 0.01f, m_bounds.height * 2));

            if (isDivided)
            {
                if (topLeft != null)
                    topLeft.Render(offset);
                if (topRight != null)
                    topRight.Render(offset);
                if (botLeft != null)
                    botLeft.Render(offset);
                if (botRight != null)
                    botRight.Render(offset);
            }
        }
    }

    public void GetSubDivisions(ref int here)
    {
        if(isDivided)
        {
            if (topLeft != null)
            {
                here += 1;
                topLeft.GetSubDivisions(ref here);
            }

            if (botLeft != null)
            {
                here += 1;
                botLeft.GetSubDivisions(ref here);
            }
  
            if (botRight != null)
            {
                here += 1;
                botRight.GetSubDivisions(ref here);
            }

            if (topRight != null)
            {
                here += 1;
                topRight.GetSubDivisions(ref here);
            }

        }
    }
}

