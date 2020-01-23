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
    [SerializeField] [Range(0, 40)]
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


    //Local variables
    private Transform m_playerReference;
    private Vector3 desiredPosition;
    private Vector3 previousDesiredPosition;
    private float lerpTime;

    private Dictionary<GameObject, bool> minimapPairDictionary = new Dictionary<GameObject, bool>();

    // Start is called before the first frame update
    public void Start()
    {
        //Assign the players reference
        m_playerReference = PlayerHandler.instance.transform;
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

    public GameObject lmao;

    public IEnumerator UpdateMinimap()
    {
        while (true)
        {

            List<GameObject> queries = QuadTreeManager.instance.QueryDynamicObjects(m_minimapBounds, QuadTreeManager.DYNAMIC_TYPES.ENEMIES);
            
            if(queries.Count >0 && queries[0] != null)
            {
                
                RectTransform lmaoTransform = lmao.GetComponent<RectTransform>();
                Vector2 thisPosition = CustomUtility.ToVector2(m_playerReference.transform.position);
                Vector2 thatPosition = CustomUtility.ToVector2(queries[0].transform.position);
                Vector3 displacement = Vector3.Scale(queries[0].transform.position - m_playerReference.transform.position, new Vector3(1, 0, 1));
                Vector3 bap = displacement.normalized;
                float dist = displacement.magnitude;
                float lol = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;
                lmaoTransform.localPosition = Vector3.zero;
                lmaoTransform.localEulerAngles = new Vector3(0, 0, lol + PlayerHandler.instance.transform.eulerAngles.y + 180);
                RectTransform childTransform = lmaoTransform.GetChild(0).GetComponent<RectTransform>();
                childTransform.rotation = Quaternion.Euler(new Vector3(0, PlayerHandler.instance.transform.eulerAngles.y, 0));
                lmaoTransform.Translate(0, Mathf.Min(0.14f, displacement.sqrMagnitude * 0.00005f), 0);
            }
            yield return null;
            //yield return new WaitForSeconds(m_minimapPollRate);
        }
    }


}
