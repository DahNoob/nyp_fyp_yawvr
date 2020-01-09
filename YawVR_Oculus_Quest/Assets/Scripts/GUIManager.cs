using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIManager : MonoBehaviour
{
    //[Header("Configurations")]
    //[SerializeField]
    //[Range(0.0f, 1.0f)]
    //private float m_uiRotationAlpha = 0.5f;

    public static GUIManager instance { private set; get; }

    [Header("Configuration")]
    [SerializeField]
    private GUIReticleModule reticleModule;

    [Header("Resources")]
    [SerializeField]
    private Transform m_cameraTransform;

    [Header("Experimental Resources")]
    [SerializeField]
    private GameObject m_dumbCubes;
    [SerializeField]
    private GameObject m_heavyMech2Prefab;
    [SerializeField]
    private GameObject m_lightMech1Prefab;

    [Header("Debug Resources")]
    [SerializeField]
    private UnityEngine.UI.Text m_fpsValue;
    [SerializeField]
    private UnityEngine.UI.Text m_armRotationValue;

    //Local variables
    int frameCount = 0;
    float dt = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.

    void Awake()
    {
        if (instance == null)
            instance = this;
        print("GUIManager awake!");
    }

    void Start()
    {
        //transform.position = m_cameraTransform.position;
        //transform.eulerAngles = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);
        reticleModule.SetupReticleModule();
        reticleModule.SetupReticleColors();

        //Disable both reticles
        EnableReticle(OVRInput.Controller.LTouch, false);
        EnableReticle(OVRInput.Controller.RTouch, false);
    }

    void Update()
    {
        frameCount++;
        dt += Time.deltaTime;
        if (dt > 1.0 / updateRate)
        {
            fps = frameCount / dt;
            frameCount = 0;
            dt -= 1.0f / updateRate;
        }
        //RaycastHit hit;
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawRay(ray.origin, ray.direction * 1000);
        //LayerMask projectileMask = ~LayerMask.GetMask("PlayerProjectile");
        //if (Physics.Raycast(ray.origin, ray.direction, out hit, 300, projectileMask))
        //{
        //    SetReticleInformation(OVRInput.Controller.LTouch, hit.point, hit.collider.gameObject, true);
        //}

        //if (Input.GetMouseButtonDown(0))
        //{
        //    Triggered(OVRInput.Controller.LTouch);
        //}

        reticleModule.UpdateEase();
    }

    void LateUpdate()
    {
        //transform.position = m_cameraTransform.position;
        //transform.eulerAngles = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
        m_fpsValue.text = fps.ToString();
        m_armRotationValue.text = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles.ToString();
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);
    }


    public void SpawnCubes()
    {
        Instantiate(m_dumbCubes).transform.SetParent(GameObject.Find("CubePile").transform);
    }
    public void ClearCubes()
    {
        foreach (Transform item in GameObject.Find("CubePile").transform)
        {
            Destroy(item.gameObject);
        }
    }
    public void SpawnLightMech1()
    {
        Instantiate(m_lightMech1Prefab, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 5 + Vector3.up * 3, PlayerHandler.instance.transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
    }
    public void SpawnHeavyMech2()
    {
        Instantiate(m_heavyMech2Prefab, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 5 + Vector3.up * 3, PlayerHandler.instance.transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
    }
    public void RecenterPose()
    {
        OVRManager.display.RecenterPose();
        PlayerHandler.instance.ResetPose();
    }

    public void SetReticleInformation(OVRInput.Controller _controller, Vector3 hitPoint, GameObject hitObject, bool useTag = true)
    {
        //Set the reticle position based on raycasted position
        SetReticlePosition(_controller, hitPoint);
        //Set hit object name by accessing the object
        SetHitObjectName(_controller, hitObject);
        //Set the hit color by accessing the layer of the object
        SetReticleColor(_controller, hitObject, useTag);
        //Scale the reticle to be correct scale
        ScaleReticle(_controller);

        if (hitObject != null)
        {
            GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;
            //Temporary code
            reticleConfig.ObjectOfInterest(hitObject.tag == "Enemy");
        }
    }

    public void SetReticlePosition(OVRInput.Controller _controller, Vector3 _worldPosition)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        reticleConfig.reticleReference.transform.position = _worldPosition;
        reticleConfig.reticleReference.transform.LookAt(Camera.main.transform);
        reticleConfig.reticleReference.transform.Rotate(0, 180, 0);

        ScaleReticle(_controller);
    }

    public void SetHitObjectName(OVRInput.Controller _controller, GameObject hitObject)
    {
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        reticleConfig.reticleText.text = hitObject == null ? "N/A" : hitObject.name;
    }

    public void SetReticleColor(OVRInput.Controller _controller, GameObject hitObject, bool useTag)
    {
        //Access the reticle
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        if (hitObject == null)
        {
            reticleConfig.SetReticleDefaultColor();
            return;
        }

        //Access the color config
        GUIReticleColorConfig colorConfig = reticleModule.ReticleColors;

        //If the existing result is contained
        bool containsResult = useTag ? colorConfig.ContainsTag(hitObject.tag) : colorConfig.ContainsLayer(hitObject.layer);

        if (containsResult)
        {
            //If the result is contained
            Color resultantColor = useTag ? colorConfig.QueryTagColor(hitObject.tag) : colorConfig.QueryLayerColor(hitObject.layer);
            reticleConfig.SetReticleColor(resultantColor);
        }
        else
        {
            reticleConfig.SetReticleDefaultColor();
        }
    }

    //Scaling the reticle to make its same size no matter what
    public void ScaleReticle(OVRInput.Controller _controller)
    {      
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        //Distance between camera and the reticle
        float pos = Vector3.Distance(Camera.main.transform.position, reticleConfig.reticleReference.transform.position);

        //Some scaling formula from my other fill stuff
        float h = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;
        reticleConfig.reticleReference.transform.localScale = new Vector3(h, h, h) * (reticleConfig.reticleSize * 0.01f);
    }

    public void Triggered(OVRInput.Controller _controller)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;
        //reticleConfig.Triggered();
    }

    public void EnableReticle(OVRInput.Controller _controller, bool enabled)
    {
        //Get config I suppose.
        GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;
        reticleConfig.reticleReference.SetActive(enabled);
    }

}
