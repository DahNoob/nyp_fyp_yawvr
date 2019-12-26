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
    private UnityEngine.UI.Image m_rightReticle;
    [SerializeField]
    private UnityEngine.UI.Image m_leftReticle;

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
        UnityEngine.UI.Image reticle = _controller == OVRInput.Controller.RTouch ? m_rightReticle : m_leftReticle;
        reticle.rectTransform.position = Camera.main.WorldToScreenPoint(_worldPosition, Camera.MonoOrStereoscopicEye.Mono);
    }
}
