using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/******************************  
** Name: Edge Detection Controller
** Desc: A class that controls the edge detection wave shader and calculations. (if there is a use it will stay)
** Author: Isaac
** Date: 10/12/2019, 11.31AM
**************************
** Change History
**************************
** PR   Date                            Author    Description 
** --   --------                                -------      ------------------------------------
** 1    10/12/2019, 11.31AM  Isaac   Created
** 2    10/12/2019, 11:42PM   Isaac   Implemented draft functions and variables
** 3    10/12/2019, 3.17PM     Isaac   Completed class and shaders
*******************************/
public class EdgeDetectionController : MonoBehaviour
{
    public static EdgeDetectionController instance;

    [Header("Scan Manager Configurations")]
    [SerializeField]
    //Whether to use player's transform
    private bool usePlayer = true;
    //Where to start the scan.
    [SerializeField]
    private Transform m_scanOrigin;
    //Edge detection material
    [SerializeField]
    public Material m_edgeDetectionMaterial;
    //Scan distance for the edge detection.
    [SerializeField]
    private float m_startScanDistance;
    [SerializeField]
    private float m_scanWidth;
    [SerializeField]
    private float m_scanSpeed;

    //Local variables
    private Camera m_camera;
    private float scanDistance;

    //Is the scan happening?
    bool isScanning;

    //Get and set
    public Transform scanOrigin
    {
        get
        {
            return m_scanOrigin;
        }
        set
        {
            m_scanOrigin = value;
        }
    }
    public float startScanDistance
    {
        get
        {
            return m_startScanDistance;
        }
        set
        {
            m_startScanDistance = value;
        }
    }
    public float scanWidth
    {
        get
        {
            return m_scanWidth;
        }
        set
        {
            m_edgeDetectionMaterial.SetFloat("_ScanWidth", m_scanWidth);
            m_scanWidth = value;
        }
    }
    public float scanSpeed
    {
        get
        {
            return m_scanSpeed;
        }
        set
        {
            m_scanSpeed = value;
        }
    }


    private void Awake()
    {
        //Scan Manager
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        if (usePlayer)
            m_scanOrigin = PlayerHandler.instance.transform;
    }

    private void OnEnable()
    {
        m_camera = GetComponent<Camera>();
        if (m_camera == null)
            m_camera = Camera.main;


        scanDistance = m_startScanDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            StartScanWithPosition(m_scanOrigin.position);

        if (isScanning)
            scanDistance += Time.deltaTime * m_scanSpeed;

        //always in update
        m_edgeDetectionMaterial.SetFloat("_ScanDistance", scanDistance);
    }

    public void StartScanWithPosition(Vector3 position)
    {
        isScanning = true;
        scanDistance = m_startScanDistance;
        m_edgeDetectionMaterial.SetVector("_ScannerPosition", position);
    }

    public void StartScan()
    {
        isScanning = true;
        scanDistance = m_startScanDistance;
        m_edgeDetectionMaterial.SetVector("_ScannerPosition", m_scanOrigin.position);
    }

    private void OnValidate()
    {
        //Update the material when the value changes
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        m_edgeDetectionMaterial.SetFloat("_ScanDistance", scanDistance);
        m_edgeDetectionMaterial.SetFloat("_ScanWidth", m_scanWidth);
        m_edgeDetectionMaterial.SetVector("_ScannerPosition", m_scanOrigin.position);
    }
}
