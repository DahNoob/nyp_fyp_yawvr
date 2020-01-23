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

    public GameObject lmao;


    //Local variables
    private Transform m_playerReference;
    private Vector3 desiredPosition;
    private Vector3 previousDesiredPosition;
    private float lerpTime;
    //Minimap ranges
    private float customRange = 20;
    private float customRangeTwo = 0.1f;


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

    public IEnumerator UpdateMinimap()
    {
        while (true)
        {

            List<GameObject> queries = QuadTreeManager.instance.QueryDynamicObjects(m_minimapBounds, QuadTreeManager.DYNAMIC_TYPES.ENEMIES);
            
            if(queries.Count >0 && queries[0] != null)
            {
                lmao.SetActive(true);
                RectTransform lmaoTransform = lmao.GetComponent<RectTransform>();
                //Vector3 displacement = Vector3.Scale(queries[0].transform.position - m_playerReference.transform.position, new Vector3(1, 0, 1));
                Vector3 displacement = (queries[0].transform.position - m_playerReference.transform.position);
                Vector3 bap = displacement.normalized;
                float lol = Mathf.Atan2(bap.x, -bap.z) * Mathf.Rad2Deg;
                lmaoTransform.localPosition = Vector3.zero;
                lmaoTransform.localEulerAngles = new Vector3(0, 0, lol + PlayerHandler.instance.transform.eulerAngles.y + 180);
                RectTransform childTransform = lmaoTransform.GetChild(0).GetComponent<RectTransform>();
                childTransform.rotation = Quaternion.Euler(new Vector3(0, PlayerHandler.instance.transform.eulerAngles.y, 0));

                float displacementMagnitude = displacement.magnitude;
                float normalized = CustomUtility.Normalize(displacementMagnitude, 0, m_minimapCamera.orthographicSize);
                float normalizedCustomRange = CustomUtility.NormalizeCustomRange(normalized, 0, customRangeTwo);
                lmaoTransform.Translate(0, normalizedCustomRange, 0);
            }
            else
            {
                lmao.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
            //yield return new WaitForSeconds(m_minimapPollRate);
        }
    }


}
