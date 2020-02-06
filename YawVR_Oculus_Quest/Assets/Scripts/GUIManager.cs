using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    //[Header("Configurations")]
    //[SerializeField]
    //[Range(0.0f, 1.0f)]
    //private float m_uiRotationAlpha = 0.5f;

    public static GUIManager instance { private set; get; }

    [Header("Resources")]
    [SerializeField]
    private Transform m_cameraTransform;
    [SerializeField]
    private GameObject m_objectiveTextPanelPrefab;

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


    [Header("UI Panels")]
    [SerializeField]
    private RectTransform m_objectiveListPanel;

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
        //print("GUIManager events +attached+ successfully!");
        //transform.position = m_cameraTransform.position;
        //transform.eulerAngles = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);

        foreach (Transform obj in m_objectiveListPanel)
        {
            Destroy(obj.gameObject);
        }
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
        //transform.position = m_cameraTransform.position;
        //transform.eulerAngles = new Vector3(0, m_cameraTransform.eulerAngles.y, 0);
        m_fpsValue.text = fps.ToString();
        m_armRotationValue.text = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch).eulerAngles.ToString();
        //transform.Rotate(Vector3.up, m_cameraTransform.rotation.y);

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.L))
        {
            SpawnLightMech1();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            SpawnHeavyMech2();
        }
#endif
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
        //Instantiate(m_lightMech1Prefab, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 5 + Vector3.up * 3, PlayerHandler.instance.transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
        ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.LIGHT_MECH1, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 10 + Vector3.up * 3, PlayerHandler.instance.transform.rotation);
    }
    public void SpawnHeavyMech2()
    {
        //Instantiate(m_heavyMech2Prefab, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 10 + Vector3.up * 3, PlayerHandler.instance.transform.rotation, Persistent.instance.GO_DYNAMIC.transform);
        ObjectPooler.instance.SpawnFromPool(PoolObject.OBJECTTYPES.HEAVY_MECH2, PlayerHandler.instance.transform.position + PlayerHandler.instance.transform.forward * 10 + Vector3.up * 3, PlayerHandler.instance.transform.rotation);
    }
    public void RecenterPose()
    {
        OVRManager.display.RecenterPose();
        PlayerHandler.instance.ResetPose();
    }

}
