﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/******************************  
** Name: QuadTree Manager
** Desc: Provides access to quadtree functions, while making sure information in quadtree goes out only.
** Author: Isaac
** Date: 19/12/2019, 11:31 AM
**************************
** Change History
**************************
** PR   Date                    Author    Description 
** --   --------                -------   ------------------------------------
** 1    19/12/2019, 11:31 AM     Isaac   Created
*******************************/
public class QuadTreeManager : MonoBehaviour
{
    //Instance to access the quadtree manager
    public static QuadTreeManager instance { get; private set; }

    private enum GIZMOMODES
    {
        OFF,
        SHOW,
        SHOWSELECTED
    }

    private enum SELECTEDTREE
    {
        STATIC,
        DYNAMIC,
        STATIC_DYNAMIC,
        NONE
    }

    #region Visualizations
        [Header("QuadTree Visualisations")]
        [SerializeField] [Tooltip("Toggle between gizmo modes for visualizations")]
        private GIZMOMODES gizmoModes;
        [SerializeField] [Tooltip("Which tree to render")]
        private SELECTEDTREE selectedTree;
        [SerializeField] [Tooltip("Color for the gizmos grid")]
        private Color staticTreeColor;
        [SerializeField] [Tooltip("Color for the quadtree bounds")]
        private Color dynamicTreeColor;
        [SerializeField] [Tooltip("Offsets for bounds")]
        private Vector2 quadTreeBoundsOffset;
        [SerializeField] [Tooltip("Overall height for visualizations")]
        private Vector3 gizmosOffsets;
    #endregion

    [Header("QuadTree Configuration")]
    [SerializeField] [Tooltip("How big the quadtree is, turn on gizmos to check.")]
    private QuadRect quadTreeBounds;
    [SerializeField]
    [Tooltip("How much each node can take before it splits")]
    private int maxCapacity = 4;

    //Two trees which might take more memory, but provides faster search results
    //Static one rarely needs to be "edited"
    //As long as there are a controllable amount of "dynamic", it should still be ok
    //Static tree
    private QuadTree staticQuadTree;
    //Dynamic tree
    private QuadTree dynamicQuadTree;

    public GameObject poggers;

    //Test variables
    public QuadRect queryBounds;
    public List<GameObject> staticList;
    public List<GameObject> dynamicList;
    public Material[] materialArray;

    public Text quadTreeCheck;
    public Text staticsubDivisions;
    public Text dynamicsubDivisions;

    public int checkCounter;
    public int staticsubDiv;
    public int dynamicsubDiv;
    

    void Awake()
    {
        if (instance == null)
            instance = this;

        //Set minimum capacity to be 1
        maxCapacity = Mathf.Max(1, maxCapacity);

        staticQuadTree = new QuadTree(quadTreeBounds, maxCapacity);
        dynamicQuadTree = new QuadTree(quadTreeBounds, maxCapacity);
    }

    private void Start()
    {
        SpawnStuff();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            //SpawnDynamic();
            ResetQuadTree();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * 10000);

        if (Physics.Raycast(ray.origin, ray.direction, out hit, LayerMask.GetMask("PlaneTest")))
        {
            queryBounds.position = hit.point;
            if (Input.GetMouseButton(0))
            {
                GameObject newObject = Instantiate(poggers, Vector3.zero, Quaternion.identity);
                newObject.transform.position = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                if (selectedTree == SELECTEDTREE.STATIC)
                {
                    staticQuadTree.Insert(newObject);
                    staticList.Add(newObject);
                }
                else if(selectedTree == SELECTEDTREE.DYNAMIC)
                {
                    dynamicQuadTree.Insert(newObject);
                    dynamicList.Add(newObject);
                }

            }
        }
        checkCounter = 0;
        RangeStuff();
        quadTreeCheck.text = "Quadtree checks : " + checkCounter.ToString();

        staticsubDiv = 0;
        dynamicsubDiv = 0;

        staticQuadTree.GetSubDivisions(ref staticsubDiv);
        dynamicQuadTree.GetSubDivisions(ref dynamicsubDiv);

