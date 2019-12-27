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
        transform.position = m_cameraTransform.position;
        transform.eulerAngles = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);
        reticleModule.SetupReticleModule();
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
        //if (Physics.Raycast(ray.origin, ray.direction, out hit, 300/*,projectileMask)*/)
        //{
        //    Debug.Log(hit.collider.gameObject.name);
        //    //SetReticlePosition(OVRInput.Controller.LTouch, hit.point);
        //    //SetHitObjectName(OVRInput.Controller.LTouch, hit.collider.gameObject.name);
        //}

        ScaleReticle(OVRInput.Controller.LTouch);
        ScaleReticle(OVRInput.Controller.RTouch);
    }

    void LateUpdate()
    {
        transform.position = m_cameraTransform.position;
        transform.eulerAngles = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
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
    public void RecenterPose()
    {
        OVRManager.display.RecenterPose();
        PlayerHandler.instance.ResetPose();

    }

    public void SetReticlePosition(OVRInput.Controller _controller, Vector3 _worldPosition)
    {
        //Get config I suppose.
        GUIReticleModule.GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        reticleConfig.m_reticleReference.transform.position = _worldPosition;
        reticleConfig.m_reticleReference.transform.LookAt(Camera.main.transform);
        reticleConfig.m_reticleReference.transform.Rotate(0, 180, 0);

        ScaleReticle(_controller);
    }

    public void SetHitObjectName(OVRInput.Controller _controller, string name)
    {
        GUIReticleModule.GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        reticleConfig.m_reticleText.text = name;

    }

    //Scaling the reticle to make its same size no matter what
    public void ScaleReticle(OVRInput.Controller _controller)
    {      
        //Get config I suppose.
        GUIReticleModule.GUIReticleConfig reticleConfig = _controller == OVRInput.Controller.RTouch ? reticleModule.RightReticle : reticleModule.LeftReticle;

        //Distance between camera and the reticle
        float pos = Vector3.Distance(Camera.main.transform.position, reticleConfig.m_reticleReference.transform.position);

        //Some scaling formula from my other fill stuff
        float h = Mathf.Tan(Camera.main.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f;
        reticleConfig.m_reticleReference.transform.localScale = new Vector3(h, h, h) * (reticleConfig.reticleSize * 0.01f);
    }

}
