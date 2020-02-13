﻿using System.Collections;
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

/// <summary>
/// This class contains all properties and functions needed for a QuadTree to function
/// </summary>
public class QuadTree
{
    //Should be 4.
    static int QUADTREE_COUNT = 4;
    //Bounds of the quad tree
    private QuadRect m_bounds;
    //Capacity of each node
    private int maxCapacity;
    //Quadtree regions
    private QuadTree[] regions;
    //Has this point been divided
    bool isDivided;
    //What's inside each object
    private List<GameObject> m_objectList;

    /// <summary>
    /// Constructor for this QuadTree
    /// </summary>
    /// <param name="m_bounds"></param>
    /// <param name="maxCapacity">Max amount of capacity this quadtree can hold before it splits into lower divisions</param>
    public QuadTree(QuadRect m_bounds, int maxCapacity)
    {
        this.m_bounds = m_bounds;
        this.maxCapacity = maxCapacity;
        this.isDivided = false;
        m_objectList = new List<GameObject>();
        regions = new QuadTree[QUADTREE_COUNT];
    }

    /// <summary>
    /// Initializes this quadtree
    /// </summary>
    /// <param name="m_bounds"></param>
    /// <param name="maxCapacity">Max amount of capacity this quadtree can hold before it splits into lower divisions</param>
    public void Init(QuadRect m_bounds, int maxCapacity)
    {
        this.m_bounds = m_bounds;
        this.maxCapacity = maxCapacity;
        this.isDivided = false;
        m_objectList = new List<GameObject>();
        regions = new QuadTree[QUADTREE_COUNT];
    }

    /// <summary>
    /// Inserts an object into the QuadTree.
    /// </summary>
    /// <param name="quadTreeObject">Object to insert to this quadTree</param>
    /// <returns></returns>
    public bool Insert(GameObject quadTreeObject)
    {
        //Do not insert if the object is null
        if (quadTreeObject == null)
            return false;

        //If the bounds does not contain, don't bother searching further.
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

            if (regions[0] != null)
            {
                for (int i = 0; i < QUADTREE_COUNT; i++)
                {
                    if (regions[i].Insert(quadTreeObject))
                        return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Removes an object from the quadTreeObject
    /// </summary>
    /// <param name="quadTreeObject">Object to remove</param>
    /// <returns></returns>
    public bool Remove(GameObject quadTreeObject)
    {
        if (quadTreeObject == null)
            return false;

        //This is given the fact that every object is confined in the quadtree bounds
        if (!m_bounds.Contains(quadTreeObject.transform.position))
            return false;

        if (m_objectList.Contains(quadTreeObject))
        {
            //Debug.Log("Removed : " + quadTreeObject.name + " from the tree");
            m_objectList.Remove(quadTreeObject);
            return true;
        }
        else
        {
            if (isDivided)
            {
                for (int i = 0; i < QUADTREE_COUNT; i++)
                {
                    if (regions[i].Remove(quadTreeObject))
                    {
                        //Debug.Log("Removed : " + quadTreeObject.name + " from the tree from sub");
                        return true;
                    }
                }
            }

            //Debug.Log("Failed to remove the object");
            return false;

        }
    }

    /// <summary>
    /// Clears all objects in this quadtree and subsequent quadtrees
    /// </summary>
    public void Clear()
    {
        //Else if its not null, we just clear
        if (m_objectList != null)
            m_objectList.Clear();

        if (isDivided)
        {
            for (int i = 0; i < QUADTREE_COUNT; i++)
            {
                if (regions[i] != null)
                {
                    regions[i].Clear();
                    regions[i] = null;
                }
            }
            isDivided = false;
        }
    }


    void SubDivide()
    {
        float x = m_bounds.position.x; // in this case x is... x?s
        float z = m_bounds.position.z; // in this case z is z.
        float w = m_bounds.width;
        float h = m_bounds.height;

        QuadRect[] rectArray = new QuadRect[QUADTREE_COUNT];

        rectArray[0] = new QuadRect(
            new Vector3(x + w * 0.5f, 0, z - h * 0.5f)
            , w * 0.5f
            , h * 0.5f);

        rectArray[1] = new QuadRect(
            new Vector3(x - w * 0.5f, 0, z - h * 0.5f)
            , w * 0.5f
            , h * 0.5f);

        rectArray[2] = new QuadRect(
            new Vector3(x + w * 0.5f, 0, z + h * 0.5f)
            , w * 0.5f
            , h * 0.5f);

        rectArray[3] = new QuadRect(
            new Vector3(x - w * 0.5f, 0, z + h * 0.5f)
            , w * 0.5f
            , h * 0.5f);

        //Make the new regions array
        for (int i = 0; i < QUADTREE_COUNT; i++)
        {
            regions[i] = new QuadTree(rectArray[i], maxCapacity);
        }
        //Set divided to be true
        isDivided = true;

    }

    /// <summary>
    /// Queries this quadtree and child quadtrees for objects.
    /// </summary>
    /// <param name="range">Range to check</param>
    /// <returns></returns>
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
                if (quadObject == null)
                    continue;

                if (range.Contains(quadObject.transform.position))
                {
                    objectsInRange.Add(quadObject);
                }
            }
            if (isDivided)
            {
                if (regions[0] != null)
                {
                    for (int i = 0; i < QUADTREE_COUNT; i++)
                    {
                        regions[i].QueryRange(range, ref objectsInRange);
                    }
                }

            }
        }
    }

    /// <summary>
    /// Draws this quadtree using Gizmos.
    /// </summary>
    /// <param name="offset"></param>
    //Render function
    public void Render(Vector3 offset = new Vector3())
    {
        //Prevent some error in the editor.

        Gizmos.DrawWireCube(
        new Vector3(m_bounds.position.x + offset.x, offset.y, m_bounds.position.z + offset.z),
        new Vector3(m_bounds.width * 2, 0.01f, m_bounds.height * 2));

        if (isDivided)
        {
            if (regions[0] != null)
            {
                for (int i = 0; i < QUADTREE_COUNT; i++)
                {
                    regions[i].Render(offset);
                }
            }
        }
    }

    /// <summary>
    /// Gets the amount of sub divisions in this tree.
    /// </summary>
    /// <param name="here"></param>
    public void GetSubDivisions(ref int here)
    {
        if (isDivided)
        {
            if (regions[0] != null)
            {
                for (int i = 0; i < QUADTREE_COUNT; i++)
                {
                    here += 1;
                    regions[i].GetSubDivisions(ref here);
                }
            }

        }
    }

    /// <summary>
    /// //Gets all objects in this quadtree and returns a reference to it
    /// </summary>
    /// <param name="referenceList">List to populate.</param>
    public void GetObjects(ref List<GameObject> referenceList)
    {
        for (int i = 0; i < m_objectList.Count; ++i)
        {
            referenceList.Add(m_objectList[i]);
        }
        if (isDivided)
        {
            for (int i = 0; i < QUADTREE_COUNT; ++i)
            {
                regions[i].GetObjects(ref referenceList);
            }
        }
    }
}