        staticsubDivisions.text = "Static div : " + staticsubDiv.ToString();
        dynamicsubDivisions.text = "Dynamic div : " + dynamicsubDiv.ToString();

    }

    public void SpawnStuff()
    {
        for (int i = 0; i < 100; ++i)
        {
            GameObject newObject = Instantiate(poggers, Vector3.zero, Quaternion.identity);
            newObject.transform.position = new Vector3(Random.Range(-90, 90), 2, Random.Range(-90, 90));
            staticQuadTree.Insert(newObject);
            staticList.Add(newObject);
        }
        for (int i = 0; i <10; ++i)
        {
            GameObject newObject = Instantiate(poggers, Vector3.zero, Quaternion.identity);
            newObject.transform.position = new Vector3(Random.Range(-90, 90), 2, Random.Range(-90, 90));
            dynamicQuadTree.Insert(newObject);
            dynamicList.Add(newObject);
        }
    }

    public void SpawnDynamic()
    {
        for (int i = 0; i < 10; ++i)
        {
            GameObject newObject = Instantiate(poggers, Vector3.zero, Quaternion.identity);
            newObject.transform.position = new Vector3(Random.Range(-90, 90), 2, Random.Range(-90, 90));
            dynamicQuadTree.Insert(newObject);
            dynamicList.Add(newObject);
        }
    }

    //Function that visualizes the current grid
    void DrawGrid()
    {
        //Draw the outer layer first
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(quadTreeBounds.position,
            new Vector3((quadTreeBounds.width * 2) + quadTreeBoundsOffset.x, 0.01f, (quadTreeBounds.height * 2) + quadTreeBoundsOffset.y));

           if (staticQuadTree != null && (selectedTree == SELECTEDTREE.STATIC ||selectedTree == SELECTEDTREE.STATIC_DYNAMIC))
        {
            Gizmos.color = staticTreeColor;
            staticQuadTree.Render(gizmosOffsets + new Vector3(0, quadTreeBounds.position.y,0));
        }
        if(dynamicQuadTree != null && (selectedTree == SELECTEDTREE.DYNAMIC || selectedTree == SELECTEDTREE.STATIC_DYNAMIC))
        {
            Gizmos.color = dynamicTreeColor;
            dynamicQuadTree.Render(gizmosOffsets + new Vector3(0, quadTreeBounds.position.y, 0));
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(queryBounds.position,
            queryBounds.GetWidth() * 2);

        if (gizmoModes != GIZMOMODES.SHOW)
            return;
        DrawGrid();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(queryBounds.position,
            queryBounds.GetWidth() * 2);

        if (gizmoModes != GIZMOMODES.SHOWSELECTED)
            return;
        DrawGrid();
    }

    //Easier access to the quad tree's functions
    public bool Insert(GameObject referenceObject, bool isStatic)
    {
        bool result = isStatic ? staticQuadTree.Insert(referenceObject) : dynamicQuadTree.Insert(referenceObject);
        return result;
    }

    public List<GameObject> Query(QuadRect queryBounds, bool isStatic)
    {
        List<GameObject> result = isStatic ? staticQuadTree.Query(queryBounds) : dynamicQuadTree.Query(queryBounds);
        return result;
    }

    public bool Remove(GameObject referenceObject, bool isStatic)
    {
        //IMPLEMENT LATER
        return false;
    }

    public bool ResetQuadTree()
    {
        if (staticQuadTree != null)
            staticQuadTree.Clear();

        if (dynamicQuadTree != null)
            dynamicQuadTree.Clear();

        //Make new ones
        staticQuadTree = new QuadTree(quadTreeBounds, maxCapacity);
        dynamicQuadTree = new QuadTree(quadTreeBounds, maxCapacity);

        Debug.Log("Quadtree reset.");
        return true;
    }

    //TEST FUNCTIONS
    void RangeStuff()
    {
        foreach (GameObject testObject in staticList)
        {
            if(selectedTree != SELECTEDTREE.STATIC)
            {
                testObject.SetActive(false);
                continue;
            }
            else
            {
                testObject.SetActive(true);
            }

            //Get access to their mesh renderers
            MeshRenderer meshRenderer = testObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = materialArray[1];
            //Set active also
            
        }
        foreach (GameObject testObject in dynamicList)
        {
            if (selectedTree != SELECTEDTREE.DYNAMIC)
            {
                testObject.SetActive(false);
                continue;
            }
            else
            {
                testObject.SetActive(true);
            }
            //Get access to their mesh renderers
            MeshRenderer meshRenderer = testObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = materialArray[1];
            //Set active also

        }

        List<GameObject> testObjectList = new List<GameObject>();
        testObjectList = selectedTree == SELECTEDTREE.STATIC ? staticQuadTree.Query(queryBounds) : dynamicQuadTree.Query(queryBounds);
        foreach (GameObject testObject in testObjectList)
        {
            //Get access to their mesh renderers
            MeshRenderer meshRenderer = testObject.GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = materialArray[0];
        }

    }

}
