using System.Collections;
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
** 1    20/12/2019,  2.58 PM     Isaac   Added remove and clear functionalities, and updating of tree
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
    [SerializeField]
    [Tooltip("Toggle between gizmo modes for visualizations")]
    private GIZMOMODES gizmoModes;
    [SerializeField]
    [Tooltip("Which tree to render")]
    private SELECTEDTREE selectedTree;
    [SerializeField]
    [ColorUsage(true,true)]
    [Tooltip("Color for the gizmos grid")]
    private Color staticTreeColor;
    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("Color for the quadtree bounds")]
    private Color dynamicTreeColor;
    [SerializeField]
    [Tooltip("Offsets for bounds")]
    private Vector2 quadTreeBoundsOffset;
    [SerializeField]
    [Tooltip("Overall height for visualizations")]
    private Vector3 gizmosOffsets;
    #endregion

    [Header("QuadTree Configuration")]
    [SerializeField]
    [Tooltip("How big the quadtree is, turn on gizmos to check.")]
    private QuadRect quadTreeBounds;
    [SerializeField]
    [Tooltip("How much each node can take before it splits")]
    private int maxCapacity = 10;
    [SerializeField]
    [Tooltip("How much time in seconds the quadtree updates itself. < More accurate, > Less accurate")]
    private float updateTick = 1.0f;

    //Two trees which might take more memory, but provides faster search results
    //Static one rarely needs to be "edited"
    //As long as there are a controllable amount of "dynamic", it should still be ok
    //Static tree
    private QuadTree staticQuadTree;
    //Dynamic tree
    private QuadTree dynamicQuadTree;

    [SerializeField] //serialize added for visualization in inspector.
    private List<GameObject> staticList;
    [SerializeField] //serialize added for visualization in inspector.
    private List<GameObject> dynamicList;

    //Test variables
    [SerializeField]
    private QuadRect queryBounds;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Material[] materialArray;

    public bool showBoundaries = false;

    void Awake()
    {
        if (instance == null)
            instance = this;

        //Set minimum capacity to be 1
        maxCapacity = Mathf.Max(1, maxCapacity);

        //Initialize the quadtrees
        staticQuadTree = new QuadTree(quadTreeBounds, maxCapacity);
        dynamicQuadTree = new QuadTree(quadTreeBounds, maxCapacity);
    }

    private void Start()
    {
        //Start the updating of the tree.
        StartCoroutine(UpdateDynamicQuadTree());
    }

    // Update is called once per frame
    void Update()
    {
        if(showBoundaries)
            DrawNodes();
    }

    //Adds an object to the static list, shouldn't be called at all, other than by quadtreeobjects.
    public bool AddToStaticQuadTree(GameObject referenceObject)
    {
        Insert(referenceObject, true);
        if (!staticList.Contains(referenceObject))
        {
            staticList.Add(referenceObject);
            return true;
        }
        return false;
    }

    //Adds an object to the dynamic list, shouldn't be called at all, other than by quadtreeobjects.
    public bool AddToDynamicQuadTree(GameObject referenceObject)
    {
        Insert(referenceObject, false);
        if (!dynamicList.Contains(referenceObject))
        {
            dynamicList.Add(referenceObject);
            return true;
        }
        return false;
    }

    //Function that visualizes the current grid
    void DrawGrid()
    {
        //Draw the outer layer first
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(quadTreeBounds.position,
            new Vector3((quadTreeBounds.width * 2) + quadTreeBoundsOffset.x, 0.01f, (quadTreeBounds.height * 2) + quadTreeBoundsOffset.y));

        if (staticQuadTree != null && (selectedTree == SELECTEDTREE.STATIC || selectedTree == SELECTEDTREE.STATIC_DYNAMIC))
        {
            Gizmos.color = staticTreeColor;
            staticQuadTree.Render(gizmosOffsets + new Vector3(0, quadTreeBounds.position.y, 0));
        }
        if (dynamicQuadTree != null && (selectedTree == SELECTEDTREE.DYNAMIC || selectedTree == SELECTEDTREE.STATIC_DYNAMIC))
        {
            Gizmos.color = dynamicTreeColor;
            dynamicQuadTree.Render(gizmosOffsets + new Vector3(0, quadTreeBounds.position.y, 0));
        }
    }

    //Updates tree
    IEnumerator UpdateDynamicQuadTree()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateTick);

            ResetDynamicTree();

            //Reset the moving tree
            for (int i = 0; i < dynamicList.Count; i++)
            {
                dynamicList[i].GetComponent<DynamicQuadTreeObject>().AddToQuadTree(dynamicList[i].gameObject);
            }
        }
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
        bool result = isStatic ? staticQuadTree.Remove(referenceObject) : dynamicQuadTree.Remove(referenceObject);
        return result;
    }

    public bool ResetQuadTrees()
    {
        if (staticQuadTree != null)
            staticQuadTree.Clear();

        if (dynamicQuadTree != null)
            dynamicQuadTree.Clear();

        ////Make new ones
        //staticQuadTree = new QuadTree(quadTreeBounds, maxCapacity);
        //dynamicQuadTree = new QuadTree(quadTreeBounds, maxCapacity);

        //Debug.Log("Quadtree reset.");
        return true;
    }

    public bool ResetDynamicTree()
    {
        if (dynamicQuadTree != null)
            dynamicQuadTree.Clear();

        return true;
    }

    public bool ResetStaticTree()
    {
        if (staticQuadTree != null)
            staticQuadTree.Clear();

        return true;
    }

    private void OnDrawGizmos()
    {
        if (gizmoModes != GIZMOMODES.SHOW)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(queryBounds.position,
            queryBounds.GetWidth() * 2);

        DrawGrid();
    }

    private void OnDrawGizmosSelected()
    {
        if (gizmoModes != GIZMOMODES.SHOWSELECTED)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(queryBounds.position,
            queryBounds.GetWidth() * 2);

        DrawGrid();
    }

    public List<GameObject> surroundingObjects;
    void DrawNodes()
    {
        queryBounds.position = playerTransform.position;
       surroundingObjects = Query(queryBounds, true);

        foreach(GameObject statics in staticList)
        {
            MeshRenderer[] renderers = statics.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer meshR in renderers)
            {
                meshR.material = materialArray[0];
            }
        }

        foreach(GameObject surrounds in surroundingObjects)
        {
            MeshRenderer[] renderers = surrounds.GetComponentsInChildren<MeshRenderer>();
            foreach(MeshRenderer meshR  in renderers)
            {
                meshR.material = materialArray[1];
            }
        }


    }

}
