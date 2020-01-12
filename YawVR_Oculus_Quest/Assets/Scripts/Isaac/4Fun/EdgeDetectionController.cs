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
    const int MAX_WAVE_COUNT = 10;

    //Int stuff
    [SerializeField]
    private int m_scanCount;
    //Store an array of all the positions
    [SerializeField]
    private Vector4[] m_scanPositions = new Vector4[MAX_WAVE_COUNT];
    //Array of distances
    [SerializeField]
    private float[] m_scanDistances = new float[MAX_WAVE_COUNT];
    //Array of widths
    [SerializeField]
    private float[] m_scanWidths = new float[MAX_WAVE_COUNT];
    //Array of sharpness
    [SerializeField]
    private float[] m_scanLeadSharps = new float[MAX_WAVE_COUNT];
    //Edge detection strengths
    [SerializeField]
    private float[] m_edgeDetectionStrengths = new float[MAX_WAVE_COUNT];
    //Colors
    [SerializeField]
    private Color[] m_midColors = new Color[MAX_WAVE_COUNT];
    [SerializeField]
    private Color[] m_leadColors = new Color[MAX_WAVE_COUNT];
    [SerializeField]
    private Color[] m_trailColors = new Color[MAX_WAVE_COUNT];
    [SerializeField]
    private Color[] m_edgeDetectionColors = new Color[MAX_WAVE_COUNT];

    //Local variables
    private Camera m_camera;
    private float scanDistance;
    //Scan durations
    [SerializeField] private float[] m_scanDurations = new float[MAX_WAVE_COUNT];
    [SerializeField] private float[] m_scanTimer = new float[MAX_WAVE_COUNT];
    [SerializeField] private float[] m_scanSpeeds = new float[MAX_WAVE_COUNT];

    private void Awake()
    {
        //Scan Manager
        if (instance == null)
            instance = this;
    }
    private void Start()
    {
        //if (usePlayer)
        //    m_scanOrigin = PlayerHandler.instance.transform;

    }

    private void OnEnable()
    {
        m_camera = GetComponent<Camera>();
        if (m_camera == null)
            m_camera = Camera.main;

    }

    // Update is called once per frame
    void Update()
    {
        //Update timers
        for(int i =0; i < m_scanCount;)
        {
            m_scanTimer[i] += Time.deltaTime;
            if (m_scanTimer[i] > m_scanDurations[i])
            {
                AssignLast(i);
            }
            else
            {
                i++;
            }
        }

        for(int i =0; i < m_scanCount; ++i)
        {
            m_scanDistances[i] += Time.deltaTime * m_scanSpeeds[i];
        }
        UpdateMaterial();
    }


    private void OnValidate()
    {
        //Update the material when the value changes
        UpdateMaterial();
    }

    void UpdateMaterial()
    {
        //Set count
        m_edgeDetectionMaterial.SetFloat("m_scanCount", m_scanCount);
        //Set positions
        m_edgeDetectionMaterial.SetVectorArray("m_scanPositions", m_scanPositions);
        //Set scan information
        m_edgeDetectionMaterial.SetFloatArray("m_scanDistances", m_scanDistances);
        m_edgeDetectionMaterial.SetFloatArray("m_scanWidths", m_scanWidths);
        m_edgeDetectionMaterial.SetFloatArray("m_scanLeadSharps", m_scanLeadSharps);
        m_edgeDetectionMaterial.SetFloatArray("m_edgeDetectionStrengths", m_edgeDetectionStrengths);
        m_edgeDetectionMaterial.SetColorArray("m_midColors", m_midColors);
        m_edgeDetectionMaterial.SetColorArray("m_leadColors", m_leadColors);
        m_edgeDetectionMaterial.SetColorArray("m_trailColors", m_trailColors);
        m_edgeDetectionMaterial.SetColorArray("m_edgeDetectionColors", m_edgeDetectionColors);
    }

    //Experimental functions
    public void ClearWaves()
    {
        m_scanCount = 0;
        UpdateMaterial();
    }

    public int GetAvailableWave()
    {
        if (m_scanCount < MAX_WAVE_COUNT)
        {
            m_scanCount += 1;
            return m_scanCount - 1;
        }
        else
        {
            //Find lowest duration to replace it
            float minDuration = float.MaxValue;
            int minID = 0;
            for (int i = 0; i < MAX_WAVE_COUNT; i++)
            {
                if (m_scanDurations[i] < minDuration)
                {
                    minDuration = m_scanDurations[i];
                    minID = i;
                }
            }
            return minID;
        }
    }

    void AssignLast(int id)
    {
        int last = m_scanCount - 1;
        if (id != last)
        {
            m_scanPositions[id] = m_scanPositions[last];
            m_scanDistances[id] = m_scanDistances[last];
            m_scanWidths[id] = m_scanWidths[last];
            m_scanLeadSharps[id] = m_scanLeadSharps[last];
            m_midColors[id] = m_midColors[last];
            m_leadColors[id] = m_leadColors[last];
            m_trailColors[id] = m_trailColors[last];
            m_edgeDetectionStrengths[id] = m_edgeDetectionStrengths[last];
            m_edgeDetectionColors[id] = m_edgeDetectionColors[last];
            m_scanDurations[id] = m_scanDurations[last];
            m_scanTimer[id] = m_scanTimer[last];
        }
        m_scanCount--;
    }

    public void AddWave(
        Vector3 scanPosition,
        float scanDuration = 5,
        float scanDistance = 0,
        float scanWidth = 100,
        float scanSharpness = 25,
        float scanSpeed = 250,
        float edgeDetectionStrength = 1,
        Color edgeDetectionColor = default(Color),
        Color midColor = default(Color),
        Color leadColor = default(Color),
        Color trailColor = default(Color))
    {
        int availableID = GetAvailableWave();
        m_scanPositions[availableID] = scanPosition;
        m_scanDistances[availableID] = scanDistance;
        m_scanWidths[availableID] = scanWidth;
        m_scanLeadSharps[availableID] = scanSharpness;
        m_midColors[availableID] = midColor == default(Color) ? new Color(1f,0.6f,0f,1f) : midColor;
        m_leadColors[availableID] = leadColor == default(Color) ? new Color(1f, 0.1294117f, 0.02352941f, 1f) : leadColor; 
        m_trailColors[availableID] = trailColor == default(Color) ? new Color(0f, 1f, 0.9803922f, 1f) : trailColor; 
        m_edgeDetectionColors[availableID] = edgeDetectionColor == default(Color) ? new Color(1f, 0.6f, 0f, 1f) : edgeDetectionColor; 
        m_edgeDetectionStrengths[availableID] = edgeDetectionStrength;
        m_scanDurations[availableID] = scanDuration;
        m_scanTimer[availableID] = 0;
        m_scanSpeeds[availableID] = scanSpeed;

    }

    //Testing functions
    public void AddWaveButton()
    {
        int availableID = GetAvailableWave();
        m_scanPositions[availableID] = PlayerHandler.instance.transform.position;
        m_scanDistances[availableID] = 0;
        m_scanWidths[availableID] = 100;
        m_scanLeadSharps[availableID] = 25;
        m_midColors[availableID] = GenerateRandomColor();
        m_leadColors[availableID] = GenerateRandomColor();
        m_trailColors[availableID] = GenerateRandomColor();
        m_edgeDetectionColors[availableID] = GenerateRandomColor();
        m_edgeDetectionStrengths[availableID] = 1;
        m_scanDurations[availableID] = 10;
        m_scanTimer[availableID] = 0;
        m_scanSpeeds[availableID] = 250;
    }

    public Color GenerateRandomColor()
    {
        return new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            1);
    }

}
