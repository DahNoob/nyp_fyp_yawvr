using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveStructure : BaseStructure
{
    [Header("Objective Structure Configuration")]
    [SerializeField]
    private MeshRenderer m_objectiveRing;

    private bool isCurrentObjective = false;
    private float ringOrigHeight;

    void Awake()
    {
        ringOrigHeight = m_objectiveRing.transform.localPosition.y;
    }

    void Update()
    {
        if(m_objectiveRing)
        {
            //m_objectiveRing.material.SetTextureOffset("_BaseMap", new Vector2(Time.time * 0.05f, 0));
            m_objectiveRing.transform.Rotate(0, 5 * Time.deltaTime, 0);
        }
        if (isCurrentObjective)
        {
            float cos = Mathf.Cos(Time.time);
            m_objectiveRing.transform.localPosition = new Vector3(0, ringOrigHeight + cos, 0);
            //m_objectiveRing.transform.Rotate(0, 5 * Time.deltaTime, 0);
            m_objectiveRing.material.SetColor("_BaseColor", new Color(0.4f, 1, 0.17f, cos.Remap(-1.0f, 1.0f, 0.2f, 1)));
        }
    }

    public void SetRingRadius(float _rad)
    {
        m_objectiveRing.transform.localScale = new Vector3(_rad, _rad * 0.15f, _rad);
    }

    public void SetCurrentObjective(bool _isCurrent)
    {
        if(_isCurrent)
        {
            isCurrentObjective = true;
        }
        else
        {
            isCurrentObjective = false;
            StartCoroutine(fadeOutRing());
        }
    }

    IEnumerator fadeOutRing()
    {
        while(m_objectiveRing.material.color.a > 0)
        {
            yield return new WaitForSeconds(0.1f);
            m_objectiveRing.material.SetColor("_BaseColor", Color.LerpUnclamped(m_objectiveRing.material.GetColor("_BaseColor"), Persistent.instance.COLOR_TRANSPARENT, 0.1f));
        }
        Destroy(m_objectiveRing.gameObject);
    }
}