using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightExplosion : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField]
    private GameObject m_debrisPrefab;
    [SerializeField]
    private Mesh[] m_debrisMeshes;
    [SerializeField]
    private Vector3 m_debrisPositionVariances = new Vector3(5, 5, 5);
    [SerializeField]
    private float m_explosionForceMin = 30.0f;
    [SerializeField]
    private float m_explosionForceMax = 200.0f;

    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        for (int i = 0; i < m_debrisMeshes.Length; ++i)
        {
            GameObject debris = Instantiate(m_debrisPrefab, transform);
            Vector3 asd = new Vector3(Random.Range(-m_debrisPositionVariances.x, m_debrisPositionVariances.x),
                                                         Random.Range(0.0f, m_debrisPositionVariances.y),
                                                         Random.Range(-m_debrisPositionVariances.z, m_debrisPositionVariances.z));
            debris.transform.localPosition = asd;
            debris.GetComponent<MeshFilter>().mesh = m_debrisMeshes[i];
            debris.GetComponent<Rigidbody>().AddExplosionForce(Mathf.Lerp(m_explosionForceMin, m_explosionForceMax, (float)i / m_debrisMeshes.Length), transform.position, 1000.0f);
            debris.GetComponent<Rigidbody>().AddTorque(asd * 10.0f);
        }
    }
}
