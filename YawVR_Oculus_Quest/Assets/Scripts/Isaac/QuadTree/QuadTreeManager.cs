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
** 1    19/12/2019,  11:31 AM    Isaac  Created
** 2    20/12/2019,    2.58 PM     Isaac  Added remove and clear functionalities, and updating of tree
** 3    06/01/2020,  12.00 AM    Isaac   Forgot the other dates, but yes finished revamping QuadTree
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

    private enum RENDER_OPTIONS
    {
        SELECTED_STATIC,     //renders the current static enum of dynamic types
        SELECTED_DYNAMIC, // renders the current enum of dynamic types
        ALL_STATIC,
        ALL_DYNAMIC,
        STATIC_DYNAMIC,
        NONE
    }

    public enum STATIC_TYPES
    {
        TERRAIN,
        MAP_POINTS,
        TOTAL_TYPE
    }

    public enum DYNAMIC_TYPES
    {
        ENEMIES,
        TOTAL_TYPE
    }

    [System.Serializable]
    public struct QuadTreeVisualization
    {
        public string name;
        public List<GameObject> m_objectList;

        public QuadTreeVisualization(string name, List<GameObject> objectList)
        {
            this.name = name;
            this.m_objectList = objectList;
        }
    }

    #region Visualizations
    [Header("QuadTree Visualisations")]
    [SerializeField]
    [Tooltip("Toggle between gizmo modes for visualizations")]
    private GIZMOMODES gizmoModes;
    [SerializeField]
    [Tooltip("Which trees to render")]
    private RENDER_OPTIONS selectedTree;
    [SerializeField]
    [Tooltip("Currently rendered static tree")]
    private STATIC_TYPES m_staticRender;
    [SerializeField]
    [Tooltip("Currently rendered dynamic tree")]
    private DYNAMIC_TYPES m_dynamicRender;
    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("Color for the static grid")]
    private Color staticTreeColor;
    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("Color for the dynamic tree bounds")]
    private Color dynamicTreeColor;
    [SerializeField]
    [ColorUsage(true, true)]
    [Tooltip("Color for the quadtree bounds")]
    private Color quadTreeColor;
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
    private Dictionary<STATIC_TYPES, QuadTree> staticTrees;
    //Dynamic tree
    private Dictionary<DYNAMIC_TYPES, QuadTree> dynamicTrees;

    [SerializeField] //serialize added for visualization in inspector.
    private List<GameObject> staticList;
    [SerializeField] //serialize added for visualization in inspector.
    private List<GameObject> dynamicList;

    [Header("Visualizations")]
    [SerializeField]
    private bool enableVisualizations;
    [SerializeField]
    private List<QuadTreeVisualization> m_staticVisualizations;
    [SerializeField]
    private List<QuadTreeVisualization> m_dynamicVisualizations;

    ////Test variables
    //[SerializeField]
    //private QuadRect queryBounds;
    //[SerializeField]
    //private Transform playerTransform;
    //[SerializeField]
    //private Material[] materialArray;


    void Awake()
    {
        if (instance == null)
            instance = this;

        //Set minimum capacity to be 1
        maxCapacity = Mathf.Max(1, maxCapacity);

        //Initialize the quadtrees
        staticTrees = new Dictionary<STATIC_TYPES, QuadTree>();
        dynamicTrees = new Dictionary<DYNAMIC_TYPES, QuadTree>();


        //Initialise the values
        for (int i = 0; i < (int)STATIC_TYPES.TOTAL_TYPE; ++i)
        {
            STATIC_TYPES currentType = (STATIC_TYPES)(i);
            staticTrees[currentType] = new QuadTree(quadTreeBounds, maxCapacity);
            if (enableVisualizations)
            {
                m_staticVisualizations.Add(new QuadTreeVisualization(currentType.ToString(), new List<GameObject>()));
            }
        }
        for (int i = 0; i < (int)DYNAMIC_TYPES.TOTAL_TYPE; ++i)
        {
            DYNAMIC_TYPES currentType = (DYNAMIC_TYPES)(i);
            dynamicTrees[currentType] = new QuadTree(quadTreeBounds, maxCapacity);
            if (enableVisualizations)
            {
                m_dynamicVisualizations.Add(new QuadTreeVisualization(currentType.ToString(), new List<GameObject>()));
            }
        }

    }

    private void Start()
    {
        //Start the updating of the tree.
        StartCoroutine(UpdateDynamicQuadTree());
    }

    //public List<GameObject> nearbyTransforms;
    // Update is called once per frame
    void Update()
    {
        //Remove all null entires
        //staticList.RemoveAll(item => item == null);


        //queryBounds.position = playerTransform.position;
        //nearbyTransforms = QueryStaticObjects(queryBounds, STATIC_TYPES.MAP_POINTS);


        if (enableVisualizations)
        {
            //Initialise the values
            for (int i = 0; i < (int)STATIC_TYPES.TOTAL_TYPE; ++i)
            {
                STATIC_TYPES currentType = (STATIC_TYPES)(i);
                List<GameObject> m_referenceList = new List<GameObject>();
                staticTrees[currentType].GetObjects(ref m_referenceList);
                m_staticVisualizations[i] = (new QuadTreeVisualization(currentType.ToString(), m_referenceList));
            }
            for (int i = 0; i < (int)DYNAMIC_TYPES.TOTAL_TYPE; ++i)
            {
                DYNAMIC_TYPES currentType = (DYNAMIC_TYPES)(i);
                List<GameObject> m_referenceList = new List<GameObject>();
                dynamicTrees[currentType].GetObjects(ref m_referenceList);
                m_dynamicVisualizations[i] = (new QuadTreeVisualization(currentType.ToString(), m_referenceList));
            }
        }

    }

    //Adds an object to the static list, shouldn't be called at all, other than by quadtreeobjects.
    public bool AddToStaticQuadTree(GameObject referenceObject, STATIC_TYPES types)
    {
        if (referenceObject == null)
            return false;

        if (InsertStaticObject(referenceObject, types))
        {
            if (!staticList.Contains(referenceObject))
            {
                staticList.Add(referenceObject);
                return true;
            }
        }
        return false;
    }

    //Adds an object to the dynamic list, shouldn't be called at all, other than by quadtreeobjects.
    public bool AddToDynamicQuadTree(GameObject referenceObject, DYNAMIC_TYPES types)
    {
        if (referenceObject == null)
            return false;
        if (InsertDynamicObject(referenceObject, types))
        {
            if (!dynamicList.Contains(referenceObject))
            {

                dynamicList.Add(referenceObject);
                return true;
            }
        }
        return false;
    }

    //Function that visualizes the current grid
    void DrawGrid()
    {
        //Draw the outer layer first
        Gizmos.color = quadTreeColor;
        Gizmos.DrawWireCube(quadTreeBounds.position,
            new Vector3((quadTreeBounds.width * 2) + quadTreeBoundsOffset.x, 0.01f, (quadTreeBounds.height * 2) + quadTreeBoundsOffset.y));


        switch (selectedTree)
        {
            case RENDER_OPTIONS.SELECTED_STATIC:
                {
                    if (staticTrees != null)
                    {
                        Gizmos.color = staticTreeColor;
                        if (staticTrees[m_staticRender] != null && staticTrees != null)
                            staticTrees[m_staticRender].Render();
                    }
                    break;
                }
            case RENDER_OPTIONS.SELECTED_DYNAMIC:
                {
                    if (dynamicTrees != null)
                    {
                        Gizmos.color = dynamicTreeColor;
                        if (dynamicTrees[m_dynamicRender] != null)
                            dynamicTrees[m_dynamicRender].Render();
                    }
                    break;
                }
            case RENDER_OPTIONS.ALL_STATIC:
                {
                    if (staticTrees != null)
                    {
                        Gizmos.color = staticTreeColor;
                        foreach (KeyValuePair<STATIC_TYPES, QuadTree> tree in staticTrees)
                        {
                            if (tree.Value != null)
                                tree.Value.Render();
                        }
                    }
                    break;
                }
            case RENDER_OPTIONS.ALL_DYNAMIC:
                {
                    if (dynamicTrees != null)
                    {
                        Gizmos.color = dynamicTreeColor;
                        foreach (KeyValuePair<DYNAMIC_TYPES, QuadTree> tree in dynamicTrees)
                        {
                            if (tree.Value != null)
                                tree.Value.Render();
                        }
                    }
                    break;
                }
            case RENDER_OPTIONS.STATIC_DYNAMIC:
                {
                    if (staticTrees != null)
                    {
                        Gizmos.color = staticTreeColor;
                        foreach (KeyValuePair<STATIC_TYPES, QuadTree> tree in staticTrees)
                        {
                            if (tree.Value != null)
                                tree.Value.Render();
                        }
                    }
                    if (dynamicTrees != null)
                    {
                        Gizmos.color = dynamicTreeColor;
                        foreach (KeyValuePair<DYNAMIC_TYPES, QuadTree> tree in dynamicTrees)
                        {
                            if (tree.Value != null)
                                tree.Value.Render();
                        }
                    }
                    break;

                }
            case RENDER_OPTIONS.NONE:
                break;
            default:
                break;
        }


    }

    //Updates tree
    IEnumerator UpdateDynamicQuadTree()
    {
        while (true)
        {

            ResetDynamicTrees();

            //Reset the moving tree
            for (int i = 0; i < dynamicList.Count; i++)
            {
                if (dynamicList[i] != null)
                {
                    DynamicQuadTreeObject dynamicObject = dynamicList[i].GetComponent<DynamicQuadTreeObject>();
                    dynamicObject.AddToQuadTree(dynamicList[i].gameObject, dynamicObject.Type);
                    continue;
                }

                dynamicList.Remove(dynamicList[i]);
            }
            yield return new WaitForSeconds(updateTick);
        }
    }

    //Fine I'll make two
    bool InsertStaticObject(GameObject referenceObject, STATIC_TYPES types)
    {
        if (!staticTrees.ContainsKey(types))
        {
            print("[InsertStaticObject]Static tree does not contain key of static type: " + types.ToString());
            return false;
        }

        //Else there is a key so pog
        bool result = staticTrees[types].Insert(referenceObject);
        return result;
    }

    bool InsertDynamicObject(GameObject referenceObject, DYNAMIC_TYPES types)
    {
        if (!dynamicTrees.ContainsKey(types))
        {
            print("[InsertDynamicObjects]Dynamic tree does not contain key of dynamic type: " + types.ToString());
            return false;
        }

        //Else there is a key so pog
        bool result = dynamicTrees[types].Insert(referenceObject);
        return result;
    }

    public List<GameObject> QueryStaticObjects(QuadRect queryBounds, STATIC_TYPES types)
    {
        if (!staticTrees.ContainsKey(types))
        {
            print("[QueryStaticObjects] Static trees does not contain key of type: " + types.ToString());
            return null;
        }

        //Else there is a key so pog
        List<GameObject> result = staticTrees[types].Query(queryBounds);
        return result;
    }

    public List<GameObject> QueryDynamicObjects(QuadRect queryBounds, DYNAMIC_TYPES types)
    {
        if (!dynamicTrees.ContainsKey(types))
        {
            print("[QueryDynamicObjects] Static trees does not contain key of type: " + types.ToString());
            return null;
        }


        //Else there is a key so pog
        List<GameObject> result = dynamicTrees[types].Query(queryBounds);
        return result;
    }

    public bool RemoveStaticObject(GameObject referenceObject, STATIC_TYPES types)
    {
        if (!staticTrees.ContainsKey(types))
        {
            print("[RemoveStaticObjects] Static trees does not contain key of type: " + types.ToString());
            return false;
        }
        //Else there is a key so pog
        bool result = staticTrees[types].Remove(referenceObject);
        staticList.Remove(referenceObject);
        return result;
    }
    public bool RemoveDynamicObject(GameObject referenceObject, DYNAMIC_TYPES types)
    {
        if (!dynamicTrees.ContainsKey(types))
        {
            print("[RemoveDynamicObjects] Static trees does not contain key of type: " + types.ToString());
            return false;
        }
        //Else there is a key so pog
        bool result = dynamicTrees[types].Remove(referenceObject);
        dynamicList.Remove(referenceObject);
        return result;
    }


    public bool ResetQuadTrees()
    {
        ResetStaticTrees();
        ResetDynamicTrees();

        //Debug.Log("Quadtree reset.");
        return true;
    }

    public bool ResetDynamicTrees()
    {
        foreach (KeyValuePair<DYNAMIC_TYPES, QuadTree> dynamicTree in dynamicTrees)
        {
            if (dynamicTree.Value != null)
                dynamicTree.Value.Clear();
        }

        return true;
    }

    public bool ResetStaticTrees()
    {
        foreach (KeyValuePair<STATIC_TYPES, QuadTree> staticTree in staticTrees)
        {
            if (staticTree.Value != null)
                staticTree.Value.Clear();
        }

        return true;
    }

    private void OnDrawGizmos()
    {
        if (gizmoModes != GIZMOMODES.SHOW)
            return;

        Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(queryBounds.position,
        //    queryBounds.GetWidth() * 2);

        DrawGrid();
    }

    private void OnDrawGizmosSelected()
    {
        if (gizmoModes != GIZMOMODES.SHOWSELECTED)
            return;

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireCube(queryBounds.position,
        //    queryBounds.GetWidth() * 2);

        DrawGrid();
    }

    //Terrain test for mid presentation

    //[SerializeField]
    // private List<GameObject> surroundingObjects;
    // public bool showBoundaries = false;
    // void DrawNodes()
    // {
    //     queryBounds.position = playerTransform.position;
    //     surroundingObjects = QueryStaticObjects(queryBounds, STATIC_TYPES.TERRAIN);

    //     foreach (GameObject statics in staticList)
    //     {
    //         MeshRenderer[] renderers = statics.GetComponentsInChildren<MeshRenderer>();
    //         foreach (MeshRenderer meshR in renderers)
    //         {
    //             meshR.material = materialArray[0];
    //         }
    //     }

    //     foreach (GameObject surrounds in surroundingObjects)
    //     {
    //         MeshRenderer[] renderers = surrounds.GetComponentsInChildren<MeshRenderer>();
    //         foreach (MeshRenderer meshR in renderers)
    //         {
    //             meshR.material = materialArray[1];
    //         }
    //     }


    // }

    // void DrawNodesLessOptimized()
    // {
    //     foreach (GameObject statics in staticList)
    //     {
    //         MeshRenderer[] renderers = statics.GetComponentsInChildren<MeshRenderer>();

    //         if (queryBounds.Contains(statics.transform.position))
    //         {
    //             foreach (MeshRenderer meshR in renderers)
    //             {
    //                 meshR.material = materialArray[1];
    //             }
    //         }
    //         else
    //         {
    //             foreach (MeshRenderer meshR in renderers)
    //             {
    //                 meshR.material = materialArray[0];
    //             }
    //         }
    //     }
    // }

    // void DistanceCheck()
    // {
    //     for (int i = 0; i < staticList.Count; ++i)
    //     {
    //         QuadRect bounds = new QuadRect(staticList[i].transform.position, 50, 50);
    //         List<GameObject> surrounds = QueryStaticObjects(queryBounds, STATIC_TYPES.TERRAIN);

    //         foreach (GameObject surround in surrounds)
    //         {
    //             float distance = Vector3.Distance(staticList[i].transform.position, surround.transform.position);
    //         }
    //     }
    // }

    // void DistanceCheckLessOptimized()
    // {
    //     for (int i = 0; i < staticList.Count; ++i)
    //     {
    //         for (int x = 0; x < staticList.Count; ++x)
    //         {
    //             if (i == x)
    //                 continue;

    //             float distance = Vector3.Distance(staticList[i].transform.position, staticList[x].transform.position);
    //         }
    //     }
    // }
}
