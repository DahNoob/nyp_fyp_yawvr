using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyCratesDropHandler : MonoBehaviour
{
    [Header("Configuration")]
    public Transform m_crateA;
    public Transform m_crateB;
    public Transform m_crateC;

    private void Start()
    {
        StartDrop();
    }

    public void StartDrop()
    {
        GetComponent<Animator>().Play("Drop");
    }

    void _AnimationEnd()
    {
        Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE, m_crateA.position, m_crateA.rotation, Persistent.instance.GO_STATIC.transform);
        Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE, m_crateB.position, m_crateB.rotation, Persistent.instance.GO_STATIC.transform);
        Instantiate(Persistent.instance.PREFAB_SUPPLYCRATE, m_crateC.position, m_crateC.rotation, Persistent.instance.GO_STATIC.transform);
        Destroy(gameObject);
    }
}
