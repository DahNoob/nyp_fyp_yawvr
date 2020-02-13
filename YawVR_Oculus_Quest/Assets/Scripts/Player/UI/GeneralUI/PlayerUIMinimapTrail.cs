using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// This class controls the pathfinding done by the minimap to lead players to objectives or interest points.
/// </summary>
[System.Serializable]
public class PlayerUIMinimapTrail
{
    [Header("Player Minimap Trail")]
    [SerializeField]
    private bool componentEnabled;
    [SerializeField]
    private bool enableMinimapTrail;
    [SerializeField]
    private bool enableSecondaryTrail;
    [SerializeField]
    private GameObject minimapTrailFinderPrefab;
    [SerializeField]
    private RectTransform minimapTransformChecker;
    [SerializeField]
    private LineRenderer minimapLineRenderer;
    [SerializeField]
    private LineRenderer minimapLineRendererVisualization;
    [SerializeField]
    private float minimapTrailPollTime = 0.25f;
    [SerializeField]
    private Vector3 lineOffset;
    [SerializeField]
    private float minimapTrailSizeOffset = 0.1f;

    [Header("Player Mech Movement")]
    [SerializeField]
    private MechMovement m_mechMovement;

    //Local variables
    //Object used to find stuff
    private GameObject minimapTrailFinder;
    private NavMeshAgent minimapFinderNav;
    private Camera m_minimapCamera;
    //Timer for updating the minimap trail
    private float minimapTrailTimer;

    //Path variable
    private NavMeshPath navPath;

    //Path for debugging
    //Visualization
    private Vector3[] minimapPath;


    // Start is called before the first frame update
    public void Start()
    {
        if (!componentEnabled)
            return;

        if (enableMinimapTrail)
        {
            m_minimapCamera = PlayerUIManager.instance.minimapCamera;
            minimapTrailFinder = GameObject.Instantiate(minimapTrailFinderPrefab, PlayerHandler.instance.transform.position, Quaternion.identity, PlayerHandler.instance.transform);
            minimapFinderNav = minimapTrailFinder.GetComponent<NavMeshAgent>();
            minimapFinderNav.Warp(minimapTrailFinder.transform.position);
            minimapFinderNav.updatePosition = true;
        }

        //Initialise nav path
        navPath = new NavMeshPath();
    }

    // Update is called once per frame
    public void Update()
    {
        if (!componentEnabled)
            return;


        if (minimapTrailTimer < minimapTrailPollTime)
        {
            minimapTrailTimer += Time.smoothDeltaTime;
        }
        else
        {
            //Reset timer
            minimapTrailTimer -= minimapTrailPollTime;
            //Calculate path towards stuff

            if (IsCharacterMoving())
                Pathfind();

        }
    }

    /// <summary>
    /// Finds a path to nearest objective or interest point.
    /// </summary>
    public void Pathfind()
    {
        minimapFinderNav.Warp(PlayerHandler.instance.transform.position);

        ObjectiveInfo currObj = Game.instance.GetCurrentObjectiveInfo();
        if (currObj != null)
        {
            PathfindToObjective(currObj);
        }
        else
        {
            //Curr obj is null, find nearest one
            //Find path towards current objective
            //ObjectiveInfo currObj = Game.instance.GetCurrentObjectiveInfo();
            ObjectiveInfo nearestObjective = Game.instance.ReturnNearestObjectiveToPlayer();
            PathfindToObjective(nearestObjective);
        }
    }

    void PathfindToObjective(ObjectiveInfo referenceObj)
    {
        if (IsObjectiveValid(referenceObj))
        {
            minimapFinderNav.CalculatePath(referenceObj.m_highlight.position, navPath);

            //Check if path is valid
            if (navPath.status != NavMeshPathStatus.PathPartial)
            {
                //Set the path for easier access or something
                minimapPath = navPath.corners;
                int minimapPathLength = minimapPath.Length;
                FirstTrail(minimapPathLength);
                SecondTrail(minimapPathLength);
            }
        }
        else
        {
            //Disable the renderers
            HandleActive(minimapLineRenderer.gameObject, false);
            HandleActive(minimapLineRendererVisualization.gameObject, false);
        }
    }

