using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

    //Local variables
    //Object used to find stuff
    private GameObject minimapTrailFinder;
    private NavMeshAgent minimapFinderNav;
    private Camera m_minimapCamera;

    //Path for debugging
    //Visualization
    [SerializeField]
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
        }
    }

    // Update is called once per frame
    public void Update()
    {
        if (!componentEnabled)
            return;


        //Calculate path towards stuff
        minimapFinderNav.Warp(PlayerHandler.instance.transform.position);
        minimapFinderNav.updatePosition = true;
        NavMeshPath navPath = new NavMeshPath();
        minimapFinderNav.CalculatePath(Game.instance.m_objectives[0].m_highlight.position, navPath);
        if (navPath.status != NavMeshPathStatus.PathPartial)
        {
            //Set the path for easier access or something
            minimapPath = navPath.corners;
            int minimapPathLength = minimapPath.Length;
            if (enableMinimapTrail)
            {
                //Use the line renderer
                if (minimapLineRenderer != null)
                {       
                    // minimapLineRenderer.positionCount = (minimapPath.Length);
                    minimapLineRenderer.positionCount = minimapPathLength;

                    for (int i = 0; i < minimapPath.Length; ++i)
                    {
                        //Check if this point is inside the viewport
                        Vector3 displacement = Vector3.Scale((minimapPath[i] - PlayerHandler.instance.transform.position), new Vector3(1, 0, 1));
                        Vector3 displacementNormalized = displacement.normalized;
                        //Normalized value of that distance between the max size (query bounds) and not
                        float normalized = CustomUtility.Normalize(displacement.magnitude, 0, m_minimapCamera.orthographicSize + PlayerUIManager.instance.m_customRange);
                        float normalizedRejection = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_rejectionRange);

                        //If any index is outside, we can safely assume that nothing else will be rendered, and we can keep the cost of the line renderer down

                        //Get angle in which the displacement is done
                        Vector3 bap = displacement.normalized;
                        float angle = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;

                        if (normalizedRejection > PlayerUIManager.instance.m_rejectionRange)
                        {
                            //Set the rect transform stuffs
                            minimapTransformChecker.localPosition = Vector3.zero;
                            minimapTransformChecker.localEulerAngles = new Vector3(0, 0, angle + PlayerHandler.instance.transform.eulerAngles.y + 180);
                            //Translate the rectTransform based on eulers
                            minimapTransformChecker.Translate(0, PlayerUIManager.instance.m_customRangeTwo + 0.02f, 0);
                            ////Set scale
                            //rectTransform.localScale = new Vector3(normalizedScale, normalizedScale, normalizedScale);

                            minimapLineRenderer.SetPosition(i, minimapTransformChecker.localPosition);

                            minimapLineRenderer.positionCount = i + 1;
                            //Can start the break from here, else we just continue as usual.
                            break;
                        }
                        else
                        {
                            float normalizedCustomRange = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_customRangeTwo);

                            //Set the rect transform stuffs
                            minimapTransformChecker.localPosition = Vector3.zero;
                            minimapTransformChecker.localEulerAngles = new Vector3(0, 0, angle + PlayerHandler.instance.transform.eulerAngles.y + 180);
                            //Translate the rectTransform based on eulers
                            minimapTransformChecker.Translate(0, normalizedCustomRange, 0);
                            ////Set scale
                            //rectTransform.localScale = new Vector3(normalizedScale, normalizedScale, normalizedScale);

                            minimapLineRenderer.SetPosition(i, minimapTransformChecker.localPosition);
                        }
                    }

                }
            }

            if(enableSecondaryTrail)
            {
                minimapLineRendererVisualization.positionCount = minimapPathLength;

                for (int i = 0; i < minimapPathLength; ++i)
                {
                    minimapLineRendererVisualization.SetPosition(i, minimapPath[i] + lineOffset);
                }
            }
        }
    }
}
