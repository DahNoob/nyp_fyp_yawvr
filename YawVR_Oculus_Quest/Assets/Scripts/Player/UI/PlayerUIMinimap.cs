using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerUIMinimap
{
    [Header("Minimap Configuration")]
    [SerializeField]
    [Tooltip("Allow animations to be handled")]
    private bool doAnimations = true;
    [SerializeField]
    [Tooltip("The camera rendering this minimap")]
    private Camera m_minimapCamera;
    [SerializeField]
    private float m_minimapOffset;
    [SerializeField]
    [Range(0, 80)]
    private float m_minimapZoom;

    [Header("Inner Ring Configuration")]
    [SerializeField]
    [Tooltip("The inner ring object")]
    private Image m_innerRing;
    [SerializeField]
    private RectTransform m_innerRingTransform;
    [SerializeField]
    [Tooltip("Inner Ring Rotation Speed")]
    private float m_innerRingRotationSpeed;
    [SerializeField]
    [Tooltip("Direction to rotate")]
    private bool m_innerRingRotateClockwise;

    [Header("Outer Ring Configuration")]
    [SerializeField]
    [Tooltip("The inner ring object")]
    private Image m_outerRing;
    [SerializeField]
    private RectTransform m_outerRingTransform;
    [SerializeField]
    [Tooltip("Inner Ring Rotation Speed")] //Lerps towards the desired rotation
    private float m_outerRingRotationSpeed;

    [Header("Camera Configuration")]
    [SerializeField]
    [Range(0.5f, 1.0f)]
    private float cameraMultiplier = 0.95f;
    [SerializeField]
    private float cameraLerpSpeed = 1;

    [Header("Main Minimap Configuration")]
    [SerializeField]
    public QuadRect m_minimapBounds;
    [SerializeField]
    private float m_minimapPollRate;
    [SerializeField]
    private GameObject m_minimapParent;
    [SerializeField]
    private List<GameObject> minimapIconsList = new List<GameObject>();

    [Header("Minimap Icons Configuration")]
    private Dictionary<int, PlayerUIMinimapIcons> minimapIconDictionary = new Dictionary<int, PlayerUIMinimapIcons>();
    [SerializeField]
    private List<PlayerUIMinimapIcons> minimapIconListData;
    [SerializeField]
    private float minScaleRange;
    [SerializeField]
    private float maxScaleRange;


    //Local variables
    private Transform m_playerReference;
    private Vector3 desiredPosition;
    private Vector3 previousDesiredPosition;
    private float lerpTime;
    //Minimap ranges
    public float customRange = 20;
    public float customRangeTwo = 0.1f;
    public float rejectionRange = 3;


    // Start is called before the first frame update
    public void Start()
    {
        //Assign the players reference
        m_playerReference = PlayerHandler.instance.transform;

        //Initialise all things into dictionary

        for (int i = 0; i < minimapIconListData.Count; ++i)
        {
            minimapIconDictionary.Add((int)minimapIconListData[i].enemyType, minimapIconListData[i]);
        }
    }

    // Update is called once per frame
    public void Update()
    {
        bool result = doAnimations ? AnimatedMinimap() : NonAnimatedMinimap();

        //Size of camera
        m_minimapCamera.orthographicSize = m_minimapZoom;

        //Do raycast upwards onto terrain to bring it down if needed.
        Ray ray = new Ray(m_playerReference.transform.position, Vector3.up);
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
        if (Physics.Raycast(ray.origin, ray.direction, out hit, float.MaxValue, LayerMask.GetMask("Terrain")))
        {
            //Fake kinda of camera stuff for the minimap
            float desired = hit.distance * cameraMultiplier;
            if (desired < m_minimapOffset)
                desiredPosition.y = desired;
        }
        else
        {
            //Calculate the offsets for minimaps and stuff
            desiredPosition = m_minimapCamera.transform.localPosition;
            desiredPosition.y = m_minimapOffset;
        }

        if (previousDesiredPosition != desiredPosition)
        {
            previousDesiredPosition = desiredPosition;
            lerpTime = 0;
        }

        if (lerpTime != 1)
        {
            lerpTime += Time.deltaTime * cameraLerpSpeed;
            lerpTime = Mathf.Min(lerpTime, 1f);
            //Lerp towards the desiredPosition always
            m_minimapCamera.transform.localPosition = Vector3.Lerp(m_minimapCamera.transform.localPosition, desiredPosition, lerpTime);
        }

        m_minimapBounds.width = m_minimapCamera.orthographicSize;
        m_minimapBounds.height = m_minimapCamera.orthographicSize;

    }
    bool AnimatedMinimap()
    {
        //Apply rotations to the inner ring on the z-axis
        if (m_innerRing != null)
        {
            float innerRotationResult = m_innerRingRotateClockwise ? -m_innerRingRotationSpeed : m_outerRingRotationSpeed;
            m_innerRingTransform.Rotate(Vector3.forward * Time.smoothDeltaTime * innerRotationResult);
        }

        if (m_outerRing != null)
        {

        }

        return true;
    }
    bool NonAnimatedMinimap()
    {
        //Do not even attempt to spin the inner ring.

        return true;
    }
    public List<GameObject> queries;

    public IEnumerator UpdateMinimap()
    {
        while (true)
        {
            //Fill up the list once more
            queries = QuadTreeManager.instance.QueryDynamicObjects(m_minimapBounds, QuadTreeManager.DYNAMIC_TYPES.ENEMIES);
            int queriesCount = queries.Count;
            if (minimapIconsList.Count != queriesCount)
            {
                //Instantiate all the way to the count
                while (minimapIconsList.Count < queriesCount)
                {
                    //Instantiate that thingy and do stuff
                    GameObject minimapIconObject = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.MINIMAP_ICONS, Vector3.zero, Quaternion.identity);
                    minimapIconsList.Add(minimapIconObject);

                    //Set the parent
                    minimapIconObject.transform.SetParent(m_minimapParent.transform);
                }
            }

            //Set active false all
            for (int i = 0; i < minimapIconsList.Count; ++i)
            {
                //Set all to false, then set back everything
                minimapIconsList[i].SetActive(false);
            }

            //Set it back with the appopriate calculations.
            for (int i = 0; i < queries.Count; ++i)
            {
                minimapIconsList[i].SetActive(true);
                //Loop through all the enemies, then set acctive based on the result
                RectTransform rectTransform = minimapIconsList[i].GetComponent<RectTransform>();
                //Vector3 displacement = Vector3.Scale(queries[0].transform.position - m_playerReference.transform.position, new Vector3(1, 0, 1));
                Vector3 displacement = Vector3.Scale((queries[i].transform.position - m_playerReference.transform.position), new Vector3(1, 0, 1));

                //Normalized value of that distance between the max size (query bounds) and not
                float normalized = CustomUtility.Normalize(displacement.magnitude, 0, m_minimapCamera.orthographicSize + customRange);
                float normalizedRejection = CustomUtility.NormalizeCustomRange(normalized, 0, rejectionRange);

                if (normalizedRejection > rejectionRange)
                    minimapIconsList[i].SetActive(false);
                else
                {
                    //Get angle in which the displacement is done
                    Vector3 bap = displacement.normalized;
                    float angle = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;

                    //Calculate space in minimap
                    float normalizedCustomRange = CustomUtility.NormalizeCustomRange(normalized, 0, customRangeTwo);

                    //Set the rect transform stuffs
                    rectTransform.localPosition = Vector3.zero;
                    rectTransform.localEulerAngles = new Vector3(0, 0, angle + PlayerHandler.instance.transform.eulerAngles.y + 180);
                    //Translate the rectTransform based on eulers
                    rectTransform.Translate(0, normalizedCustomRange, 0);

                    //Calculate the scale of everything
                    float normalizedScale = CustomUtility.Normalize(80 - m_minimapZoom, 0, 80);
                    normalizedScale = CustomUtility.NormalizeCustomRange(normalizedScale, minScaleRange, maxScaleRange);
                    rectTransform.localScale = new Vector3(normalizedScale, normalizedScale, normalizedScale);



                    //Set child transform values
                    RectTransform childTransform = rectTransform.GetChild(0).GetComponent<RectTransform>();
                    childTransform.rotation = Quaternion.Euler(new Vector3(0, PlayerHandler.instance.transform.eulerAngles.y, 0));

                    LineRenderer lineRenderer = minimapIconsList[i].GetComponent<LineRenderer>();

                    //Get the current enemy stuff for their enemytype
                    EnemyInfo.ENEMY_TYPE enemyType = queries[i].GetComponent<EnemyBase>().enemyInfo.enemyType;

                    PlayerUIMinimapIcons currentData = GetMinimapData(enemyType);

                    if (currentData != null)
                    {
                        //Set data lul
                        lineRenderer.startColor = currentData.lineRendererStartColor;
                        lineRenderer.endColor = currentData.lineRendererEndColor;

                        Image currImage = minimapIconsList[i].GetComponent<Image>();
                        currImage.color = currentData.m_circleTint;
                        currImage.sprite = currentData.m_circleSprite;

                        Image childImage = childTransform.GetComponent<Image>();
                        childImage.color = currentData.m_iconTint;
                        childImage.sprite = currentData.m_iconSprite;
                    }
                }


            }
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(m_minimapPollRate);
        }
    }

    //Getter function for getting the sprites properly
    PlayerUIMinimapIcons GetMinimapData(EnemyInfo.ENEMY_TYPE enemyType)
    {
        int tag = (int)enemyType;

        if (minimapIconDictionary.ContainsKey(tag))
            return minimapIconDictionary[tag];

        return null;
    }


}