    /// <summary>
    /// Enables the first trail (on minimap)
    /// </summary>
    /// <param name="minimapPathLength"></param>
    public void FirstTrail(int minimapPathLength)
    {
        if (!enableMinimapTrail)
        {
            HandleActive(minimapLineRenderer.gameObject, false);
            return;
        }

        HandleActive(minimapLineRenderer.gameObject, true);

        if (minimapLineRenderer != null)
        {
            // minimapLineRenderer.positionCount = (minimapPath.Length);
            minimapLineRenderer.positionCount = minimapPathLength;

            //Set the width based on the normalized scale
            minimapLineRenderer.startWidth = PlayerUIManager.instance.normalizedScale - minimapTrailSizeOffset;
            minimapLineRenderer.endWidth = PlayerUIManager.instance.normalizedScale - minimapTrailSizeOffset;

            for (int i = 0; i < minimapPathLength; ++i)
            {
                //Check if this point is inside the viewport
                Vector3 displacement = Vector3.Scale((minimapPath[i] - PlayerHandler.instance.transform.position), new Vector3(1, 0, 1));
                Vector3 displacementNormalized = displacement.normalized;
                //Normalized value of that distance between the max size (query bounds) and not
                float normalized = CustomUtility.Normalize(displacement.magnitude, 0, m_minimapCamera.orthographicSize + PlayerUIManager.instance.m_customRange);
                float normalizedRejection = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_rejectionRange);

                //Get angle in which the displacement is done
                Vector3 bap = displacement.normalized;
                float angle = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;

                //Set the rect transform stuffs
                minimapTransformChecker.localPosition = Vector3.zero;
                minimapTransformChecker.localEulerAngles = new Vector3(0, 0, angle + PlayerHandler.instance.transform.eulerAngles.y + 180);

                //If any index is outside, we can safely assume that nothing else will be rendered, and we can keep the cost of the line renderer down
                if (normalizedRejection > PlayerUIManager.instance.m_rejectionRange)
                {
                    //Translate the rectTransform based on eulers
                    minimapTransformChecker.Translate(0, PlayerUIManager.instance.m_customRangeTwo, 0);
                    //float normalizedCustomRange = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_customRangeTwo);
                    ////Translate the rectTransform based on eulers
                    //minimapTransformChecker.Translate(0, normalizedCustomRange, 0);

                    //Prevent any more points from rendering
                    minimapLineRenderer.positionCount = i + 1;
                    //Can start the break from here, else we just continue as usual.
                    minimapLineRenderer.SetPosition(i, minimapTransformChecker.localPosition);
                    break;
                }
                else
                {
                    float normalizedCustomRange = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_customRangeTwo);
                    //Translate the rectTransform based on eulers
                    minimapTransformChecker.Translate(0, normalizedCustomRange, 0);
                }

                //Set position of new line to new calculated position on the map
                minimapLineRenderer.SetPosition(i, minimapTransformChecker.localPosition);
            }
        }

    }
    /// <summary>
    /// Enables the second trail (world space)
    /// </summary>
    /// <param name="minimapPathLength"></param>
    public void SecondTrail(int minimapPathLength)
    {
        if (!enableSecondaryTrail)
        {
            HandleActive(minimapLineRendererVisualization.gameObject, false);
            return;
        }

        HandleActive(minimapLineRendererVisualization.gameObject, true);

        if (minimapLineRendererVisualization != null)
        {
            minimapLineRendererVisualization.positionCount = minimapPathLength;

            for (int i = 0; i < minimapPathLength; ++i)
            {
                minimapLineRendererVisualization.SetPosition(i, minimapPath[i] + lineOffset);
            }
        }

    }

    /// <summary>
    /// Helper function to prevent redundant SetActive calls .
    /// </summary>
    /// <param name="go">GameObject reference</param>
    /// <param name="active">Active value</param>
    public void HandleActive(GameObject go, bool active)
    {
        if (go.activeInHierarchy != active)
            go.SetActive(active);

    }

    /// <summary>
    /// Checks if the referenceObjective is null or completed to update the minimap
    /// </summary>
    /// <param name="referenceInfo">Reference objective</param>
    /// <returns>True if valid, false if not.</returns>
    public bool IsObjectiveValid(ObjectiveInfo referenceInfo)
    {
        return !(referenceInfo == null || referenceInfo.m_highlight == null || referenceInfo.m_completed);
    }

    /// <summary>
    /// Helper function that returns a bool if the player is moving.
    /// </summary>
    /// <returns>True if player is moving, false if not.</returns>
    public bool IsCharacterMoving()
    {
        if (m_mechMovement == null)
            return false;

        return m_mechMovement.isWalking || m_mechMovement.rotationAxisSmoothedDelta_Current != 0 || m_mechMovement.isFalling;
    }
}
