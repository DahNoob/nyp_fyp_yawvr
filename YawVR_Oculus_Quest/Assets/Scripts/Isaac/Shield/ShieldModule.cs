using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldModule : MonoBehaviour
{
    [Header("Shield Configurations")]
    [SerializeField]
    [Tooltip("Ring radius")]
    private float m_ringRadius = 1;
    [SerializeField]
    [Tooltip("How long the ring stays on the shield")]
    private float m_ringDuration = 0.5f;
    [SerializeField]
    [Tooltip("Ring width, lower makes it look like a ring, higher means a more solid circle")]
    [Range(0, 1)]
    private float m_ringWidth = 0.1f;

    //Can be seperated to a different class
    [Header("Shield Logic Configuration")]
    [SerializeField]
    private float m_shieldHP = 100;
    [SerializeField]
    private float m_shieldMaxHP = 100;
    [SerializeField]
    private float m_shieldMinHP = 0;
    [SerializeField]
    private float crackedStrength;

    [SerializeField]
    private float maxCrackedStrength;

    //Local variables.
    const int MAX_HIT_COUNT = 10;
    //Access to the renderer
    private MeshRenderer meshRenderer;
    private MaterialPropertyBlock materialPropertyBlock;

    //Arrays to store all the information
    private int m_hitCount;
    //Store an array of all the positions
    private Vector4[] m_hitPosition = new Vector4[MAX_HIT_COUNT];
    //Array of durations
    private float[] m_hitDuration = new float[MAX_HIT_COUNT];
    //Array of timers
    private float[] m_hitTimer = new float[MAX_HIT_COUNT];
    //Array of radiuses
    private float[] m_hitRadius = new float[MAX_HIT_COUNT];
    //Array of widths
    private float[] m_hitWidths = new float[MAX_HIT_COUNT];
    //Array of intensities
    private float[] m_hitIntensity = new float[MAX_HIT_COUNT];

    public Collider collider;

    private void Awake()
    {
        //Get access to the renderers
        meshRenderer = GetComponent<MeshRenderer>() ?? GetComponentInChildren<MeshRenderer>();
        //Initialize material property block
        materialPropertyBlock = new MaterialPropertyBlock();
        collider = GetComponent<Collider>();

        m_shieldHP = Mathf.Clamp(m_shieldHP, m_shieldMinHP, m_shieldMaxHP);
    }

    private void Update()
    {
        //Update cracked strength, if not can use colors i guess
        crackedStrength = (1 - (m_shieldHP - m_shieldMinHP) / (m_shieldMaxHP - m_shieldMinHP)) * maxCrackedStrength;

        //Still not sure how this works, need more visualization
        for (int i = 0; i < m_hitCount;)
        {
            m_hitTimer[i] += Time.deltaTime;
            if (m_hitTimer[i] > m_hitDuration[i])
            {
                AssignLast(i);
            }
            else
            {
                i++;
            }
        }
        UpdateMaterial();
    }

    //Such an insane function, I didnt even know you could do this
    //Don't need to sort any further
    void AssignLast(int id)
    {
        int last = m_hitCount - 1;
        if (id != last)
        {
            m_hitPosition[id] = m_hitPosition[last];
            m_hitDuration[id] = m_hitDuration[last];
            m_hitWidths[id] = m_hitWidths[last];
            m_hitRadius[id] = m_hitRadius[last];
            m_hitTimer[id] = m_hitTimer[last];
        }
        m_hitCount--;
    }

    //Add a hit at a certain position
    public void AddHit(Vector3 hitPosition)
    {
        int availableID = GetAvailableHit();
        //Set the variables in the array
        //Convert the position to local space.
        //If the shader uses world space, simply assigning it will do.
        //Note to self ^
        m_hitPosition[availableID] = transform.InverseTransformPoint(hitPosition);
        m_hitDuration[availableID] = m_ringDuration;
        m_hitRadius[availableID] = m_ringRadius;
        m_hitWidths[availableID] = m_ringWidth;
        m_hitTimer[availableID] = 0;
    }

    //Sort of queue system, without using the data structure
    public int GetAvailableHit()
    {
        //Don't want to instantiate things when theres nothing
        if (m_hitCount < MAX_HIT_COUNT)
        {
            m_hitCount += 1;
            return m_hitCount - 1;
        }
        else
        {
            //Find lowest duration to replace it
            float minDuration = float.MaxValue;
            int minID = 0;
            for (int i = 0; i < MAX_HIT_COUNT; i++)
            {
                if (m_hitDuration[i] < minDuration)
                {
                    minDuration = m_hitDuration[i];
                    minID = i;
                }
            }
            return minID;
        }
    }

    public void ClearHits()
    {
        m_hitCount = 0;
        UpdateMaterial();
    }

    public void UpdateMaterial()
    {
        meshRenderer.GetPropertyBlock(materialPropertyBlock);
        //Set all stuff in here.
        // materialPropertyBlock
        materialPropertyBlock.SetFloat("m_hitCount", m_hitCount);
        materialPropertyBlock.SetFloat("m_crackStrength", crackedStrength);
        materialPropertyBlock.SetFloatArray("m_hitRadius", m_hitRadius);
        materialPropertyBlock.SetFloatArray("m_hitWidth", m_hitWidths);
        materialPropertyBlock.SetVectorArray("m_hitObjectPosition", m_hitPosition);

        for (int i = 0; i < m_hitCount; i++)
        {
            if (m_hitDuration[i] > 0f)
            {
                //Gives a value, that reduces opacity based on the time
                m_hitIntensity[i] = 1 - Mathf.Clamp01(m_hitTimer[i] / m_hitDuration[i]);
            }
        }
        materialPropertyBlock.SetFloatArray("m_hitIntensity", m_hitIntensity);
        meshRenderer.SetPropertyBlock(materialPropertyBlock);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Bullet")
        {
            AddHit(other.gameObject.transform.position);
        }
    }

    //Simple take damage function for now
    public void TakeDamage(int damage)
    {
        m_shieldHP -= damage;
      //  m_shieldHP = Mathf.Max(0, m_shieldHP - damage);
    }


}
