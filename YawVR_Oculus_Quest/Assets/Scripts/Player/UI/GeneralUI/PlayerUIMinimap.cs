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
    [HideInInspector]
    public QuadRect m_minimapBounds;
    [SerializeField]
    private float m_minimapPollRate;
    [SerializeField]
    private GameObject m_minimapParent;
    [SerializeField]
    private GameObject m_playerIcon;

    [Header("Main Minimap Settings")]
    [SerializeField]
    [Tooltip("Check this if the camera's FOV/size will not change ever")]
    private bool staticViewport;
    [SerializeField]
    private bool updateEnemies = true;
    [SerializeField]
    private bool updateObjectives = true;

    [Header("Global Minimap Icons Configuration")]
    //Initializer Data
    [SerializeField]
    private List<PlayerUIMinimapIcons> minimapIconListData;
    private Dictionary<int, PlayerUIMinimapIcons> minimapIconDictionary = new Dictionary<int, PlayerUIMinimapIcons>();

    [SerializeField]
    private List<PlayerUIMinimapObjectiveIcons> minimapIconObjectivesListData;
    private Dictionary<int, PlayerUIMinimapObjectiveIcons> minimapObjectiveDictionary = new Dictionary<int, PlayerUIMinimapObjectiveIcons>();

    [SerializeField]
    private float minScaleRange;
    [SerializeField]
    private float maxScaleRange;

    //Local variables
    private Transform m_playerReference;
    private Vector3 desiredPosition;
    private Vector3 previousDesiredPosition;
    private float lerpTime;
    //Normalized scale // higher minimap zoom lower icon size to give sense of depth
    float normalizedScale;
    //Minimap camera
    private Camera m_minimapCamera;

    private float minimapTimer = 0;

    private enum MINIMAP_ICONTYPE
    {
        ENEMIES,
        OBJECTIVES,
        TOTAL_ICONTYPE
    }

    //Storing list for the indexes
    private Dictionary<int, List<GameObject>> minimapIcons = new Dictionary<int, List<GameObject>>();

    // Start is called before the first frame update
    public void Start()
    {
        //Assign the players reference
        m_playerReference = PlayerHandler.instance.transform;
        m_minimapCamera = PlayerUIManager.instance.minimapCamera;

        //Initialise all things into dictionary

        for (int i = 0; i < minimapIconListData.Count; ++i)
        {
            minimapIconDictionary.Add((int)minimapIconListData[i].enemyType, minimapIconListData[i]);
        }

        for (int i = 0; i < minimapIconObjectivesListData.Count; ++i)
        {
            minimapObjectiveDictionary.Add((int)minimapIconObjectivesListData[i].objectiveType, minimapIconObjectivesListData[i]);
        }

        //Update list stuffs
        for (int i = 0; i < (int)MINIMAP_ICONTYPE.TOTAL_ICONTYPE; ++i)
        {
            minimapIcons.Add(i, new List<GameObject>());
        }

        //Initialise normalized Scale
        normalizedScale = CustomUtility.Normalize(80 - m_minimapZoom, 0, 80);
        //Normalize that value to another custom range
        normalizedScale = CustomUtility.NormalizeCustomRange(normalizedScale, minScaleRange, maxScaleRange);

        PlayerUIManager.instance.normalizedScale = normalizedScale;
    }

    // Update is called once per frame
    public void Update()
    {
        bool result = doAnimations ? AnimatedMinimap() : NonAnimatedMinimap();

        //Size of camera
        m_minimapCamera.orthographicSize = m_minimapZoom;

        //Update minimapBounds
        m_minimapBounds.width = m_minimapCamera.orthographicSize;
        m_minimapBounds.height = m_minimapCamera.orthographicSize;

        //Updates camera position
        UpdateCameraPosition();

        if (!staticViewport)
        {
            normalizedScale = CustomUtility.Normalize(80 - m_minimapZoom, 0, 80);
            //Normalize that value to another custom range
            normalizedScale = CustomUtility.NormalizeCustomRange(normalizedScale, minScaleRange, maxScaleRange);

            PlayerUIManager.instance.normalizedScale = normalizedScale;
        }
        //Set scale of that.
        m_playerIcon.transform.localScale = new Vector3(normalizedScale, normalizedScale, normalizedScale);


        if (minimapTimer < m_minimapPollRate)
        {
            minimapTimer += Time.smoothDeltaTime;
        }
        else
        {
            if (updateEnemies)
                UpdateEnemies();
            if (updateObjectives)
                UpdateObjectives();

            minimapTimer = 0;
        }



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

    public bool UpdateEnemies()
    {
        //Update queries
        List<GameObject> queries = QuadTreeManager.instance.QueryDynamicObjects(m_minimapBounds, QuadTreeManager.DYNAMIC_TYPES.ENEMIES);
        //Amount of enemies
        int queriesCount = queries.Count;

        int tag = (int)MINIMAP_ICONTYPE.ENEMIES;

        List<GameObject> enemyList = minimapIcons[tag];

        //Spawn from pool 
        if (enemyList.Count != queriesCount)
        {
            //Instantiate all the way to the count
            while (enemyList.Count < queriesCount)
            {
                //Instantiate that thingy and do stuff
                GameObject minimapIconObject = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.MINIMAP_ICONS, Vector3.zero, Quaternion.identity);
                enemyList.Add(minimapIconObject);

                //Set the parent
                minimapIconObject.transform.SetParent(m_minimapParent.transform);
            }
        }

        //Loop through the list...
        for (int i = 0; i < queriesCount; ++i)
        {
            Vector3 displacement = Vector3.Scale((queries[i].transform.position - m_playerReference.transform.position), new Vector3(1, 0, 1));

            //Normalized value of that distance between the max size (query bounds) and not
            float normalized = CustomUtility.Normalize(displacement.magnitude, 0, m_minimapCamera.orthographicSize + PlayerUIManager.instance.m_customRange);
            float normalizedRejection = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_rejectionRange);

            if (normalizedRejection > PlayerUIManager.instance.m_rejectionRange)
            {
                HandleActive(enemyList[i], false);
            }
            else
            {
                HandleActive(enemyList[i], true);

                //Get angle in which the displacement is done
                Vector3 bap = displacement.normalized;
                float angle = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;

                //Calculate space in minimap
                float normalizedCustomRange = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_customRangeTwo);

                //Loop through all the enemies, then set acctive based on the result
                RectTransform rectTransform = enemyList[i].GetComponent<RectTransform>();
                //Set the rect transform stuffs
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = new Vector3(0, 0, angle + PlayerHandler.instance.transform.eulerAngles.y + 180);
                //Translate the rectTransform based on eulers
                rectTransform.Translate(0, normalizedCustomRange, 0);
                //Set scale
                rectTransform.localScale = new Vector3(normalizedScale, normalizedScale, normalizedScale);

                //Set child transform values
                RectTransform childTransform = rectTransform.GetChild(0).GetComponent<RectTransform>();
                childTransform.rotation = Quaternion.Euler(new Vector3(0, PlayerHandler.instance.transform.eulerAngles.y, 0));

                //Get the current enemy stuff for their enemytype
                EnemyInfo.ENEMY_TYPE enemyType = queries[i].GetComponent<EnemyBase>().enemyInfo.enemyType;

                PlayerUIMinimapIcons currentData = GetMinimapData(enemyType);

                if (currentData != null)
                {
                    LineRenderer lineRenderer = enemyList[i].GetComponent<LineRenderer>();

                    //Set data lul
                    lineRenderer.startColor = currentData.lineRendererStartColor;
                    lineRenderer.endColor = currentData.lineRendererEndColor;

                    Image currImage = enemyList[i].GetComponent<Image>();
                    currImage.color = currentData.m_circleTint;
                    currImage.sprite = currentData.m_circleSprite;

                    Image childImage = childTransform.GetComponent<Image>();
                    childImage.color = currentData.m_iconTint;
                    childImage.sprite = currentData.m_iconSprite;

                    float newScale = normalizedScale + currentData.scaleOffset;
                    //Set scale
                    rectTransform.localScale = new Vector3(newScale, newScale, newScale);
                }
            }
        }

        if (enemyList.Count > queriesCount)
        {
            for (int i = queriesCount; i < enemyList.Count; ++i)
            {
                HandleActive(enemyList[i], false);
            }
        }

        return true;

    }

    void UpdateObjectives()
    {
        //Get the appropriate list
        int tag = (int)MINIMAP_ICONTYPE.OBJECTIVES;
        List<GameObject> objectiveIconList = minimapIcons[tag];

        int objectivesCount = Game.instance.m_objectives.Length;

        //Spawn from pool 
        if (objectiveIconList.Count != objectivesCount)
        {
            //Instantiate all the way to the count
            while (objectiveIconList.Count < objectivesCount)
            {
                //Instantiate that thingy and do stuff
                GameObject minimapIconObject = ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.MINIMAP_ICONS, Vector3.zero, Quaternion.identity);
                objectiveIconList.Add(minimapIconObject);

                //Set the parent
                minimapIconObject.transform.SetParent(m_minimapParent.transform);
            }
        }

        //Idk i  will optimise later hopefully
        for (int i = 0; i < objectivesCount; ++i)
        {
            //Ignore objective if the objective null or something
            if (Game.instance.m_objectives[i] == null
                || Game.instance.m_objectives[i].m_highlight == null
                || Game.instance.m_objectives[i].m_completed)
            {
                HandleActive(objectiveIconList[i], false);
                continue;
            }
            Vector3 displacement = Vector3.Scale((Game.instance.m_objectives[i].m_highlight.position - m_playerReference.transform.position), new Vector3(1, 0, 1));

            //Normalized value of that distance between the max size (query bounds) and not
            float normalized = CustomUtility.Normalize(displacement.magnitude, 0, m_minimapCamera.orthographicSize + PlayerUIManager.instance.m_customRange);
            float normalizedRejection = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_rejectionRange);

            //Reject anything outside the range
            if (normalizedRejection > PlayerUIManager.instance.m_rejectionRange)
            {
                HandleActive(objectiveIconList[i], false);
            }
            else
            {
                HandleActive(objectiveIconList[i], true);

                //Get angle in which the displacement is done
                Vector3 bap = displacement.normalized;
                float angle = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;

                //Calculate space in minimap
                float normalizedCustomRange = CustomUtility.NormalizeCustomRange(normalized, 0, PlayerUIManager.instance.m_customRangeTwo);

                //Loop through all the objectives, then set acctive based on the result
                RectTransform rectTransform = objectiveIconList[i].GetComponent<RectTransform>();
                //Set the rect transform stuffs
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = new Vector3(0, 0, angle + PlayerHandler.instance.transform.eulerAngles.y + 180);
                //Translate the rectTransform based on eulers
                rectTransform.Translate(0, normalizedCustomRange, 0);
                //Set scale
                rectTransform.localScale = new Vector3(normalizedScale, normalizedScale, normalizedScale);

                //Set child transform values
                RectTransform childTransform = rectTransform.GetChild(0).GetComponent<RectTransform>();
                childTransform.rotation = Quaternion.Euler(new Vector3(0, PlayerHandler.instance.transform.eulerAngles.y, 0));

                //Get the current enemy stuff for their enemytype
                VariedObjectives.TYPE objectivesType = Game.instance.m_objectives[i].type;
                //Get current objectives
                PlayerUIMinimapObjectiveIcons currentData = GetMinimapData(objectivesType);

                if (currentData != null)
                {
                    LineRenderer lineRenderer = objectiveIconList[i].GetComponent<LineRenderer>();

                    //Set data lul
                    lineRenderer.startColor = currentData.lineRendererStartColor;
                    lineRenderer.endColor = currentData.lineRendererEndColor;

                    Image currImage = objectiveIconList[i].GetComponent<Image>();
                    currImage.color = currentData.m_circleTint;
                    currImage.sprite = currentData.m_circleSprite;

                    Image childImage = childTransform.GetComponent<Image>();
                    childImage.color = currentData.m_iconTint;
                    childImage.sprite = currentData.m_iconSprite;

                    float newScale = normalizedScale + currentData.scaleOffset;
                    //Set scale
                    rectTransform.localScale = new Vector3(newScale, newScale, newScale);
                }
            }


        }
    }

    void HandleActive(GameObject referenceObject, bool desiredActive)
    {
        if (referenceObject.activeInHierarchy != desiredActive)
            referenceObject.SetActive(desiredActive);
    }

    PlayerUIMinimapIcons GetMinimapData(EnemyInfo.ENEMY_TYPE enemyType)
    {
        int tag = (int)enemyType;

        if (minimapIconDictionary.ContainsKey(tag))
            return minimapIconDictionary[tag];

        return null;
    }
    //Getter function for getting the sprites properly
    PlayerUIMinimapObjectiveIcons GetMinimapData(VariedObjectives.TYPE objectivesType)
    {
        int tag = (int)objectivesType;

        if (minimapObjectiveDictionary.ContainsKey(tag))
            return minimapObjectiveDictionary[tag];

        return null;
    }

    void UpdateCameraPosition()
    {
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

    }

}

